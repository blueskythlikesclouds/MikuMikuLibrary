using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Enrs;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class Section<T> : ISection where T : new()
    {
        private readonly List<ISection> mSections = new List<ISection>();
        private T mData;
        private bool mIsDataProcessed;
        private bool mIsProcessingData;

        public T Data
        {
            get
            {
                if ( !mIsDataProcessed && !mIsProcessingData )
                    ProcessData();

                return mData;
            }
        }

        object ISection.Data => Data;

        public Type DataType => typeof( T );

        public string Signature => SectionInfo.Signature;

        public abstract SectionFlags Flags { get; }

        public SectionInfo SectionInfo { get; }

        public IEnumerable<ISection> Sections => mSections;

        public Stream BaseStream { get; private set; }

        public SectionMode Mode { get; }

        public long DataOffset { get; private set; }

        public long DataSize { get; private set; }

        public long SectionSize { get; private set; }

        public virtual Endianness Endianness { get; set; }

        public virtual AddressSpace AddressSpace { get; set; }

        public BinaryFormat Format => AddressSpace.GetCorrespondingModernFormat();

        public EndianBinaryReader Reader { get; private set; }

        public EndianBinaryWriter Writer { get; private set; }

        public void Read( Stream source )
        {
            Read( source, false );
        }

        public void Write( Stream destination )
        {
            Write( destination, 0 );
        }

        void ISection.Read( Stream source, bool skipSignature )
        {
            Read( source, skipSignature );
        }

        void ISection.Write( Stream destination, int depth )
        {
            Write( destination, depth );
        }

        public virtual void Dispose()
        {
            Reader?.Dispose();
            Writer?.Dispose();

            foreach ( var section in mSections )
                section.Dispose();
        }

        public void ProcessData()
        {
            if ( mIsDataProcessed )
                return;

            mIsProcessingData = true;

            switch ( Mode )
            {
                case SectionMode.Read when BaseStream == null || Reader == null:
                    throw new InvalidOperationException( "Section has not been read yet, cannot process data object" );

                case SectionMode.Read:
                {
                    if ( mData == null )
                        mData = new T();

                    foreach ( var section in mSections )
                    {
                        if ( SectionInfo.SubSectionInfos.TryGetValue( section.SectionInfo, out var subSectionInfo ) )
                            subSectionInfo.ProcessPropertyForReading( section, this );
                    }

                    Reader.SeekBegin( DataOffset );
                    {
                        Read( mData, Reader, DataSize );
                    }
                    Reader.SeekBegin( DataOffset + SectionSize );

                    break;
                }

                case SectionMode.Write when BaseStream == null || Writer == null:
                    throw new InvalidOperationException( "Data object has not been written yet, cannot process data object" );

                case SectionMode.Write:
                {
                    mSections.Clear();

                    if ( IsRelocationTableWorthWriting() )
                    {
                        ISection relocationTableSection;

                        var offsets = Writer.OffsetPositions.Select( x => x - Writer.BaseOffset ).ToList();

                        if ( AddressSpace == AddressSpace.Int64 )
                            relocationTableSection = new RelocationTableSectionInt64( SectionMode.Write, offsets );

                        else
                            relocationTableSection = new RelocationTableSectionInt32( SectionMode.Write, offsets );

                        mSections.Add( relocationTableSection );
                    }

                    if ( DataSize > 0 && Writer is EnrsBinaryWriter enrsWriter )
                        mSections.Add( new EnrsSection( SectionMode.Write, enrsWriter.CreateScopeDescriptors( DataOffset, DataOffset + DataSize ) ) );

                    foreach ( var subSectionInfo in SectionInfo.SubSectionInfos.Values.OrderBy( x => x.Priority ) )
                        mSections.AddRange( subSectionInfo.ProcessPropertyForWriting( this ) );

                    if ( mSections.Count > 0 )
                        mSections.Add( new EndOfFileSection( SectionMode.Write, this ) );

                    break;
                }
            }

            mIsDataProcessed = true;
            mIsProcessingData = false;
        }

        private void Read( Stream source, bool skipSignature )
        {
            if ( Mode == SectionMode.Write )
                throw new InvalidOperationException( "Section is in write mode, cannot read" );

            if ( BaseStream != null )
                throw new InvalidOperationException( "Section has already been read before" );

            BaseStream = source;
            Reader = new EndianBinaryReader( BaseStream, Encoding.UTF8, Endianness.Little, true );

            if ( skipSignature )
            {
                Reader.PushBaseOffset( Reader.Position - 4 );
            }

            else
            {
                Reader.PushBaseOffset();

                string signature = Reader.ReadString( StringBinaryFormat.FixedLength, 4 );

                if ( signature != Signature )
                    throw new InvalidDataException( $"Invalid signature (expected {Signature}, got {signature})" );
            }

            SectionSize = Reader.ReadUInt32();
            DataOffset = Reader.BaseOffset + Reader.ReadUInt32();
            Endianness = Reader.ReadInt32() == 0x18000000 ? Endianness.Big : Endianness.Little;
            int depth = Reader.ReadInt32();
            DataSize = Reader.ReadUInt32();

            if ( SectionSize - DataSize != 0 )
            {
                Reader.SeekBegin( DataOffset + DataSize );
                {
                    while ( Reader.Position < DataOffset + SectionSize )
                    {
                        string subSectionSignature = Reader.ReadString( StringBinaryFormat.FixedLength, 4 );
                        var sectionInfo = SectionRegistry.SectionInfosBySignature[ subSectionSignature ];

                        var section = sectionInfo.Create( SectionMode.Read );
                        {
                            section.Read( source, true );
                            mSections.Add( section );
                        }

                        if ( section is RelocationTableSectionInt64 )
                            AddressSpace = AddressSpace.Int64;

                        else if ( section is EndOfFileSection )
                            break;
                    }
                }
            }
            else
            {
                Reader.SeekBegin( DataOffset + SectionSize );
            }

            if ( AddressSpace == AddressSpace.Int64 )
                Reader.BaseOffset = DataOffset;

            Reader.Endianness = Endianness;
            Reader.AddressSpace = AddressSpace;
        }

        private void Write( Stream destination, int depth )
        {
            if ( Mode == SectionMode.Read )
                throw new InvalidOperationException( "Section is in read mode, cannot write" );

            if ( BaseStream != null )
                throw new InvalidOperationException( "Section has already been written before" );

            BaseStream = destination;

            Writer = IsEnrsWorthWriting()
                ? new EnrsBinaryWriter( BaseStream, Encoding.UTF8, Endianness.Little, true )
                : new EndianBinaryWriter( BaseStream, Encoding.UTF8, Endianness.Little, true );

            long headerOffset = Writer.Position;
            {
                Writer.WriteNulls( 8 * sizeof( uint ) );
            }

            DataOffset = Writer.Position;

            Writer.BaseOffset = AddressSpace == AddressSpace.Int64 ? DataOffset : headerOffset;
            {
                Writer.Endianness = Endianness;
                Writer.AddressSpace = AddressSpace;
                {
                    Writer.PushStringTable( 16, AlignmentMode.Left, StringBinaryFormat.NullTerminated );
                    {
                        Write( mData, Writer );
                    }
                    Writer.PerformScheduledWrites();
                    Writer.PopStringTablesReversed();
                }
                Writer.Endianness = Endianness.Little;
                Writer.AddressSpace = AddressSpace.Int32;
                Writer.Align( 16 );
            }

            DataSize = Writer.Position - DataOffset;

            ProcessData();
            {
                foreach ( var section in mSections.Where( x => !x.SectionInfo.IsBinaryFile ) )
                {
                    section.Endianness = Endianness;
                    section.AddressSpace = AddressSpace;
                    section.Write( BaseStream, depth + 1 );
                }
            }

            SectionSize = Writer.Position - DataOffset;

            Writer.WriteAtOffset( headerOffset, () =>
            {
                Writer.Write( Signature, StringBinaryFormat.FixedLength, 4 );
                Writer.Write( ( uint ) SectionSize );
                Writer.Write( ( uint ) ( DataOffset - headerOffset ) );
                Writer.Write( Endianness == Endianness.Big ? 0x18000000 : 0x10000000 );
                Writer.Write( depth );
                Writer.Write( ( uint ) DataSize );
            } );
        }

        private bool IsRelocationTableWorthWriting()
        {
            return Writer.OffsetPositions.Count > 0 &&
                   !Flags.HasFlag( SectionFlags.HasNoRelocationTable ) &&
                   !( this is EnrsSection ) &&
                   !( this is EndOfFileSection ) &&
                   !( this is RelocationTableSectionInt32 ) &&
                   !( this is RelocationTableSectionInt64 );
        }

        private bool IsEnrsWorthWriting()
        {
            return Endianness == Endianness.Little &&
                   !( this is EnrsSection ) &&
                   !( this is EndOfFileSection ) &&
                   !( this is RelocationTableSectionInt32 ) &&
                   !( this is RelocationTableSectionInt64 );
        }

        protected abstract void Read( T data, EndianBinaryReader reader, long length );
        protected abstract void Write( T data, EndianBinaryWriter writer );

        protected Section( SectionMode mode, T data = default )
        {
            var type = GetType();
            SectionInfo = SectionRegistry.GetOrRegisterSectionInfo( type );

            Mode = mode;
            mData = data;

            if ( Mode == SectionMode.Write && mData == null )
                throw new ArgumentNullException( "Data object must be provided in write mode", nameof( data ) );
        }
    }
}