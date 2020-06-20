using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    public class SpriteInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public ushort Index { get; set; }
    }

    public class SpriteTextureInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public ushort Index { get; set; }
    }

    public class SpriteSetInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public List<SpriteInfo> Sprites { get; }
        public List<SpriteTextureInfo> Textures { get; }

        public SpriteSetInfo()
        {
            Sprites = new List<SpriteInfo>();
            Textures = new List<SpriteTextureInfo>();
        }
    }

    public class SpriteDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<SpriteSetInfo> SpriteSets { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int spriteSetCount = reader.ReadInt32();
            long spriteSetsOffset = reader.ReadOffset();
            int spriteCount = reader.ReadInt32();
            long spritesOffset = reader.ReadOffset();

            reader.ReadAtOffset( spriteSetsOffset, () =>
            {
                SpriteSets.Capacity = spriteCount;

                for ( int i = 0; i < spriteSetCount; i++ )
                {
                    uint id = reader.ReadUInt32();
                    long nameOffset = reader.ReadOffset();
                    long fileNameOffset = reader.ReadOffset();
                    uint index = reader.ReadUInt32();
                    long endOffset = reader.Position;

                    SpriteSets.Add( new SpriteSetInfo
                    {
                        Id = id,
                        Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated ),
                        FileName = reader.ReadStringAtOffset( fileNameOffset, StringBinaryFormat.NullTerminated )
                    } );
                }
            } );

            reader.ReadAtOffset( spritesOffset, () =>
            {
                for ( int i = 0; i < spriteCount; i++ )
                {
                    uint id = reader.ReadUInt32();
                    long nameOffset = reader.ReadOffset();
                    int info = reader.ReadInt32();

                    if ( section?.Format == BinaryFormat.X )
                        reader.SeekCurrent( 4 );

                    ushort index = ( ushort ) ( info & 0xFFFF );
                    ushort setIndex = ( ushort ) ( ( info >> 16 ) & 0xFFFF );
                    string name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

                    var set = SpriteSets[ setIndex & 0xFFF ];

                    if ( ( setIndex & 0x1000 ) == 0x1000 )
                    {
                        set.Textures.Add( new SpriteTextureInfo
                        {
                            Id = id,
                            Name = name,
                            Index = index
                        } );
                    }

                    else
                    {
                        set.Sprites.Add( new SpriteInfo
                        {
                            Id = id,
                            Name = name,
                            Index = index
                        } );
                    }
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( SpriteSets.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                for ( int i = 0; i < SpriteSets.Count; i++ )
                {
                    var spriteSetInfo = SpriteSets[ i ];
                    writer.Write( spriteSetInfo.Id );
                    writer.AddStringToStringTable( spriteSetInfo.Name );
                    writer.AddStringToStringTable( spriteSetInfo.FileName );
                    writer.Write( i );
                }
            } );
            writer.Write( SpriteSets.Sum( x => x.Sprites.Count + x.Textures.Count ) );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                for ( int i = 0; i < SpriteSets.Count; i++ )
                {
                    var spriteSetInfo = SpriteSets[ i ];

                    foreach ( var spriteInfo in spriteSetInfo.Sprites )
                    {
                        writer.Write( spriteInfo.Id );
                        writer.AddStringToStringTable( spriteInfo.Name );
                        writer.Write( spriteInfo.Index | ( i << 16 ) );

                        if ( section?.Format == BinaryFormat.X )
                            writer.WriteNulls( sizeof( uint ) );
                    }

                    foreach ( var spriteTextureInfo in spriteSetInfo.Textures )
                    {
                        writer.Write( spriteTextureInfo.Id );
                        writer.AddStringToStringTable( spriteTextureInfo.Name );
                        writer.Write( spriteTextureInfo.Index | ( ( i | 0x1000 ) << 16 ) );

                        if ( section?.Format == BinaryFormat.X )
                            writer.WriteNulls( sizeof( uint ) );
                    }
                }
            } );
        }

        public SpriteDatabase()
        {
            SpriteSets = new List<SpriteSetInfo>();
        }
    }
}