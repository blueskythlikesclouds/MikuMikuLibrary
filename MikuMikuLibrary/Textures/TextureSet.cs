using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Textures
{
    public class TextureSet : BinaryFile
    {
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public Endianness Endianness { get; set; }

        public List<Texture> Textures { get; }

        protected override void Read( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                reader.PushBaseOffset();

                int signature = reader.ReadInt32();
                if ( signature != 0x03505854 )
                {
                    Endianness = Endianness.BigEndian;
                    signature = EndiannessSwapUtilities.Swap( signature );
                }

                if ( signature != 0x03505854 )
                    throw new InvalidDataException( "Invalid signature (expected TXP with type 3)" );

                int textureCount = reader.ReadInt32();
                int textureCountWithRubbish = reader.ReadInt32();

                Textures.Capacity = textureCount;
                for ( int i = 0; i < textureCount; i++ )
                {
                    reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
                    {
                        Textures.Add( new Texture( reader ) );
                    } );
                }

                reader.PopBaseOffset();
            }
        }

        protected override void Write( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness ) )
            {
                writer.PushBaseOffset();

                writer.Write( 0x03505854 );
                writer.Write( Textures.Count );
                writer.Write( Textures.Count | 0x01010100 );

                foreach ( var texture in Textures )
                {
                    writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
                    {
                        texture.Write( writer );
                    } );
                }

                writer.PopBaseOffset();
            }
        }

        public TextureSet()
        {
            Endianness = Endianness.LittleEndian;
            Textures = new List<Texture>();
        }
    }
}
