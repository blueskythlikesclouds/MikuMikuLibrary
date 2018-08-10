using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;

namespace MikuMikuLibrary.Sprites
{
    public class SpriteSet : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat; }
        }

        public List<Sprite> Sprites { get; }
        public TextureSet TextureSet { get; }

        internal override void Read( EndianBinaryReader reader, Section section = null )
        {
            int signature = reader.ReadInt32();
            uint texturesOffset = reader.ReadUInt32();
            int textureCount = reader.ReadInt32();
            int spriteCount = reader.ReadInt32();
            uint spritesOffset = reader.ReadUInt32();
            uint textureNamesOffset = reader.ReadUInt32();
            uint spriteNamesOffset = reader.ReadUInt32();
            uint spriteUnknownsOffset = reader.ReadUInt32();

            reader.ReadAtOffset( texturesOffset, () => TextureSet.Load( reader.BaseStream ) );

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

        internal override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( 0 );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () => TextureSet.Save( writer.BaseStream ) );
            writer.Write( TextureSet.Textures.Count );
            writer.Write( Sprites.Count );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.WriteFirst( writer );
            } );
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
            writer.PopStringTablesReversed();
            writer.DoEnqueuedOffsetWritesReversed();
        }

        public SpriteSet()
        {
            Sprites = new List<Sprite>();
            TextureSet = new TextureSet();
        }
    }
}
