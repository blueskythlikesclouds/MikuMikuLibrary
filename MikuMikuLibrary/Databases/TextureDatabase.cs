using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    public class TextureEntry
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class TextureDatabase : BinaryFile
    {
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public List<TextureEntry> Textures { get; }

        protected override void Read( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
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
        }

        protected override void Write( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( Textures.Count );
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var textureEntry in Textures )
                    {
                        writer.Write( textureEntry.ID );
                        writer.AddStringToStringTable( textureEntry.Name );
                    }
                } );
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public TextureDatabase()
        {
            Textures = new List<TextureEntry>();
        }
    }
}
