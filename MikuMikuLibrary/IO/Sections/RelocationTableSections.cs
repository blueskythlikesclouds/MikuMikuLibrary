using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "POF0" )]
    public class RelocationTableSectionInt32 : Section<List<long>>
    {
        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;
        public override Endianness Endianness => Endianness.Little;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        protected override void Read( List<long> data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( List<long> data, EndianBinaryWriter writer )
        {
            var bytes = new List<byte>();

            data.Sort();

            long currentOffset = 0;

            foreach ( long offset in data )
            {
                long distance = ( offset - currentOffset ) >> 2;

                if ( distance > 0x3FFF )
                {
                    bytes.Add( ( byte ) ( 0xC0 | ( distance >> 24 ) ) );
                    bytes.Add( ( byte ) ( distance >> 16 ) );
                    bytes.Add( ( byte ) ( distance >> 8 ) );
                    bytes.Add( ( byte ) ( distance & 0xFF ) );
                }
                else if ( distance > 0x3F )
                {
                    bytes.Add( ( byte ) ( 0x80 | ( distance >> 8 ) ) );
                    bytes.Add( ( byte ) distance );
                }
                else if ( distance > 0 )
                {
                    bytes.Add( ( byte ) ( 0x40 | distance ) );
                }

                currentOffset = offset;
            }

            writer.Write( 4 + AlignmentHelper.Align( bytes.Count, 4 ) );

            foreach ( byte val in bytes )
                writer.Write( val );

            writer.Align( 4 );
        }

        public RelocationTableSectionInt32( SectionMode mode, List<long> data = null ) : base( mode, data )
        {
        }
    }

    [Section( "POF1" )]
    public class RelocationTableSectionInt64 : Section<List<long>>
    {
        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;
        public override Endianness Endianness => Endianness.Little;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        protected override void Read( List<long> data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( List<long> data, EndianBinaryWriter writer )
        {
            var bytes = new List<byte>();

            data.Sort();

            long currentOffset = 0;

            foreach ( long offset in data )
            {
                long distance = ( offset - currentOffset ) >> 3;

                if ( distance > 0x3FFF )
                {
                    bytes.Add( ( byte ) ( 0xC0 | ( distance >> 24 ) ) );
                    bytes.Add( ( byte ) ( distance >> 16 ) );
                    bytes.Add( ( byte ) ( distance >> 8 ) );
                    bytes.Add( ( byte ) ( distance & 0xFF ) );
                }
                else if ( distance > 0x3F )
                {
                    bytes.Add( ( byte ) ( 0x80 | ( distance >> 8 ) ) );
                    bytes.Add( ( byte ) distance );
                }
                else if ( distance > 0 )
                {
                    bytes.Add( ( byte ) ( 0x40 | distance ) );
                }

                currentOffset = offset;
            }

            writer.Write( 4 + bytes.Count );

            foreach ( byte val in bytes )
                writer.Write( val );

            writer.Align( 8 );
        }

        public RelocationTableSectionInt64( SectionMode mode, List<long> data = null ) : base( mode, data )
        {
        }
    }
}