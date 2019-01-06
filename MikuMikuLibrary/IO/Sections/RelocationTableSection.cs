using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "POF0", typeof( List<long> ) )]
    public class RelocationTableSectionInt32 : Section<List<long>>
    {
        public override Endianness Endianness => Endianness.LittleEndian;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        public override SectionFlags Flags => SectionFlags.None;

        protected override void Read( EndianBinaryReader reader, long length )
        {
            //long startOffset = reader.Position;
            //uint tableSize = reader.ReadUInt32();

            //long currentOffset = 0;

            //while ( reader.Position < ( startOffset + tableSize ) )
            //{
            //    byte currentByte = reader.ReadByte();

            //    long unpackedDistance = 0;
            //    switch ( ( currentByte >> 6 ) & 3 )
            //    {
            //        case 0:
            //            break;

            //        case 1:
            //            unpackedDistance = currentByte & 0x3F;
            //            break;

            //        case 2:
            //            unpackedDistance = ( ( currentByte << 8 ) & 0x3F ) | reader.ReadByte();
            //            break;

            //        case 3:
            //            unpackedDistance = ( ( currentByte << 24 ) & 0x3F ) | reader.ReadByte() << 16 | reader.ReadByte() << 8 | reader.ReadByte();
            //            break;

            //        default:
            //            throw new Exception( "初音ミクの消失" );
            //    }

            //    unpackedDistance <<= 2;

            //    Data.Add( currentOffset += unpackedDistance );
            //}
        }

        protected override void Write( EndianBinaryWriter writer )
        {
            var bytes = new List<byte>();

            long currentOffset = 0;
            foreach ( var offset in Data )
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

        public RelocationTableSectionInt32( Stream source, List<long> dataToRead = null ) : base( source, dataToRead )
        {
        }

        public RelocationTableSectionInt32( List<long> dataToWrite ) : base( dataToWrite, Endianness.LittleEndian, AddressSpace.Int32 )
        {
        }
    }

    [Section( "POF1", typeof( List<long> ) )]
    public class RelocationTableSectionInt64 : Section<List<long>>
    {
        public override Endianness Endianness => Endianness.LittleEndian;

        public override SectionFlags Flags => SectionFlags.None;

        protected override void Read( EndianBinaryReader reader, long length )
        {
            //long startOffset = reader.Position;
            //uint tableSize = reader.ReadUInt32();

            //long currentOffset = 0;

            //while ( reader.Position < ( startOffset + tableSize ) )
            //{
            //    byte currentByte = reader.ReadByte();

            //    long unpackedDistance = 0;
            //    switch ( ( currentByte >> 6 ) & 3 )
            //    {
            //        case 0:
            //            break;

            //        case 1:
            //            unpackedDistance = currentByte & 0x3F;
            //            break;

            //        case 2:
            //            unpackedDistance = ( ( currentByte << 8 ) & 0x3F ) | reader.ReadByte();
            //            break;

            //        case 3:
            //            unpackedDistance = ( ( currentByte << 24 ) & 0x3F ) | reader.ReadByte() << 16 | reader.ReadByte() << 8 | reader.ReadByte();
            //            break;

            //        default:
            //            throw new Exception( "初音ミクの消失" );
            //    }

            //    // Literally the only difference
            //    unpackedDistance <<= 3;

            //    Data.Add( currentOffset += unpackedDistance );
            //}
        }

        protected override void Write( EndianBinaryWriter writer )
        {
            var bytes = new List<byte>();

            long currentOffset = 0;
            foreach ( var offset in Data )
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

            writer.Write( 4 + AlignmentUtilities.Align( bytes.Count, 8 ) );

            foreach ( var val in bytes )
                writer.Write( val );

            writer.WriteAlignmentPadding( 8 );
        }

        public RelocationTableSectionInt64( Stream source, List<long> dataToRead = null ) : base( source, dataToRead )
        {
        }

        public RelocationTableSectionInt64( List<long> dataToWrite ) : base( dataToWrite, Endianness.LittleEndian, AddressSpace.Int32 )
        {
            dataToWrite.Sort();
        }
    }
}
