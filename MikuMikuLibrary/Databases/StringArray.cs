using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;

namespace MikuMikuLibrary.Databases
{
    public class StringArray : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save; }
        }

        public List<string> Strings { get; }

        public override void Read( EndianBinaryReader reader, Section section = null )
        {
            var offsets = new List<long>();

            // Try to determine endianness (apparently DT uses big endian string arrays)
            uint stringOffset = reader.ReadUInt32();
            if ( stringOffset >= reader.BaseStreamLength )
            {
                reader.Endianness = Endianness.BigEndian;
                stringOffset = EndiannessSwapUtilities.Swap( stringOffset );
            }

            Endianness = reader.Endianness;

            do
            {
                offsets.Add( stringOffset );
                stringOffset = reader.ReadUInt32();
            } while ( reader.Position < offsets[ 0 ] && stringOffset != 0 );

            foreach ( var offset in offsets )
            {
                reader.SeekBegin( offset );
                Strings.Add( reader.ReadString( StringBinaryFormat.NullTerminated ) );
            }
        }

        public override void Write( EndianBinaryWriter writer, Section section = null )
        {
            foreach ( var str in Strings )
                writer.AddStringToStringTable( str );
        }

        public StringArray()
        {
            Strings = new List<string>();
        }
    }
}
