using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    public class AetInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public ushort Index { get; set; }
    }

    public class AetSetInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public uint SpriteSetId { get; set; }
        public List<AetInfo> Aets { get; }

        public AetSetInfo()
        {
            Aets = new List<AetInfo>();
        }
    }

    public class AetDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<AetSetInfo> AetSets { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int aetSetCount = reader.ReadInt32();
            long aetSetsOffset = reader.ReadOffset();
            int aetCount = reader.ReadInt32();
            long aetsOffset = reader.ReadOffset();

            reader.ReadAtOffset( aetSetsOffset, () =>
            {
                AetSets.Capacity = aetSetCount;

                for ( int i = 0; i < aetSetCount; i++ )
                {
                    uint id = reader.ReadUInt32();
                    string name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    string fileName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    int index = reader.ReadInt32();
                    uint spriteSetId = reader.ReadUInt32();

                    AetSets.Add( new AetSetInfo
                    {
                        Id = id,
                        Name = name,
                        FileName = fileName,
                        SpriteSetId = spriteSetId
                    } );
                }
            } );

            reader.ReadAtOffset( aetsOffset, () =>
            {
                for ( int i = 0; i < aetCount; i++ )
                {
                    uint id = reader.ReadUInt32();
                    string name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

                    int info = reader.ReadInt32();
                    ushort index = ( ushort ) ( info & 0xFFFF );
                    ushort setIndex = ( ushort ) ( ( info >> 16 ) & 0xFFFF );

                    var aetInfo = new AetInfo
                    {
                        Id = id,
                        Name = name,
                        Index = index
                    };

                    AetSets[ setIndex ].Aets.Add( aetInfo );
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
                    var aetSetInfo = AetSets[ i ];
                    writer.Write( aetSetInfo.Id );
                    writer.AddStringToStringTable( aetSetInfo.Name );
                    writer.AddStringToStringTable( aetSetInfo.FileName );
                    writer.Write( i );
                    writer.Write( aetSetInfo.SpriteSetId );
                }
            } );
            writer.Write( AetSets.Sum( x => x.Aets.Count ) );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                for ( int i = 0; i < AetSets.Count; i++ )
                {
                    var aetSetInfo = AetSets[ i ];

                    foreach ( var aetInfo in aetSetInfo.Aets )
                    {
                        writer.Write( aetInfo.Id );
                        writer.AddStringToStringTable( aetInfo.Name );
                        writer.Write( ( i << 16 ) | aetInfo.Index );
                    }
                }
            } );
        }

        public AetDatabase()
        {
            AetSets = new List<AetSetInfo>();
        }
    }
}