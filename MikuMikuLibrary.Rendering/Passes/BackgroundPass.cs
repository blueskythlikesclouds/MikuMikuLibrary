using System.Numerics;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Passes
{
    public class BackgroundPass : Pass
    {
        private Shader mCopyColorShader;

        public override void Initialize( Renderer renderer )
        {
            if ( mCopyColorShader == null )
                mCopyColorShader = renderer.CreateShader( "Filter/CopyColor" );
        }

        public override void Render( Renderer renderer, Camera camera, Scene scene, Effect effect  )
        {
            renderer.State.Blend( true );
            renderer.State.BlendFunc( BlendingFactor.OneMinusDstAlpha, BlendingFactor.DstAlpha );

            renderer.State.CullFace( false );

            renderer.State.DepthTest( false );

            mCopyColorShader.Use( renderer.State );
            mCopyColorShader.SetUniform( "uColor", new Vector4( 60.0f / 255.0f, 60.0f / 255.0f, 60.0f / 255.0f, 1.0f ) );

            renderer.State.BindFramebuffer( renderer.ToneMapFramebuffer );
            renderer.RenderQuad();
        }
    }
}
