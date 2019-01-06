using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MikuMikuLibrary.IO.Sections
{
    public abstract class Section
    {
        protected readonly List<Section> mSections = new List<Section>();

        public SectionInfo SectionInfo { get; }

        public abstract SectionFlags Flags { get; }

        public object Data { get; }

        public virtual Endianness Endianness { get; protected set; }
        public virtual AddressSpace AddressSpace { get; protected set; }

        public BinaryFormat Format => AddressSpace == AddressSpace.Int64 ? BinaryFormat.X : BinaryFormat.F2nd;

        public Section Parent { get; protected set; }

        public int Depth => ( Parent != null ) ? ( Parent.Depth + 1 ) : 0;

        public IEnumerable<Section> EnumerateSections()
        {
            return mSections;
        }

        public IEnumerable<Section> EnumerateSections( string sig )
        {
            return mSections.Where( x => x.SectionInfo.Signature == sig );
        }

        public IEnumerable<T> EnumerateSections<T>() where T : Section
        {
            return mSections.Where( x => x is T ).Cast<T>();
        }

        public void Add( Section section )
        {
            section.Remove();
            section.Parent = this;
            mSections.Add( section );
        }

        public void Insert( int index, Section section )
        {
            section.Remove();
            section.Parent = this;
            mSections.Insert( index, section );
        }

        public void Remove()
        {
            if ( Parent != null )
                Parent.Remove( this );
        }

        public void Remove( Section section )
        {
            if ( mSections.Contains( section ) )
            {
                section.Parent = null;
                mSections.Remove( section );
            }
        }

        private void ReadSubSections( EndianBinaryReader reader, long endPosition )
        {
            while ( reader.Position < endPosition )
            {
                reader.PushOffset();

                Section subSection;
                var subSectionSignature = reader.ReadString( StringBinaryFormat.FixedLength, 4 );

                // Skip the section if it couldn't be detected.
                if ( !SectionManager.SectionInfosBySignature.TryGetValue( subSectionSignature, out SectionInfo sectionInfo ) )
                {
                    Debug.WriteLine( $"WARNING: Unknown section signature {subSectionSignature}" );

                    uint subSectionSize = reader.ReadUInt32();
                    uint subSectionDataOffset = reader.ReadUInt32();

                    reader.SeekBegin( reader.PopOffset() + subSectionDataOffset + subSectionSize );
                    continue;
                }

                reader.SeekBeginToPoppedOffset();

                if ( SectionInfo.SubSectionInfos.TryGetValue( subSectionSignature, out SubSectionInfo subSectionInfo ) )
                    subSection = subSectionInfo.SetFromSection( this, reader.BaseStream );

                else
                    subSection = sectionInfo.Read( reader.BaseStream );

                Add( subSection );

                if ( subSection is EndOfFileSection )
                    break;

                else if ( subSection is RelocationTableSectionInt32 )
                    AddressSpace = AddressSpace.Int32;

                else if ( subSection is RelocationTableSectionInt64 )
                    AddressSpace = AddressSpace.Int64;
            }
        }

        public void Read( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                reader.PushBaseOffset();

                var signature = reader.ReadString( StringBinaryFormat.FixedLength, 4 );
                if ( signature != SectionInfo.Signature )
                    throw new InvalidDataException( $"Invalid signature (expected {SectionInfo.Signature}, got {signature}" );

                uint sectionSize = reader.ReadUInt32();
                uint dataOffset = reader.ReadUInt32();
                int endiannessFlag = reader.ReadInt32();
                Endianness = ( endiannessFlag == 0x18000000 ) ? Endianness.BigEndian : Endianness.LittleEndian;
                int depth = reader.ReadInt32();
                uint dataSize = reader.ReadUInt32();

                long endOffset = reader.BaseOffset + dataOffset + sectionSize;

                // Read the sections first because if the Read methods want to access child sections
                // this will let them do so
                reader.ReadAtOffsetIf( ( sectionSize - dataSize ) != 0, dataOffset + dataSize, () =>
                {
                    ReadSubSections( reader, endOffset );
                } );

                // Read the sibling sections
                // This is an ugly hack, I'll implement this better
                // when I get to reconstruct the classes.
                reader.ReadAtOffsetIf( depth == 0, endOffset, () =>
                {
                    ReadSubSections( reader, reader.Length );
                } );

                reader.ReadAtOffset( dataOffset, () =>
                {
                    reader.Endianness = Endianness;
                    reader.AddressSpace = AddressSpace;

                    if ( AddressSpace == AddressSpace.Int64 )
                        reader.PushBaseOffset();

                    Read( reader, dataSize );
                } );

                reader.SeekBegin( endOffset );
            }
        }

        protected void Write( Stream destination, bool putEndOfFileSection )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.PushBaseOffset();

                // We will fill the header later
                long headerOffset = writer.Position;
                writer.WriteNulls( 0x20 );

                long dataOffset = writer.Position;
                {
                    if ( AddressSpace == AddressSpace.Int64 )
                        writer.PushBaseOffset();

                    writer.Endianness = Endianness;
                    {
                        writer.PushStringTable( 16, AlignmentMode.Center, StringBinaryFormat.NullTerminated );

                        Write( writer );

                        writer.DoScheduledWriteOffsets();
                        writer.PopStringTablesReversed();
                        writer.WriteAlignmentPadding( 16 );
                    }
                    writer.Endianness = Endianness.LittleEndian;
                }

                long sectionsStartOffset = writer.Position;
                {
                    mSections.Clear();

                    // Push enrs section
                    if ( Flags.HasFlag( SectionFlags.EnrsSection ) )
                        Insert( 0, new EnrsSection( this ) );

                    // Push a relocation table if section makes use of them
                    if ( Flags.HasFlag( SectionFlags.RelocationTableSection ) )
                    {
                        var positions = writer.OffsetPositions
                            .Select( x => x - writer.PeekBaseOffset() ).Distinct().OrderBy( x => x ).ToList();

                        if ( AddressSpace == AddressSpace.Int32 )
                            Insert( 0, new RelocationTableSectionInt32( positions ) );
                        else if ( AddressSpace == AddressSpace.Int64 )
                            Insert( 0, new RelocationTableSectionInt64( positions ) );
                        else
                            throw new ArgumentException( nameof( AddressSpace ) );
                    }

                    // Create and push the subsections
                    foreach ( var subSectionInfo in SectionInfo.SubSectionInfos.Values.OrderBy( x => x.Order ) )
                    {
                        if ( subSectionInfo.IsList )
                        {
                            foreach ( var section in subSectionInfo.GetSections( this, Endianness ) )
                                Add( section );
                        }
                        else
                        {
                            Add( subSectionInfo.GetSection( this, Endianness ) );
                        }
                    }

                    // Push an end of file section if there are any sections
                    if ( mSections.Count > 0 )
                        Add( new EndOfFileSection( this ) );

                    // Write the sections with an ugly hack
                    foreach ( var section in mSections )
                    {
                        if ( !section.SectionInfo.IsBinaryFile )
                            section.Write( destination );
                    }
                }
                long sectionEndOffset = writer.Position;

                // Now we can fill the header
                writer.WriteAtOffset( headerOffset, () =>
                {
                    writer.Write( SectionInfo.Signature, StringBinaryFormat.FixedLength, 4 );
                    writer.Write( ( uint )( sectionEndOffset - dataOffset ) );
                    writer.Write( ( uint )( dataOffset - writer.PeekBaseOffset() ) );
                    writer.Write( Endianness == Endianness.LittleEndian ? 0x10000000 : 0x18000000 );
                    writer.Write( Depth );
                    writer.Write( ( uint )( sectionsStartOffset - dataOffset ) );
                } );
            }

            // Write the siblings
            // This is a pretty ugly hack, but I'll reconstruct it at another time
            if ( Parent == null && !( this is EndOfFileSection ) )
            {
                foreach ( var section in mSections )
                {
                    if ( section.SectionInfo.IsBinaryFile )
                    {
                        section.Parent = null;
                        section.Write( destination, false );
                        section.Parent = this;
                    }
                }

                if ( putEndOfFileSection )
                {
                    var endOfFileSection = new EndOfFileSection( this );
                    endOfFileSection.Write( destination );
                }
            }
        }

        public void Write( Stream destination ) => Write( destination, true );

        protected abstract void Read( EndianBinaryReader reader, long length );

        protected abstract void Write( EndianBinaryWriter writer );

        public Section( Stream source, object dataToRead = null )
        {
            // Try calling a default constructor if the data is null
            SectionInfo = SectionManager.GetOrRegister( GetType() );
            Data = dataToRead ?? Activator.CreateInstance( SectionInfo.DataType );
            Read( source );
        }

        public Section( object dataToWrite, Endianness endianness )
        {
            SectionInfo = SectionManager.GetOrRegister( GetType() );
            Endianness = endianness;
            Data = dataToWrite ?? throw new ArgumentNullException( nameof( dataToWrite ) );
        }
    }

    [Flags]
    public enum SectionFlags
    {
        None = 0,
        RelocationTableSection = 1,
        EnrsSection = 2,
    };
}
