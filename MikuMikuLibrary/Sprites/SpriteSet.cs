using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.Sprites
{
    public class SpriteSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<Sprite> Sprites { get; }
        public TextureSet TextureSet { get; internal set; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int signature = reader.ReadInt32();
            uint texturesOffset = reader.ReadUInt32();
            int textureCount = reader.ReadInt32();
            int spriteCount = reader.ReadInt32();
            long spritesOffset = reader.ReadOffset();
            long textureNamesOffset = reader.ReadOffset();
            long spriteNamesOffset = reader.ReadOffset();
            long spriteModesOffset = reader.ReadOffset();

            reader.ReadAtOffsetIf( section == null, texturesOffset,
                () => { TextureSet.Load( reader.BaseStream, true ); } );

            reader.ReadAtOffset( spritesOffset, () =>
            {
                Sprites.Capacity = spriteCount;

                for ( int i = 0; i < spriteCount; i++ )
                {
                    var sprite = new Sprite();
                    sprite.Read( reader );
                    Sprites.Add( sprite );
                }
            } );

            reader.ReadAtOffsetIf( section?.Endianness != Endianness.Big, textureNamesOffset, () =>
            {
                foreach ( var texture in TextureSet.Textures )
                    texture.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffsetIf( section?.Endianness != Endianness.Big, spriteNamesOffset, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( spriteModesOffset, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.ReadMode( reader );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( 0 );

            if ( section != null )
                writer.WriteNulls( sizeof( uint ) );

            else
                writer.ScheduleWriteOffset( 1, 16, AlignmentMode.Left, () =>
                {
                    TextureSet.Endianness = Endianness;
                    TextureSet.Format = Format;
                    TextureSet.Save( writer.BaseStream, true );
                } );

            writer.Write( TextureSet.Textures.Count );
            writer.Write( Sprites.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.Write( writer );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var texture in TextureSet.Textures )
                    writer.AddStringToStringTable( texture.Name );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    writer.AddStringToStringTable( sprite.Name );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.WriteMode( writer );

                writer.PopStringTablesReversed();
            } );
        }

        public override void Load( string filePath )
        {
            base.Load( filePath );

            if ( !filePath.EndsWith( ".spr", StringComparison.OrdinalIgnoreCase ) )
                return;

            string spriteDatabaseFilePath = Path.ChangeExtension( filePath, "spi" );

            if ( !File.Exists( spriteDatabaseFilePath ) )
                return;

            var spriteDatabase = Load<SpriteDatabase>( spriteDatabaseFilePath );
            var spriteSetInfo = spriteDatabase.SpriteSets[ 0 ];

            foreach ( var spriteInfo in spriteSetInfo.Sprites )
                Sprites[ spriteInfo.Index ].Name = spriteInfo.Name;

            foreach ( var textureInfo in spriteSetInfo.Textures )
                TextureSet.Textures[ textureInfo.Index ].Name = textureInfo.Name;
        }

        public SpriteSet()
        {
            Sprites = new List<Sprite>();
            TextureSet = new TextureSet();
        }
    }
}