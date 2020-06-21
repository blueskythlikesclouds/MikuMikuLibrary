using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLTexture : IDisposable
    {
        private static readonly int[] sCubeMapIndices = { 0, 1, 2, 3, 5, 4 };

        private static readonly IReadOnlyDictionary<TextureFormat, InternalFormat> sInternalFormatMap =
            new Dictionary<TextureFormat, InternalFormat>
            {
                { TextureFormat.A8, InternalFormat.Alpha8 },
                { TextureFormat.RGB8, InternalFormat.Rgb8 },
                { TextureFormat.RGBA8, InternalFormat.Rgba8 },
                { TextureFormat.RGB5, InternalFormat.Rgb5 },
                { TextureFormat.RGB5A1, InternalFormat.Rgb5A1 },
                { TextureFormat.RGBA4, InternalFormat.Rgba4 },
                { TextureFormat.DXT1, InternalFormat.CompressedRgbS3tcDxt1Ext },
                { TextureFormat.DXT1a, InternalFormat.CompressedRgbaS3tcDxt1Ext },
                { TextureFormat.DXT3, InternalFormat.CompressedRgbaS3tcDxt3Ext },
                { TextureFormat.DXT5, InternalFormat.CompressedRgbaS3tcDxt5Ext },
                { TextureFormat.ATI1, InternalFormat.CompressedRedRgtc1 },
                { TextureFormat.ATI2, InternalFormat.CompressedRgRgtc2 },
                { TextureFormat.L8, InternalFormat.Luminance8 },
                { TextureFormat.L8A8, InternalFormat.Luminance8Alpha8 }
            };

        private readonly long mLength;
        private bool mDisposed;

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
            if ( mDisposed )
                return;

            GL.DeleteTexture( Id );
            GL.Finish();

            GC.RemoveMemoryPressure( mLength );

            mDisposed = true;
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

                var format = sInternalFormatMap[ texture.Format ];

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

                var format = sInternalFormatMap[ texture.Format ];

                for ( int i = 0; i < texture.MipMapCount; i++ )
                {
                    GL.CompressedTexImage2D( TextureTarget.Texture2D, i, format, texture[ i ].Width, texture[ i ].Height, 0, texture[ i ].Data.Length,
                        texture[ i ].Data );
                }
            }

            mLength = texture.EnumerateLevels()
                .SelectMany( x => x ).Sum( x => x.Data.Length );

            GC.AddMemoryPressure( mLength );
        }
    }
}