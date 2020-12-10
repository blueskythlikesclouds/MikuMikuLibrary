using System.Numerics;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Passes
{
    public class SSSPass : Pass
    {    
        private Shader mSceneSSSShader;
        private Shader mCopyTextureShader;
        private Shader mSSSFilterMinShader;
        private Shader mSSSFilterGauss2DShader;

        private FramebufferTexture mSSSLowFilteredFramebuffer;

        public override void Initialize( Renderer renderer )
        {
            if ( mSceneSSSShader != null )
                return;

            mSceneSSSShader = renderer.CreateShader( "Scene/SceneSSS" );
            mCopyTextureShader = renderer.CreateShader( "Filter/CopyTexture" );
            mSSSFilterMinShader = renderer.CreateShader( "Filter/SSS/SSSFilterMin" );
            mSSSFilterGauss2DShader = renderer.CreateShader( "Filter/SSS/SSSFilterGauss2D" );

            mSSSFilterMinShader.SetUniform( "uResolution", Vector2.One / new Vector2( renderer.SSSMiddleTexture.Width, renderer.SSSMiddleTexture.Height ) );
            mSSSFilterGauss2DShader.SetUniform( "uResolution", Vector2.One / new Vector2( renderer.SSSLowFramebuffer.Width, renderer.SSSLowFramebuffer.Height ) );

            mSSSLowFilteredFramebuffer = new FramebufferTexture(
                renderer.State, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, PixelInternalFormat.Rgba16f, 
                Renderer.SSSWidth / 4, Renderer.SSSHeight / 4, PixelFormat.Rgba, PixelType.HalfFloat );
        }

        public override void Render( Renderer renderer, Camera camera, Scene scene, Effect effect )
        {
            renderer.State.Blend( false );
            renderer.State.DepthTest( true );
            renderer.State.DepthFunc( DepthFunction.Less );
            renderer.State.DepthMask( true );

            renderer.State.BindFramebuffer( renderer.SSSHighFramebuffer );

            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            renderer.Scheduler.Begin( camera );
            {
                scene.CharacterRoot?.Render( renderer.Scheduler, scene, Matrix4x4.Identity );
                scene.StageRoot?.Render( renderer.Scheduler, scene, Matrix4x4.Identity );
            }
            renderer.Scheduler.End( renderer, mSceneSSSShader, scene );

            renderer.State.Blend( false );
            renderer.State.CullFace( false );
            renderer.State.DepthTest( true );
            renderer.State.DepthMask( true );
            renderer.State.DepthFunc( DepthFunction.Always );

            renderer.State.BindFramebuffer( renderer.SSSMiddleFramebuffer );
            {
                GL.Clear( ClearBufferMask.ColorBufferBit );

                renderer.State.UseProgram( mCopyTextureShader );
                {
                    renderer.SSSHighTexture.Bind( renderer.State, 0 );
                }

                renderer.RenderQuad();
            }           
            
            renderer.State.BindFramebuffer( renderer.SSSLowFramebuffer );
            {
                GL.Clear( ClearBufferMask.ColorBufferBit );

                renderer.State.UseProgram( mSSSFilterMinShader );
                {
                    renderer.SSSMiddleTexture.Bind( renderer.State, 0 );
                }

                renderer.RenderQuad();
            }

            renderer.State.UseProgram( mSSSFilterGauss2DShader );
            {
                renderer.State.BindFramebuffer( mSSSLowFilteredFramebuffer );
                {
                    renderer.State.BindTexture( renderer.SSSLowTexture );

                    mSSSFilterGauss2DShader.SetUniform( "uDirection", new Vector2( -1, 0 ) );

                    renderer.RenderQuad();
                }

                renderer.State.BindFramebuffer( renderer.SSSLowFilteredFramebuffer );
                {
                    renderer.State.BindTexture( mSSSLowFilteredFramebuffer.Texture );

                    mSSSFilterGauss2DShader.SetUniform( "uDirection", new Vector2( 0, -1 ) );

                    renderer.RenderQuad();
                }
            }
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                mSSSLowFilteredFramebuffer.Dispose();

            base.Dispose( disposing );
        }
    }
}