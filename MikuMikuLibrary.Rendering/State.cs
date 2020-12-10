using MikuMikuLibrary.Rendering.Arrays;
using MikuMikuLibrary.Rendering.Buffers;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering
{
    public class State
    {
        private bool mBlend;
        private BlendingFactor mSrcBlendingFactor;
        private BlendingFactor mDstBlendingFactor;
        private bool mCullFace;
        private CullFaceMode mCullFaceMode;
        private bool mDepthTest;
        private DepthFunction mDepthFunc;
        private bool mDepthMask;
        private int mActiveTexture;
        private readonly int[] mTextures = new int[ 32 ];
        private int mBufferId;
        private int mVertexArrayId;
        private int mProgramId;
        private int mViewportX;
        private int mViewportY;
        private int mViewportWidth;
        private int mViewportHeight;
        private int mFramebufferId;

        public void Blend( bool value )
        {
            if ( mBlend == value )
                return;

            mBlend = value;

            if ( mBlend )
                GL.Enable( EnableCap.Blend );
            else
                GL.Disable( EnableCap.Blend );
        }

        public void BlendFunc( BlendingFactor srcBlendingFactor, BlendingFactor dstBlendingFactor )
        {
            if ( mSrcBlendingFactor == srcBlendingFactor && mDstBlendingFactor == dstBlendingFactor )
                return;

            GL.BlendFuncSeparate( 
                ( BlendingFactorSrc ) ( mSrcBlendingFactor = srcBlendingFactor ),
                ( BlendingFactorDest ) ( mDstBlendingFactor = dstBlendingFactor ), 
                BlendingFactorSrc.OneMinusDstAlpha, BlendingFactorDest.One );
        }

        public void CullFace( bool cullFace )
        {
            if ( mCullFace == cullFace )
                return;

            mCullFace = cullFace;

            if ( mCullFace )
                GL.Enable( EnableCap.CullFace );

            else
                GL.Disable( EnableCap.CullFace );
        }

        public void CullFaceMode( CullFaceMode cullFaceMode )
        {
            if ( mCullFaceMode == cullFaceMode )
                return;

            GL.CullFace( mCullFaceMode = cullFaceMode );
        }

        public void DepthTest( bool depthTest )
        {
            if ( mDepthTest == depthTest )
                return;

            mDepthTest = depthTest;

            if ( mDepthTest )
                GL.Enable( EnableCap.DepthTest );
            else
                GL.Disable( EnableCap.DepthTest );
        }

        public void DepthFunc( DepthFunction depthFunc )
        {
            if ( mDepthFunc == depthFunc )
                return;

            GL.DepthFunc( mDepthFunc = depthFunc );
        }

        public void DepthMask( bool depthMask )
        {
            if ( mDepthMask == depthMask )
                return;

            GL.DepthMask( mDepthMask = depthMask );
        }

        public void BindTexture( TextureTarget target, int id, int unit = 0 )
        {
            if ( mActiveTexture != unit )
                GL.ActiveTexture( TextureUnit.Texture0 + ( mActiveTexture = unit ) );

            if ( mTextures[ mActiveTexture ] != id )
                GL.BindTexture( target, mTextures[ mActiveTexture ] = id );
        }

        public void BindTexture( Texture texture, int unit = 0 )
        {
            BindTexture( texture.Target, texture.Id, unit );
        }

        public void BindBuffer( BufferTarget target, int bufferId )
        {
            if ( mBufferId == bufferId )
                return;

            GL.BindBuffer( target, mBufferId = bufferId );
        }

        public void BindBuffer<T>( ArrayBuffer<T> buffer ) where T : unmanaged
        {
            BindBuffer( buffer.Target, buffer.Id );
        }

        public void BindVertexArray( int vertexArrayId )
        {
            if ( mVertexArrayId == vertexArrayId )
                return;

            GL.BindVertexArray( mVertexArrayId = vertexArrayId );
        }

        public void UseProgram( int programId )
        {
            if ( mProgramId == programId )
                return;

            mActiveTexture = -1;

            GL.UseProgram( mProgramId = programId );
        }

        public void UseProgram( Shader shader )
        {
            UseProgram( shader.Id );
        }

        public void Viewport( int x, int y, int width, int height )
        {
            if ( x == mViewportX && y == mViewportY && width == mViewportWidth && height == mViewportHeight )
                return;

            GL.Viewport( mViewportX = x, mViewportY = y, mViewportWidth = width, mViewportHeight = height );
        }

        public void BindFramebuffer( FramebufferTarget target, int framebufferId )
        {
            if ( mFramebufferId == framebufferId )
                return;

            GL.BindFramebuffer( target, mFramebufferId = framebufferId );
        }

        public void BindFramebuffer( Framebuffer framebuffer )
        {
            Viewport( 0, 0, framebuffer.Width, framebuffer.Height );
            BindFramebuffer( FramebufferTarget.Framebuffer, framebuffer.Id );
        }

        public void BindFramebuffer( Framebuffer framebuffer, int x, int y, int width, int height )
        {
            Viewport( x, y, width, height );
            BindFramebuffer( FramebufferTarget.Framebuffer, framebuffer.Id );
        }

        public void BindFramebuffer( FramebufferTexture framebufferTexture )
        {
            BindFramebuffer( framebufferTexture.Framebuffer );
        }

        public State()
        {
            GL.Disable( EnableCap.Blend );
            GL.Enable( EnableCap.CullFace );
            GL.Enable( EnableCap.DepthTest );
            GL.DepthMask( true );
            GL.DepthFunc( DepthFunction.Less );
            GL.FrontFace( FrontFaceDirection.Ccw );
            GL.Enable( EnableCap.PrimitiveRestart );
            GL.PrimitiveRestartIndex( -1 );

            mBlend = false;
            mCullFace = true;
            mDepthTest = true;
            mDepthMask = true;
            mDepthFunc = DepthFunction.Less;
        }
    }
}