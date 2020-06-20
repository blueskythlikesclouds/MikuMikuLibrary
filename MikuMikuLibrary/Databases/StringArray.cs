using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    public class StringEntry
    {
        public string Value { get; set; }
        public uint Id { get; set; }
    }

    public class StringArray : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<StringEntry> Strings { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            if ( section != null )
                ReadModern();
            else
                ReadClassic();

            void ReadClassic()
            {
                var offsets = new List<long>();

                // Try to determine endianness (apparently DT uses big endian string arrays)
                uint stringOffset = reader.ReadUInt32();

                if ( stringOffset >= reader.Length )
                {
                    reader.Endianness = Endianness.Big;
                    stringOffset = EndiannessHelper.Swap( stringOffset );
                }

                Endianness = reader.Endianness;

                do
                {
                    offsets.Add( stringOffset );
                    stringOffset = reader.ReadUInt32();
                } while ( reader.Position < offsets[ 0 ] && stringOffset != 0 );

                Strings.Capacity = offsets.Count;

                for ( int i = 0; i < offsets.Count; i++ )
                {
                    long offset = offsets[ i ];
                    reader.SeekBegin( offset );

                    string value = reader.ReadString( StringBinaryFormat.NullTerminated );
                    if ( !string.IsNullOrEmpty( value ) )
                        Strings.Add( new StringEntry { Value = value, Id = ( uint ) i } );
                }
            }

            void ReadModern()
            {
                int count = reader.ReadInt32();

                reader.ReadOffset( () =>
                {
                    Strings.Capacity = count;

                    for ( int i = 0; i < count; i++ )
                    {
                        Strings.Add( new StringEntry
                        {
                            Value = reader.ReadStringOffset( StringBinaryFormat.NullTerminated ),
                            Id = reader.ReadUInt32()
                        } );
                    }
                } );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
                WriteModern();
            else
                WriteClassic();

            void WriteClassic()
            {
                uint previousId = 0;

                foreach ( var stringEntry in Strings.OrderBy( x => x.Id ) )
                {
                    for ( int i = 0; i < stringEntry.Id - previousId - 1; i++ )
                        writer.AddStringToStringTable( string.Empty );

                    previousId = stringEntry.Id;

                    writer.AddStringToStringTable( stringEntry.Value );
                }
            }

            void WriteModern()
            {
                writer.Write( Strings.Count );
                writer.ScheduleWriteOffset( 64, AlignmentMode.Left, () =>
                {
                    foreach ( var stringEntry in Strings )
                    {
                        writer.AddStringToStringTable( stringEntry.Value );
                        writer.Write( stringEntry.Id );
                    }
                } );
            }
        }

        public StringArray()
        {
            Strings = new List<StringEntry>();
        }
    }
}