using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MikuMikuLibrary.Databases
{
    public class SpriteEntry
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public ushort Index { get; set; }
    }

    public class SpriteTextureEntry
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public ushort Index { get; set; }
    }

    public class SpriteSetEntry
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public List<SpriteEntry> Sprites { get; }
        public List<SpriteTextureEntry> Textures { get; }

        public SpriteSetEntry()
        {
            Sprites = new List<SpriteEntry>();
            Textures = new List<SpriteTextureEntry>();
        }
    }

    public class SpriteDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<SpriteSetEntry> SpriteSets { get; }

        public override void Read( EndianBinaryReader reader, Section section = null )
        {
            int spriteSetCount = reader.ReadInt32();
            uint spriteSetsOffset = reader.ReadUInt32();
            int spriteCount = reader.ReadInt32();
            uint spritesOffset = reader.ReadUInt32();

            reader.ReadAtOffset( spriteSetsOffset, () =>
            {
                SpriteSets.Capacity = spriteCount;
                for ( int i = 0; i < spriteSetCount; i++ )
                {
                    ushort id = reader.ReadUInt16();
                    reader.SeekCurrent( 2 );
                    uint nameOffset = reader.ReadUInt32();
                    uint fileNameOffset = reader.ReadUInt32();
                    int index = reader.ReadInt32();
                    Debug.Assert( index == i );
                    long endOffset = reader.Position;

                    var spriteSetEntry = new SpriteSetEntry();
                    spriteSetEntry.ID = id;
                    spriteSetEntry.Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );
                    spriteSetEntry.FileName = reader.ReadStringAtOffset( fileNameOffset, StringBinaryFormat.NullTerminated );
                    SpriteSets.Add( spriteSetEntry );
                }
            } );

            reader.ReadAtOffset( spritesOffset, () =>
            {
                for ( int i = 0; i < spriteCount; i++ )
                {
                    ushort id = reader.ReadUInt16();
                    reader.SeekCurrent( 2 );
                    uint nameOffset = reader.ReadUInt32();
                    ushort index = reader.ReadUInt16();
                    ushort setIndex = reader.ReadUInt16();

                    string name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

                    var set = SpriteSets[ setIndex & 0xFFF ];
                    if ( ( setIndex & 0x1000 ) == 0x1000 )
                    {
                        set.Textures.Add( new SpriteTextureEntry
                        {
                            ID = id,
                            Name = name,
                            Index = index,
                        } );
                    }

                    else
                    {
                        set.Sprites.Add( new SpriteEntry
                        {
                            ID = id,
                            Name = name,
                            Index = index,
                        } );
                    }
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( SpriteSets.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                for ( int i = 0; i < SpriteSets.Count; i++ )
                {
                    var spriteSetEntry = SpriteSets[ i ];
                    writer.Write( spriteSetEntry.ID );
                    writer.WriteNulls( 2 );
                    writer.AddStringToStringTable( spriteSetEntry.Name );
                    writer.AddStringToStringTable( spriteSetEntry.FileName );
                    writer.Write( i );
                }
            } );
            writer.Write( SpriteSets.Sum( x => x.Sprites.Count + x.Textures.Count ) );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                for ( int i = 0; i < SpriteSets.Count; i++ )
                {
                    var spriteSetEntry = SpriteSets[ i ];
                    foreach ( var spriteEntry in spriteSetEntry.Sprites )
                    {
                        writer.Write( spriteEntry.ID );
                        writer.WriteNulls( 2 );
                        writer.AddStringToStringTable( spriteEntry.Name );
                        writer.Write( spriteEntry.Index );
                        writer.Write( ( ushort )i );
                    }

                    foreach ( var textureEntry in spriteSetEntry.Textures )
                    {
                        writer.Write( textureEntry.ID );
                        writer.WriteNulls( 2 );
                        writer.AddStringToStringTable( textureEntry.Name );
                        writer.Write( textureEntry.Index );
                        writer.Write( ( ushort )( i | 0x1000 ) );
                    }
                }
            } );
        }

        public SpriteDatabase()
        {
            SpriteSets = new List<SpriteSetEntry>();
        }
    }
}
