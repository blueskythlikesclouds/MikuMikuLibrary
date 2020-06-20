using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.IO.Sections.Textures;

namespace MikuMikuLibrary.Textures
{
    public class TextureSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<Texture> Textures { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            reader.PushBaseOffset();

            int signature = reader.ReadInt32();

            if ( signature != 0x03505854 )
            {
                reader.Endianness = Endianness = Endianness.Big;
                signature = EndiannessHelper.Swap( signature );
            }

            if ( signature != 0x03505854 )
                throw new InvalidDataException( "Invalid signature (expected TXP with type 3)" );

            int textureCount = reader.ReadInt32();
            int textureCountWithRubbish = reader.ReadInt32();

            Textures.Capacity = textureCount;

            for ( int i = 0; i < textureCount; i++ )
                reader.ReadOffset( () => { Textures.Add( new Texture( reader ) ); } );

            reader.PopBaseOffset();
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.PushBaseOffset();

            writer.Write( 0x03505854 );
            writer.Write( Textures.Count );
            writer.Write( Textures.Count | 0x01010100 );

            foreach ( var texture in Textures )
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () => { texture.Write( writer ); } );

            writer.PopBaseOffset();
        }

        protected override ISection GetSectionInstanceForWriting() => 
            new TextureSetSection( SectionMode.Write, this );

        public override void Load( string filePath )
        {
            base.Load( filePath );

            if ( !filePath.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) ) 
                return;

            var textureDatabase = LoadIfExist<TextureDatabase>( Path.ChangeExtension( filePath, "txi" ) );

            if ( Textures.Count != textureDatabase.Textures.Count ) 
                return;

            for ( int i = 0; i < Textures.Count; i++ )
            {
                Textures[ i ].Id = textureDatabase.Textures[ i ].Id;
                Textures[ i ].Name = textureDatabase.Textures[ i ].Name;
            }
        }

        public override void Save( string filePath )
        {
            // Assume it's being exported for F2nd PS3
            if ( BinaryFormatUtilities.IsClassic( Format ) &&
                 filePath.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.F2nd;
                Endianness = Endianness.Big;
            }

            // Or vice versa
            else if ( BinaryFormatUtilities.IsModern( Format ) &&
                      filePath.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.DT;
                Endianness = Endianness.Little;
            }

            // Save a TXI if we are modern
            if ( filePath.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) )
            {
                var textureDatabase = new TextureDatabase();

                foreach ( var texture in Textures )
                {
                    textureDatabase.Textures.Add( new TextureInfo
                    {
                        Id = texture.Id,
                        Name = texture.Name ?? Guid.NewGuid().ToString()
                    } );
                }

                textureDatabase.Format = Format;
                textureDatabase.Endianness = Endianness;
                textureDatabase.Save( Path.ChangeExtension( filePath, "txi" ) );
            }

            base.Save( filePath );
        }

        public TextureSet()
        {
            Textures = new List<Texture>();
        }
    }
}