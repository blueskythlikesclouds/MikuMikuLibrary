using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;

namespace MikuMikuLibrary.Databases
{
    public class TextureEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class TextureDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat; }
        }

        public List<TextureEntry> Textures { get; }

        internal override void Read( EndianBinaryReader reader, Section section = null )
        {
            int textureCount = reader.ReadInt32();
            uint texturesOffset = reader.ReadUInt32();

            reader.ReadAtOffset( texturesOffset, () =>
            {
                Textures.Capacity = textureCount;
                for ( int i = 0; i < textureCount; i++ )
                {
                    var textureEntry = new TextureEntry();
                    textureEntry.ID = reader.ReadInt32();
                    textureEntry.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                    Textures.Add( textureEntry );
                }
            } );
        }

        internal override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( Textures.Count );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var textureEntry in Textures )
                {
                    writer.Write( textureEntry.ID );
                    writer.AddStringToStringTable( textureEntry.Name );
                }
            } );
        }

        public TextureDatabase()
        {
            Textures = new List<TextureEntry>();
        }
    }
}
