using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Databases
{
    public class SpriteEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class SpriteTextureEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class SpriteSetEntry
    {
        public int Id { get; set; }
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
                    int id = reader.ReadInt32();
                    long nameOffset = reader.ReadOffset();
                    long fileNameOffset = reader.ReadOffset();
                    int index = reader.ReadInt32();
                    long endOffset = reader.Position;

                    SpriteSets.Add( new SpriteSetEntry
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
                    int id = reader.ReadInt32();
                    long nameOffset = reader.ReadOffset();
                    int info = reader.ReadInt32();

                    if ( section?.Format == BinaryFormat.X )
                        reader.SeekCurrent( 4 );

                    int index = ( ushort ) ( info & 0xFFFF );
                    int setIndex = ( ushort ) ( ( info >> 16 ) & 0xFFFF );
                    string name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

                    var set = SpriteSets[ setIndex & 0xFFF ];
                    if ( ( setIndex & 0x1000 ) == 0x1000 )
                    {
                        set.Textures.Add( new SpriteTextureEntry
                        {
                            Id = id,
                            Name = name,
                            Index = index,
                        } );
                    }

                    else
                    {
                        set.Sprites.Add( new SpriteEntry
                        {
                            Id = id,
                            Name = name,
                            Index = index,
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
                    var spriteSetEntry = SpriteSets[ i ];
                    writer.Write( spriteSetEntry.Id );
                    writer.AddStringToStringTable( spriteSetEntry.Name );
                    writer.AddStringToStringTable( spriteSetEntry.FileName );
                    writer.Write( i );
                }
            } );
            writer.Write( SpriteSets.Sum( x => x.Sprites.Count + x.Textures.Count ) );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                for ( int i = 0; i < SpriteSets.Count; i++ )
                {
                    var spriteSetEntry = SpriteSets[ i ];
                    foreach ( var spriteEntry in spriteSetEntry.Sprites )
                    {
                        writer.Write( spriteEntry.Id );
                        writer.AddStringToStringTable( spriteEntry.Name );
                        writer.Write( spriteEntry.Index | i << 16 );

                        if ( section?.Format == BinaryFormat.X )
                            writer.WriteNulls( 4 );
                    }

                    foreach ( var textureEntry in spriteSetEntry.Textures )
                    {
                        writer.Write( textureEntry.Id );
                        writer.AddStringToStringTable( textureEntry.Name );
                        writer.Write( textureEntry.Index | ( i | 0x1000 ) << 16 );

                        if ( section?.Format == BinaryFormat.X )
                            writer.WriteNulls( 4 );
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
