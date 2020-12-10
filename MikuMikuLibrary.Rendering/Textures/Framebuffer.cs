using System;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Textures
{
    public sealed class Framebuffer : IDisposable
    {
        private bool mDisposed;

        public int Width { get; }
        public int Height { get; }

        public int Id { get; }

        public void Bind( State state )
        {
            state.Viewport( 0, 0, Width, Height );
            state.BindFramebuffer( FramebufferTarget.Framebuffer, Id );
        }

        public void Attach( FramebufferAttachment attachment, Texture texture )
        {
            GL.FramebufferTexture2D( FramebufferTarget.Framebuffer, attachment, texture.Target, texture.Id, 0 );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            GL.DeleteFramebuffer( Id );
            GC.SuppressFinalize( this );
        }

        public Framebuffer( State state, int width, int height )
        {
            Width = width;
            Height = height;
            Id = GL.GenFramebuffer();

            state.BindFramebuffer( FramebufferTarget.Framebuffer, Id );
        }

        ~Framebuffer()
        {
            Dispose();
        }
    }

    public sealed class FramebufferTexture : IDisposable
    {
        public Framebuffer Framebuffer { get; }
        public Texture Texture { get; }

        public void Dispose()
        {
            Framebuffer.Dispose();
            Texture.Dispose();
        }

        public FramebufferTexture( State state, FramebufferAttachment attachment, TextureTarget target, PixelInternalFormat internalFormat, 
            int width, int height, PixelFormat format, PixelType type )
        {
            Framebuffer = new Framebuffer( state, width, height );
            Texture = new Texture( state, target, internalFormat, width, height, format, type );

            Framebuffer.Bind( state );
            Framebuffer.Attach( attachment, Texture );
        }
    }
}
