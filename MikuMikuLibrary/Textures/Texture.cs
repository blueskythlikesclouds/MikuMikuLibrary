using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuLibrary.Textures
{
    public partial class Texture
    {
        private SubTexture[,] subTextures;

        public int ID { get; set; }
        public string Name { get; set; }

        public int Width => subTextures[ 0, 0 ].Width;
        public int Height => subTextures[ 0, 0 ].Height;
        public TextureFormat Format => subTextures[ 0, 0 ].Format;

        public bool IsYCbCr => 
            Format == TextureFormat.ATI2 && Depth == 1 && MipMapCount == 2;

        public int Depth => subTextures.GetLength( 0 );
        public int MipMapCount => subTextures.GetLength( 1 );

        public bool UsesDepth => Depth > 1;
        public bool UsesMipMaps => MipMapCount > 1;

        public SubTexture this[ int level, int mipMapIndex ] => subTextures[ level, mipMapIndex ]; 
        public SubTexture this[ int mipMapIndex ] => subTextures[ 0, mipMapIndex ];

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

            var signature = reader.ReadInt32();
            if ( signature != 0x04505854 && signature != 0x05505854 )
                throw new InvalidDataException( "Invalid signature (expected TXP with type 4 or 5)" );

            int subTextureCount = reader.ReadInt32();
            byte mipMapCount = reader.ReadByte();
            byte depth = reader.ReadByte();
            short rubbish = reader.ReadInt16();

            if ( depth == 1 && mipMapCount != subTextureCount )
                mipMapCount = ( byte )subTextureCount;

            subTextures = new SubTexture[ depth, mipMapCount ];
            for ( int i = 0; i < depth; i++ )
            {
                for ( int j = 0; j < mipMapCount; j++ )
                {
                    reader.ReadAtOffset( reader.ReadUInt32(), () =>
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
            writer.Write( UsesDepth ? 0x05505854 : 0x04505854 );
            writer.Write( MipMapCount * Depth );
            writer.Write( ( byte )MipMapCount );
            writer.Write( ( byte )Depth );
            writer.Write( ( ushort )( 0x0101 ) );
            for ( int i = 0; i < Depth; i++ )
            {
                for ( int j = 0; j < MipMapCount; j++ )
                {
                    var subTexture = subTextures[ i, j ];
                    writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
                    {
                        subTexture.Write( writer );
                    } );
                }
            }
            writer.PopBaseOffset();
        }

        private void Init( int width, int height, TextureFormat format, int depth, int mipMapCount )
        {
            width = Math.Max( 1, width );
            height = Math.Max( 1, height );
            depth = Math.Max( 1, depth );
            mipMapCount = Math.Max( 1, mipMapCount );

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
