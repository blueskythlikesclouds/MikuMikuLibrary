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

        public List<Texture> Textures { get; }

        protected override void InternalRead( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                reader.PushBaseOffset();

                var signature = reader.ReadString( StringBinaryFormat.FixedLength, 3 );
                if ( signature != "TXP" )
                    throw new InvalidDataException( "Invalid signature (expected TXP)" );

                byte typeNum = reader.ReadByte();
                if ( typeNum != 3 )
                    throw new InvalidDataException( "Invalid type number (expected 3)" );

                int textureCount = reader.ReadInt32();
                int textureCountWithRubbish = reader.ReadInt32();

                Textures.Capacity = textureCount;
                for ( int i = 0; i < textureCount; i++ )
                {
                    reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
                    {
                        var texture = new Texture();
                        texture.Read( reader );
                        Textures.Add( texture );
                    } );
                }

                reader.PopBaseOffset();
            }
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.PushBaseOffset();

                writer.Write( "TXP", StringBinaryFormat.FixedLength, 3 );
                writer.Write( ( byte )3 );
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
            Textures = new List<Texture>();
        }
    }
}
