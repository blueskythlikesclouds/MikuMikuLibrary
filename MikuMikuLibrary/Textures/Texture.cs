using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public class Texture
    {
        private SubTexture[,] subTextures;

        public int ID { get; set; }
        public string Name { get; set; }

        public int Width
        {
            get { return subTextures[ 0, 0 ].Width; }
        }

        public int Height
        {
            get { return subTextures[ 0, 0 ].Height; }
        }

        public TextureFormat Format
        {
            get { return subTextures[ 0, 0 ].Format; }
        }

        public bool IsYCbCr
        {
            get { return Format == TextureFormat.ATI2 && Depth == 1 && MipMapCount == 2; }
        }

        public int Depth
        {
            get { return subTextures.GetLength( 0 ); }
        }

        public int MipMapCount
        {
            get { return subTextures.GetLength( 1 ); }
        }

        public bool UsesDepth
        {
            get { return Depth > 1; }
        }

        public bool UsesMipMaps
        {
            get { return MipMapCount > 1; }
        }

        public SubTexture this[ int level, int mipMapIndex ]
        {
            get { return subTextures[ level, mipMapIndex ]; }
        }

        public SubTexture this[ int mipMapIndex ]
        {
            get { return subTextures[ 0, mipMapIndex ]; }
        }

        public IEnumerable<SubTexture> EnumerateMipMaps( int level = 0 )
        {
            for ( int i = 0; i < MipMapCount; i++ )
                yield return subTextures[ level, i ];
        }

        public IEnumerable<IEnumerable<SubTexture>> EnumerateLevels()
        {
            for ( int i = 0; i < Depth; i++ )
                yield return EnumerateMipMaps( i );
        }

        internal void Read( EndianBinaryReader reader )
        {
            reader.PushBaseOffset();

            var signature = reader.ReadString( StringBinaryFormat.FixedLength, 3 );
            if ( signature != "TXP" )
                throw new InvalidDataException( "Invalid signature (expected TXP)" );

            byte typeNum = reader.ReadByte();
            if ( typeNum != 4 && typeNum != 5 )
                throw new InvalidDataException( "Invalid type number (expected 4 or 5)" );

            int subTextureCount = reader.ReadInt32();
            byte mipMapCount = reader.ReadByte();
            byte depth = reader.ReadByte();
            short rubbish = reader.ReadInt16();

            subTextures = new SubTexture[ depth, mipMapCount ];
            for ( int i = 0; i < depth; i++ )
            {
                for ( int j = 0; j < mipMapCount; j++ )
                {
                    reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
                    {
                        subTextures[ i, j ] = new SubTexture( reader );
                    } );
                }
            }

            reader.PopBaseOffset();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.PushBaseOffset();
            writer.Write( "TXP", StringBinaryFormat.FixedLength, 3 );
            writer.Write( ( byte )( UsesDepth ? 5 : 4 ) );
            writer.Write( MipMapCount * Depth );
            writer.Write( ( byte )MipMapCount );
            writer.Write( ( byte )Depth );
            writer.Write( ( ushort )( 0x0101 ) );
            for ( int i = 0; i < Depth; i++ )
            {
                for ( int j = 0; j < MipMapCount; j++ )
                {
                    var subTexture = subTextures[ i, j ];
                    writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
                    {
                        subTexture.Write( writer );
                    } );
                }
            }
            writer.PopBaseOffset();
        }

        private void Init( int width, int height, TextureFormat format, int depth, int mipMapCount )
        {
            if ( width < 1 )
                width = 1;

            if ( height < 1 )
                height = 1;

            if ( depth < 1 )
                depth = 1;

            if ( mipMapCount < 1 )
                mipMapCount = 1;

            subTextures = new SubTexture[ depth, mipMapCount ];
            for ( int i = 0; i < depth; i++ )
            {
                for ( int j = 0; j < mipMapCount; j++ )
                {
                    subTextures[ i, j ] = new SubTexture( width >> j, height >> j, format, ( i * mipMapCount ) + j );
                }
            }
        }

        internal Texture( EndianBinaryReader reader )
        {
            Read( reader );
        }

        public Texture( int width, int height, TextureFormat format, int depth = 1, int mipMapCount = 1 )
        {
            Init( width, height, format, depth, mipMapCount );
        }
    }
}
