using System;
using OpenTK.Graphics.OpenGL;
using MMLLightMap = MikuMikuLibrary.IBLs.LightMap;

namespace MikuMikuLibrary.Rendering.IBLs
{
    public class LightMap
    {
        private bool mDisposed;

        public int Id { get; }

        public void Bind( State state, int unit )
        {
            state.BindTexture( TextureTarget.TextureCubeMap, Id, unit );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            GL.DeleteTexture( Id );
            GC.SuppressFinalize( this );
        }

        public LightMap( State state, MMLLightMap lightMap, bool generateMipMaps )
        {
            Id = GL.GenTexture();

            state.BindTexture( TextureTarget.TextureCubeMap, Id );
            GL.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.ClampToEdge );
            GL.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.ClampToEdge );
            GL.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, ( int ) TextureWrapMode.ClampToEdge );

            GL.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );

            GL.TexParameter( TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter,
                ( int ) ( generateMipMaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear ) );

            for ( int i = 0; i < 6; i++ )
            {
                GL.TexImage2D(
                    TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    PixelInternalFormat.Rgba16f,
                    lightMap.Width,
                    lightMap.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.HalfFloat,
                    lightMap.Sides[ i ] );
            }

            if ( generateMipMaps )
                GL.GenerateMipmap( GenerateMipmapTarget.TextureCubeMap );
        }

        ~LightMap()
        {
            Dispose();
        }
    }
}