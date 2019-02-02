using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class Section<T> : ISection where T : new()
    {
        private readonly List<ISection> mSections = new List<ISection>();
        private T mDataObject;
        private bool mObjectProcessed;
        private bool mProcessingObject;

        public T DataObject
        {
            get
            {
                if ( !mObjectProcessed && !mProcessingObject )
                    ProcessDataObject();

                return mDataObject;
            }
        }

        object ISection.DataObject => DataObject;

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

        public BinaryFormat Format => BinaryFormatUtilities.GetFormat( AddressSpace );

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

        private void Read( Stream source, bool skipSignature )
        {
            if ( Mode == SectionMode.Write )
                throw new InvalidOperationException( "Section is in write mode, cannot read" );

            if ( BaseStream != null )
                throw new InvalidOperationException( "Section has already been read before" );

            BaseStream = source;
            Reader = new EndianBinaryReader( BaseStream, Encoding.UTF8, true, Endianness.LittleEndian );

            if ( skipSignature )
                Reader.PushBaseOffset( Reader.Position - 4 );
            else
            {
                Reader.PushBaseOffset();

                string signature = Reader.ReadString( StringBinaryFormat.FixedLength, 4 );
                if ( signature != Signature )
                    throw new InvalidDataException( $"Invalid signature (expected {Signature}, got {signature})" );
            }

            SectionSize = Reader.ReadUInt32();
            DataOffset = Reader.BaseOffset + Reader.ReadUInt32();
            int endiannessFlag = Reader.ReadInt32();
            Endianness = endiannessFlag == 0x18000000 ? Endianness.BigEndian : Endianness.LittleEndian;
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
                Reader.SeekBegin( DataOffset + SectionSize );

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
            Writer = new EndianBinaryWriter( BaseStream, Encoding.UTF8, true, Endianness.LittleEndian );

            long headerOffset = Writer.Position;
            {
                Writer.WriteNulls( 32 );
            }

            DataOffset = Writer.Position;

            Writer.BaseOffset = AddressSpace == AddressSpace.Int64 ? DataOffset : headerOffset;
            {
                Writer.Endianness = Endianness;
                Writer.AddressSpace = AddressSpace;
                {
                    Writer.PushStringTable( 16, AlignmentMode.Left, StringBinaryFormat.NullTerminated );
                    {
                        Write( mDataObject, Writer );
                    }
                    Writer.PerformScheduledWrites();
                    Writer.PopStringTablesReversed();
                }
                Writer.Endianness = Endianness.LittleEndian;
                Writer.AddressSpace = AddressSpace.Int32;
                Writer.WriteAlignmentPadding( 16 );
            }

            DataSize = Writer.Position - DataOffset;

            ProcessDataObject();
            {
                foreach ( var section in mSections.Where( x => !x.SectionInfo.IsBinaryFileType ) )
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
                Writer.Write( Endianness == Endianness.BigEndian ? 0x18000000 : 0x10000000 );
                Writer.Write( depth );
                Writer.Write( ( uint ) DataSize );
            } );
        }

        public void ProcessDataObject()
        {
            if ( mObjectProcessed )
                return;

            mProcessingObject = true;

            switch ( Mode )
            {
                case SectionMode.Read when BaseStream == null || Reader == null:
                    throw new InvalidOperationException( "Section has not been read yet, cannot process data object" );
                case SectionMode.Read:
                {
                    if ( mDataObject == null )
                        mDataObject = new T();

                    foreach ( var section in mSections )
                        if ( SectionInfo.SubSectionInfos.TryGetValue( section.SectionInfo, out var subSectionInfo ) )
                            subSectionInfo.ProcessPropertyForReading( section, this );

                    Reader.SeekBegin( DataOffset );
                    {
                        Read( mDataObject, Reader, DataSize );
                    }
                    Reader.SeekBegin( DataOffset + SectionSize );
                    break;
                }

                case SectionMode.Write when BaseStream == null || Writer == null:
                    throw new InvalidOperationException(
                        "Data object has not been written yet, cannot process data object" );
                case SectionMode.Write:
                {
                    mSections.Clear();
                    if ( Writer.OffsetPositions.Count > 0 && Flags.HasFlag( SectionFlags.HasRelocationTable ) )
                    {
                        ISection relocationTableSection;

                        var offsets = Writer.OffsetPositions.Select( x => x - Writer.BaseOffset ).ToList();
                        if ( AddressSpace == AddressSpace.Int64 )
                            relocationTableSection = new RelocationTableSectionInt64( SectionMode.Write, offsets );
                        else
                            relocationTableSection = new RelocationTableSectionInt32( SectionMode.Write, offsets );

                        mSections.Add( relocationTableSection );
                    }

                    foreach ( var subSectionInfo in SectionInfo.SubSectionInfos.Values.OrderBy( x => x.Priority ) )
                        mSections.AddRange( subSectionInfo.ProcessPropertyForWriting( this ) );

                    if ( mSections.Count > 0 )
                        mSections.Add( new EndOfFileSection( SectionMode.Write, this ) );
                    break;
                }
            }

            mObjectProcessed = true;
            mProcessingObject = false;
        }

        protected abstract void Read( T dataObject, EndianBinaryReader reader, long length );
        protected abstract void Write( T dataObject, EndianBinaryWriter writer );

        protected Section( SectionMode mode, T dataObject = default( T ) )
        {
            var type = GetType();
            SectionInfo = SectionRegistry.GetOrRegisterSectionInfo( type );

            Mode = mode;
            mDataObject = dataObject;

            if ( Mode == SectionMode.Write && mDataObject == null )
                throw new ArgumentNullException( "Data object must be provided in write mode", nameof( dataObject ) );
        }
    }
}