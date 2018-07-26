using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    public class StringArray : BinaryFile
    {
        public List<string> Strings { get; }

        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public Endianness Endianness { get; set; }

        protected override void InternalRead( Stream source )
        {
            var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian );

            var offsets = new List<long>();

            // Try to determine endianness (apparently DT uses big endian string arrays)
            uint stringOffset = reader.ReadUInt32();
            if ( stringOffset >= source.Length )
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

            reader.Close();
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness ) )
            {
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );

                foreach ( var str in Strings )
                    writer.AddStringToStringTable( str );

                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public StringArray()
        {
            Strings = new List<string>();
            Endianness = Endianness.LittleEndian;
        }
    }
}
