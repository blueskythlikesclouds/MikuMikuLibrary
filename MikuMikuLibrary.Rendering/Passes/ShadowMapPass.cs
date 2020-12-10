using System.Linq;
using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Lights;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Passes
{
    public class ShadowMapPass : Pass
    {
        private ShadowMapCamera mShadowMapCamera;

        private Shader mSceneShadowMapShader;
        private Shader mEsmGaussShader;

        private Framebuffer mShadowMapFilteredFramebuffer;
        private Texture mShadowMapFilteredTexture;

        public override void Initialize( Renderer renderer )
        {
            if ( mShadowMapCamera != null )
                return;

            mShadowMapCamera = new ShadowMapCamera();

            mSceneShadowMapShader = renderer.CreateShader( "Scene/SceneShadowMap" );

            mEsmGaussShader = renderer.CreateShader( "Filter/ESM/EsmGauss" );
            mEsmGaussShader.SetUniform( "uResolution", new Vector2( 1.0f / Renderer.ShadowMapSize ) );

            mShadowMapFilteredFramebuffer = new Framebuffer( renderer.State, Renderer.ShadowMapSize, Renderer.ShadowMapSize );
            mShadowMapFilteredTexture = new Texture( renderer.State, TextureTarget.Texture2D, PixelInternalFormat.DepthComponent,
                Renderer.ShadowMapSize, Renderer.ShadowMapSize, PixelFormat.DepthComponent, PixelType.Float );

            GL.DrawBuffer( DrawBufferMode.None );
            GL.ReadBuffer( ReadBufferMode.None );

            mShadowMapFilteredFramebuffer.Attach( FramebufferAttachment.DepthAttachment, mShadowMapFilteredTexture );
        }

        public override void Render( Renderer renderer,Camera camera, Scene scene, Effect effect )
        {
            var lightPosition = effect.LightParameter.Groups[ 0 ].Lights.First( 
                x => x.Id == LightId.Character ).Position;

            mShadowMapCamera.CalculateMatrices( scene.BoundingSphere,
                Vector3.Normalize( lightPosition.To3D() ) );

            renderer.State.DepthTest( true );
            renderer.State.DepthFunc( DepthFunction.Less );
            renderer.State.DepthMask( true );

            renderer.State.BindFramebuffer( renderer.ShadowMapFramebuffer );
            GL.Clear( ClearBufferMask.DepthBufferBit );

            renderer.Scheduler.Begin( mShadowMapCamera );
            {
                scene.CharacterRoot?.Render( renderer.Scheduler, scene, Matrix4x4.Identity );
            }
            renderer.Scheduler.End( renderer, mSceneShadowMapShader, scene );

            renderer.State.Blend( false );
            renderer.State.CullFace( false );
            renderer.State.DepthTest( true );
            renderer.State.DepthMask( true );
            renderer.State.DepthFunc( DepthFunction.Always );

            renderer.State.UseProgram( mEsmGaussShader );
            {
                renderer.State.BindFramebuffer( mShadowMapFilteredFramebuffer );
                {
                    renderer.State.BindTexture( renderer.ShadowMapTexture );

                    mEsmGaussShader.SetUniform( "uDirection", new Vector2( -1, 0 ) );

                    renderer.RenderQuad();
                }

                renderer.State.BindFramebuffer( renderer.ShadowMapFilteredFramebuffer );
                {
                    renderer.State.BindTexture( mShadowMapFilteredTexture );

                    mEsmGaussShader.SetUniform( "uDirection", new Vector2( 0, -1 ) );

                    renderer.RenderQuad();
                }
            }

            renderer.ShadowLightViewProjection = mShadowMapCamera.GetView() * mShadowMapCamera.GetProjection();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mShadowMapFilteredFramebuffer.Dispose();
                mShadowMapFilteredTexture.Dispose();
            }

            base.Dispose( disposing );
        }
    }
}