using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public class TextureSet : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat; }
        }

        public List<Texture> Textures { get; }

        internal override void Read( EndianBinaryReader reader, Section section = null )
        {
            reader.PushBaseOffset();

            int signature = reader.ReadInt32();
            if ( signature != 0x03505854 )
            {
                reader.Endianness = Endianness = Endianness.BigEndian;
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

        internal override void Write( EndianBinaryWriter writer, Section section = null )
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

        public TextureSet()
        {
            Textures = new List<Texture>();
        }
    }
}
