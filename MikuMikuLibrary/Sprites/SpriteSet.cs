using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Sprites
{
    public class SpriteSet : BinaryFile
    {
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public List<Sprite> Sprites { get; }
        public TextureSet TextureSet { get; }

        protected override void InternalRead( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                int signature = reader.ReadInt32();
                uint texturesOffset = reader.ReadUInt32();
                int textureCount = reader.ReadInt32();
                int spriteCount = reader.ReadInt32();
                uint spritesOffset = reader.ReadUInt32();
                uint textureNamesOffset = reader.ReadUInt32();
                uint spriteNamesOffset = reader.ReadUInt32();
                uint spriteUnknownsOffset = reader.ReadUInt32();

                reader.ReadAtOffset( texturesOffset, () => TextureSet.Load( source, true ) );

                Sprites.Capacity = spriteCount;
                reader.ReadAtOffset( spritesOffset, () =>
                {
                    for ( int i = 0; i < spriteCount; i++ )
                    {
                        var sprite = new Sprite();
                        sprite.ReadFirst( reader );
                        Sprites.Add( sprite );
                    }
                } );

                reader.ReadAtOffset( textureNamesOffset, () =>
                {
                    foreach ( var texture in TextureSet.Textures )
                        texture.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                } );

                reader.ReadAtOffset( spriteNamesOffset, () =>
                {
                    foreach ( var sprite in Sprites )
                        sprite.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                } );

                reader.ReadAtOffset( spriteUnknownsOffset, () =>
                {
                    foreach ( var sprite in Sprites )
                        sprite.ReadSecondary( reader );
                } );
            }
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( 0 );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () => TextureSet.Save( destination, true ) );
                writer.Write( TextureSet.Textures.Count );
                writer.Write( Sprites.Count );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var sprite in Sprites )
                        sprite.WriteFirst( writer );
                } );
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var texture in TextureSet.Textures )
                        writer.AddStringToStringTable( texture.Name );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var sprite in Sprites )
                        writer.AddStringToStringTable( sprite.Name );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var sprite in Sprites )
                        sprite.WriteSecondary( writer );
                } );
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public SpriteSet()
        {
            Sprites = new List<Sprite>();
            TextureSet = new TextureSet();
        }
    }
}
