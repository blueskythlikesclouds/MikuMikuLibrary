using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    public class AetEntry
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
    }

    public class AetSetEntry
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public ushort SpriteSetID { get; set; }
        public List<AetEntry> Aets { get; }

        public AetSetEntry()
        {
            Aets = new List<AetEntry>();
        }
    }

    public class AetDatabase : BinaryFile
    {
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public List<AetSetEntry> AetSets { get; }

        protected override void InternalRead( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                int aetSetCount = reader.ReadInt32();
                uint aetSetsOffset = reader.ReadUInt32();
                int aetCount = reader.ReadInt32();
                uint aetsOffset = reader.ReadUInt32();

                reader.ReadAtOffset( aetSetsOffset, () =>
                {
                    AetSets.Capacity = aetSetCount;
                    for ( int i = 0; i < aetSetCount; i++ )
                    {
                        ushort id = reader.ReadUInt16();
                        reader.SeekCurrent( 2 );

                        string name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                        string fileName = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );

                        ushort index = reader.ReadUInt16();
                        reader.SeekCurrent( 2 );
                        Debug.Assert( index == i );

                        ushort spriteSetID = reader.ReadUInt16();
                        reader.SeekCurrent( 2 );

                        AetSets.Add( new AetSetEntry
                        {
                            ID = id,
                            Name = name,
                            FileName = fileName,
                            SpriteSetID = spriteSetID,
                        } );
                    }
                } );

                reader.ReadAtOffset( aetsOffset, () =>
                {
                    for ( int i = 0; i < aetCount; i++ )
                    {
                        ushort id = reader.ReadUInt16();
                        reader.SeekCurrent( 2 );

                        string name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );

                        ushort index = reader.ReadUInt16();
                        ushort setIndex = reader.ReadUInt16();

                        var aetEntry = new AetEntry
                        {
                            ID = id,
                            Name = name,
                        };

                        var aetSetEntry = AetSets[ setIndex ];
                        if ( index > aetSetEntry.Aets.Count )
                            aetSetEntry.Aets.Add( aetEntry );
                        else
                            aetSetEntry.Aets.Insert( index, aetEntry );
                    }
                } );
            }
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( AetSets.Count );
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    for ( int i = 0; i < AetSets.Count; i++ )
                    {
                        var aetSetEntry = AetSets[ i ];
                        writer.Write( aetSetEntry.ID );
                        writer.WriteNulls( 2 );
                        writer.AddStringToStringTable( aetSetEntry.Name );
                        writer.AddStringToStringTable( aetSetEntry.FileName );
                        writer.Write( ( ushort )i );
                        writer.WriteNulls( 2 );
                        writer.Write( aetSetEntry.SpriteSetID );
                        writer.WriteNulls( 2 );
                    }
                } );
                writer.Write( AetSets.Sum( x => x.Aets.Count ) );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    for ( int i = 0; i < AetSets.Count; i++ )
                    {
                        var aetSetEntry = AetSets[ i ];
                        for ( int j = 0; j < aetSetEntry.Aets.Count; j++ )
                        {
                            var aetEntry = aetSetEntry.Aets[ j ];
                            writer.Write( aetEntry.ID );
                            writer.WriteNulls( 2 );
                            writer.AddStringToStringTable( aetEntry.Name );
                            writer.Write( ( ushort )j );
                            writer.Write( ( ushort )i );
                        }
                    }
                } );
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public AetDatabase()
        {
            AetSets = new List<AetSetEntry>();
        }
    }
}
