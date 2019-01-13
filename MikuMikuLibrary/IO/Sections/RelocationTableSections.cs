using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "POF0" )]
    public class RelocationTableSectionInt32 : Section<List<long>>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override Endianness Endianness => Endianness.LittleEndian;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        protected override void Read( List<long> dataObject, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( List<long> dataObject, EndianBinaryWriter writer )
        {
            var bytes = new List<byte>();

            dataObject.Sort();

            long currentOffset = 0;
            foreach ( var offset in dataObject )
            {
                long distance = ( offset - currentOffset ) >> 2;

                if ( distance > 0x3FFF )
                {
                    bytes.Add( ( byte )( 0xC0 | ( distance >> 24 ) ) );
                    bytes.Add( ( byte )( distance >> 16 ) );
                    bytes.Add( ( byte )( distance >> 8 ) );
                    bytes.Add( ( byte )( distance & 0xFF ) );
                }
                else if ( distance > 0x3F )
                {
                    bytes.Add( ( byte )( 0x80 | ( distance >> 8 ) ) );
                    bytes.Add( ( byte )( distance ) );
                }
                else if ( distance > 0 )
                {
                    bytes.Add( ( byte )( 0x40 | distance ) );
                }

                currentOffset = offset;
            }

            writer.Write( 4 + AlignmentUtilities.Align( bytes.Count, 4 ) );

            foreach ( var val in bytes )
                writer.Write( val );

            writer.WriteAlignmentPadding( 4 );
        }

        public RelocationTableSectionInt32( SectionMode mode, List<long> dataObject = null ) : base( mode, dataObject )
        {
        }
    }

    [Section( "POF1" )]
    public class RelocationTableSectionInt64 : Section<List<long>>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override Endianness Endianness => Endianness.LittleEndian;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        protected override void Read( List<long> dataObject, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( List<long> dataObject, EndianBinaryWriter writer )
        {
            var bytes = new List<byte>();

            dataObject.Sort();

            long currentOffset = 0;
            foreach ( var offset in dataObject )
            {
                long distance = ( offset - currentOffset ) >> 3;

                if ( distance > 0x3FFF )
                {
                    bytes.Add( ( byte )( 0xC0 | ( distance >> 24 ) ) );
                    bytes.Add( ( byte )( distance >> 16 ) );
                    bytes.Add( ( byte )( distance >> 8 ) );
                    bytes.Add( ( byte )( distance & 0xFF ) );
                }
                else if ( distance > 0x3F )
                {
                    bytes.Add( ( byte )( 0x80 | ( distance >> 8 ) ) );
                    bytes.Add( ( byte )( distance ) );
                }
                else if ( distance > 0 )
                {
                    bytes.Add( ( byte )( 0x40 | distance ) );
                }

                currentOffset = offset;
            }

            writer.Write( 4 + AlignmentUtilities.Align( bytes.Count, 4 ) );

            foreach ( var val in bytes )
                writer.Write( val );

            writer.WriteAlignmentPadding( 8 );
        }

        public RelocationTableSectionInt64( SectionMode mode, List<long> dataObject = null ) : base( mode, dataObject )
        {
        }
    }
}
