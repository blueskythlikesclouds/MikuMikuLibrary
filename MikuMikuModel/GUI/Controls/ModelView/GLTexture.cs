using System;
using System.Linq;
using MikuMikuLibrary.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLTexture : IDisposable
    {
        private static readonly int[] sCubeMapIndices = { 0, 1, 2, 3, 5, 4 };

        private readonly long mLength;

        public int Id { get; }
        public TextureTarget Target { get; }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public void Bind()
        {
            GL.BindTexture( Target, Id );
        }

        protected void Dispose( bool disposing )
        {
            GL.DeleteTexture( Id );
            GC.RemoveMemoryPressure( mLength );
        }

        ~GLTexture()
        {
            Dispose( false );
        }

        public GLTexture( Texture texture )
        {
            Id = GL.GenTexture();
            if ( texture.UsesArraySize )
            {
                Target = TextureTarget.TextureCubeMap;
                GL.BindTexture( TextureTarget.TextureCubeMap, Id );

                GL.TexParameter( Target, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.ClampToEdge );
                GL.TexParameter( Target, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.ClampToEdge );
                GL.TexParameter( Target, TextureParameterName.TextureWrapR, ( int ) TextureWrapMode.ClampToEdge );
                GL.TexParameter( Target, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );
                GL.TexParameter( Target, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.Linear );
                GL.TexParameter( Target, TextureParameterName.TextureMaxLevel, texture.MipMapCount - 1 );

                var format = GetGLInternalFormat( texture.Format );

                for ( int i = 0; i < sCubeMapIndices.Length; i++ )
                for ( int j = 0; j < texture.MipMapCount; j++ )
                {
                    GL.CompressedTexImage2D( TextureTarget.TextureCubeMapPositiveX + sCubeMapIndices[ i ], j, format, texture[ i, j ].Width,
                        texture[ i, j ].Height, 0, texture[ i, j ].Data.Length, texture[ i, j ].Data );
                }
            }

            else
            {
                Target = TextureTarget.Texture2D;
                GL.BindTexture( TextureTarget.Texture2D, Id );

                GL.TexParameter( Target, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.Repeat );
                GL.TexParameter( Target, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.Repeat );
                GL.TexParameter( Target, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );
                GL.TexParameter( Target, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.LinearMipmapLinear );
                GL.TexParameter( Target, TextureParameterName.TextureMaxLevel, texture.MipMapCount - 1 );

                var format = GetGLInternalFormat( texture.Format );

                for ( int i = 0; i < texture.MipMapCount; i++ )
                {
                    GL.CompressedTexImage2D( TextureTarget.Texture2D, i, format, texture[ i ].Width, texture[ i ].Height, 0, texture[ i ].Data.Length,
                        texture[ i ].Data );
                }
            }

            mLength = texture.EnumerateLevels()
                .SelectMany( x => x ).Sum( x => x.Data.Length );

            GC.AddMemoryPressure( mLength );

            InternalFormat GetGLInternalFormat( TextureFormat textureFormat )
            {
                switch ( textureFormat )
                {
                    case TextureFormat.RGB:
                        return InternalFormat.Rgb;

                    case TextureFormat.RGBA:
                        return InternalFormat.Rgba;

                    case TextureFormat.DXT1:
                        return InternalFormat.CompressedRgbaS3tcDxt1Ext;

                    case TextureFormat.DXT3:
                        return InternalFormat.CompressedRgbaS3tcDxt3Ext;

                    case TextureFormat.DXT5:
                        return InternalFormat.CompressedRgbaS3tcDxt5Ext;

                    case TextureFormat.ATI1:
                        return InternalFormat.CompressedRedRgtc1;

                    case TextureFormat.ATI2:
                        return InternalFormat.CompressedRgRgtc2;
                }

                throw new ArgumentException( nameof( textureFormat ) );
            }
        }
    }
}