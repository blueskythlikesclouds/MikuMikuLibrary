using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Passes
{
    public class EndPass : Pass
    {
        private Shader mCopyTextureShader;

        public override void Initialize( Renderer renderer )
        {
            if ( mCopyTextureShader == null )
                mCopyTextureShader = renderer.CreateShader( "Filter/CopyTexture" );
        }

        public override void Render( Renderer renderer, Camera camera, Scene scene, Effect effect  )
        {
            renderer.State.Blend( false );
            renderer.State.CullFace( false );
            renderer.State.DepthTest( false );

            renderer.State.UseProgram( mCopyTextureShader );
            renderer.State.BindTexture( renderer.ToneMapColorTexture );

            renderer.State.BindFramebuffer( FramebufferTarget.Framebuffer, 0 );
            renderer.State.Viewport( 0, 0, renderer.Width, renderer.Height );
            renderer.RenderQuad();
        }
    }
}