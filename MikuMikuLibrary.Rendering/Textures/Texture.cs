using System;
using System.Collections.Generic;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Textures;
using OpenTK.Graphics.OpenGL;
using MMLTexture = MikuMikuLibrary.Textures.Texture;

namespace MikuMikuLibrary.Rendering.Textures
{
    public sealed class Texture : IDisposable
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
                { TextureFormat.L8A8, InternalFormat.Luminance8Alpha8 },
            };

        private static readonly IReadOnlyDictionary<TextureFormat, PixelFormat> sPixelFormatMap =
            new Dictionary<TextureFormat, PixelFormat>
            {
                { TextureFormat.A8, PixelFormat.Alpha },
                { TextureFormat.RGB8 , PixelFormat.Rgb },
                { TextureFormat.RGBA8 , PixelFormat.Rgba },
                { TextureFormat.RGB5, PixelFormat.Rgb },
                { TextureFormat.RGB5A1, PixelFormat.Rgba },
                { TextureFormat.RGBA4, PixelFormat.Rgba },
                { TextureFormat.L8, PixelFormat.Luminance },
                { TextureFormat.L8A8, PixelFormat.LuminanceAlpha }
            };       
        
        private static readonly IReadOnlyDictionary<TextureFormat, PixelType> sPixelTypeMap =
            new Dictionary<TextureFormat, PixelType>
            {
                { TextureFormat.A8, PixelType.UnsignedByte },
                { TextureFormat.RGB8 , PixelType.UnsignedByte },
                { TextureFormat.RGBA8 , PixelType.UnsignedByte },
                { TextureFormat.RGB5, PixelType.UnsignedShort5551 },
                { TextureFormat.RGB5A1, PixelType.UnsignedShort5551 },
                { TextureFormat.RGBA4, PixelType.UnsignedShort4444 },
                { TextureFormat.L8, PixelType.UnsignedByte },
                { TextureFormat.L8A8, PixelType.UnsignedByte }
            };

        private bool mDisposed;

        public int Id { get; }
        public TextureTarget Target { get; }

        public int Width { get; }
        public int Height { get; }
        
        // Test
        public bool IsYCbCr { get; }

        public void Bind( State state )
        {
            state.BindTexture( Target, Id );
        }

        public void Bind( State state, int unit )
        {
            state.BindTexture( Target, Id, unit );
        }

        public void Bind( State state, Shader shader, string uniformName, int unit )
        {
            Bind( state, unit );
            shader.SetUniform( uniformName, unit );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            GL.DeleteTexture( Id );
            GC.SuppressFinalize( this );
        }

        public Texture( State state, MMLTexture texture )
        {
            Id = GL.GenTexture();
            Target = texture.UsesArraySize ? TextureTarget.TextureCubeMap : TextureTarget.Texture2D;

            Width = texture.Width;
            Height = texture.Height;

            state.BindTexture( Target, Id );
            GL.TexParameter( Target, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );
            GL.TexParameter( Target, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.LinearMipmapLinear );
            GL.TexParameter( Target, TextureParameterName.TextureMaxLevel, texture.MipMapCount - 1 );

            for ( int i = 0; i < texture.ArraySize; i++ )
            for ( int j = 0; j < texture.MipMapCount; j++ )
                SetImage( texture[ i, j ], i, j );

            IsYCbCr = texture.Format == TextureFormat.ATI1 && texture.MipMapCount == 3;
        }

        public Texture( State state, TextureTarget target, PixelInternalFormat internalFormat, int width, int height, PixelFormat format, PixelType type )
        {
            Id = GL.GenTexture();
            Target = target;

            Width = width;
            Height = height;

            state.BindTexture( Target, Id );
            GL.TexParameter( Target, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );
            GL.TexParameter( Target, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.Linear );
            GL.TexParameter( Target, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.ClampToEdge );
            GL.TexParameter( Target, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.ClampToEdge );
            GL.TexParameter( Target, TextureParameterName.TextureMaxLevel, 0 );
            GL.TexImage2D( Target, 0, internalFormat, width, height, 0, format, type, IntPtr.Zero );
        }

        ~Texture()
        {
            Dispose();
        }

        private void SetImage( SubTexture subTexture, int levelIndex, int mipMapIndex )
        {
            if ( levelIndex > 0 )
                levelIndex = sCubeMapIndices[ levelIndex ];

            var target = Target == TextureTarget.TextureCubeMap ? TextureTarget.TextureCubeMapPositiveX + levelIndex : TextureTarget.Texture2D;

            if ( TextureFormatUtilities.IsBlockCompressed( subTexture.Format ) )
            {
                GL.CompressedTexImage2D(
                    target,
                    mipMapIndex,
                    sInternalFormatMap[ subTexture.Format ],
                    subTexture.Width,
                    subTexture.Height,
                    0,
                    subTexture.Data.Length,
                    subTexture.Data );
            }

            else
            {
                GL.TexImage2D(
                    target,
                    mipMapIndex,
                    ( PixelInternalFormat ) sInternalFormatMap[ subTexture.Format ],
                    subTexture.Width,
                    subTexture.Height,
                    0,
                    sPixelFormatMap[ subTexture.Format ],
                    sPixelTypeMap[ subTexture.Format ],
                    subTexture.Data );
            }
        }
    }
}