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
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TextureDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<TextureEntry> Textures { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int textureCount = reader.ReadInt32();
            long texturesOffset = reader.ReadOffset();

            reader.ReadAtOffset( texturesOffset, () =>
            {
                Textures.Capacity = textureCount;

                for ( int i = 0; i < textureCount; i++ )
                {
                    var textureEntry = new TextureEntry();
                    textureEntry.Id = reader.ReadInt32();
                    textureEntry.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    Textures.Add( textureEntry );
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( Textures.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                int i = Textures.Max( x => x.Id ) + 1;
                foreach ( var textureEntry in Textures )
                {
                    writer.Write( textureEntry.Id );
                    writer.AddStringToStringTable( textureEntry.Name ?? i++.ToString() );
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

        public TextureEntry GetTexture( int textureId )
        {
            return Textures.FirstOrDefault( x => x.Id.Equals( textureId ) );
        }

        public TextureDatabase()
        {
            Textures = new List<TextureEntry>();
        }
    }
}
