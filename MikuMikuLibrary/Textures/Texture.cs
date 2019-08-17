using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Textures
{
    public class Texture
    {
        private SubTexture[ , ] mSubTextures;

        public int Id { get; set; }
        public string Name { get; set; }

        public int Width => mSubTextures[ 0, 0 ].Width;
        public int Height => mSubTextures[ 0, 0 ].Height;
        public TextureFormat Format => mSubTextures[ 0, 0 ].Format;

        public bool IsYCbCr =>
            Format == TextureFormat.ATI2 && Depth == 1 && MipMapCount == 2;

        public int Depth => mSubTextures.GetLength( 0 );
        public int MipMapCount => mSubTextures.GetLength( 1 );

        public bool UsesDepth => Depth > 1;
        public bool UsesMipMaps => MipMapCount > 1;

        public SubTexture this[ int level, int mipMapIndex ] => mSubTextures[ level, mipMapIndex ];
        public SubTexture this[ int mipMapIndex ] => mSubTextures[ 0, mipMapIndex ];

        public IEnumerable<SubTexture> EnumerateMipMaps( int level = 0 )
        {
            for ( int i = 0; i < MipMapCount; i++ )
                yield return mSubTextures[ level, i ];
        }

        public IEnumerable<IEnumerable<SubTexture>> EnumerateLevels()
        {
            for ( int i = 0; i < Depth; i++ )
                yield return EnumerateMipMaps( i );
        }

        internal void Read( EndianBinaryReader reader )
        {
            reader.PushBaseOffset();

            int signature = reader.ReadInt32();
            if ( signature != 0x04505854 && signature != 0x05505854 )
                throw new InvalidDataException( "Invalid signature (expected TXP with type 4 or 5)" );

            int subTextureCount = reader.ReadInt32();
            int info = reader.ReadInt32();

            int mipMapCount = info & 0xFF;
            int depth = ( info >> 8 ) & 0xFF;

            if ( depth == 1 && mipMapCount != subTextureCount )
                mipMapCount = ( byte ) subTextureCount;

            mSubTextures = new SubTexture[ depth, mipMapCount ];
            for ( int i = 0; i < depth; i++ )
            for ( int j = 0; j < mipMapCount; j++ )
                reader.ReadOffset( () => { mSubTextures[ i, j ] = new SubTexture( reader ); } );

            reader.PopBaseOffset();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.PushBaseOffset();
            writer.Write( UsesDepth ? 0x05505854 : 0x04505854 );
            writer.Write( MipMapCount * Depth );
            writer.Write( MipMapCount | ( Depth << 8 ) | 0x01010000 );

            for ( int i = 0; i < Depth; i++ )
            for ( int j = 0; j < MipMapCount; j++ )
            {
                var subTexture = mSubTextures[ i, j ];
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () => { subTexture.Write( writer ); } );
            }

            writer.PopBaseOffset();
        }

        private void Init( int width, int height, TextureFormat format, int depth, int mipMapCount )
        {
            width = Math.Max( 1, width );
            height = Math.Max( 1, height );
            depth = Math.Max( 1, depth );
            mipMapCount = Math.Max( 1, mipMapCount );

            mSubTextures = new SubTexture[ depth, mipMapCount ];
            for ( int i = 0; i < depth; i++ )
            for ( int j = 0; j < mipMapCount; j++ )
                mSubTextures[ i, j ] = new SubTexture( width >> j, height >> j, format, i * mipMapCount + j );
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