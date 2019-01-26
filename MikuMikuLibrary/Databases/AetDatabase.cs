using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MikuMikuLibrary.Databases
{
    public class AetEntry
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public ushort Index { get; set; }
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
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<AetSetEntry> AetSets { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
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

                    string name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    string fileName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

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

                    string name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

                    ushort index = reader.ReadUInt16();
                    ushort setIndex = reader.ReadUInt16();

                    var aetEntry = new AetEntry
                    {
                        ID = id,
                        Name = name,
                        Index = index,
                    };

                    AetSets[ setIndex ].Aets.Add( aetEntry );
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( AetSets.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
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
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                for ( int i = 0; i < AetSets.Count; i++ )
                {
                    var aetSetEntry = AetSets[ i ];
                    foreach ( var aetEntry in aetSetEntry.Aets )
                    {
                        writer.Write( aetEntry.ID );
                        writer.WriteNulls( 2 );
                        writer.AddStringToStringTable( aetEntry.Name );
                        writer.Write( aetEntry.Index );
                        writer.Write( ( ushort )i );
                    }
                }
            } );
        }

        public AetDatabase()
        {
            AetSets = new List<AetSetEntry>();
        }
    }
}
