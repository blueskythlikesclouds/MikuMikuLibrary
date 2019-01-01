using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Databases
{
    public class TextureEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class TextureDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<TextureEntry> Textures { get; }

        public override void Read( EndianBinaryReader reader, Section section = null )
        {
            int textureCount = reader.ReadInt32();
            long texturesOffset = reader.ReadOffset();

            reader.ReadAtOffset( texturesOffset, () =>
            {
                Textures.Capacity = textureCount;

                for ( int i = 0; i < textureCount; i++ )
                {
                    var textureEntry = new TextureEntry();
                    textureEntry.ID = reader.ReadInt32();
                    textureEntry.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    Textures.Add( textureEntry );
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( Textures.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                int i = Textures.Max( x => x.ID ) + 1;
                foreach ( var textureEntry in Textures )
                {
                    writer.Write( textureEntry.ID );
                    writer.AddStringToStringTable( textureEntry.Name ?? ( i++ ).ToString() );
                }
            } );
        }

        public override void Save( string filePath )
        {
            // Assume it's being exported for F2nd PS3
            if ( BinaryFormatUtilities.IsClassic( Format ) && filePath.EndsWith( ".txi", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.F2nd;
                Endianness = Endianness.BigEndian;
            }

            // Or reverse
            else if ( BinaryFormatUtilities.IsModern( Format ) && filePath.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.DT;
                Endianness = Endianness.LittleEndian;
            }

            base.Save( filePath );
        }

        public TextureEntry GetTexture( string textureName )
        {
            return Textures.FirstOrDefault( x => x.Name.Equals( textureName, StringComparison.OrdinalIgnoreCase ) );
        }

        public TextureEntry GetTexture( int textureID )
        {
            return Textures.FirstOrDefault( x => x.ID.Equals( textureID ) );
        }

        public TextureDatabase()
        {
            Textures = new List<TextureEntry>();
        }
    }
}
