using System.Numerics;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Passes
{
    public sealed class ScenePass : Pass
    {
        private Shader mSceneShader;

        public override void Initialize( Renderer renderer )
        {
            if ( mSceneShader != null )
                return;

            mSceneShader = renderer.CreateShader( "Scene/Scene" );
        }

        public override void Render( Renderer renderer, Camera camera, Scenes.Scene scene, Effect effect  )
        {
            renderer.State.Blend( false );
            renderer.State.DepthTest( true );
            renderer.State.DepthFunc( DepthFunction.Less );
            renderer.State.DepthMask( true );

            renderer.State.BindFramebuffer( renderer.SceneFramebuffer );

            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            renderer.Scheduler.Begin( camera );
            {
                scene.CharacterRoot?.Render( renderer.Scheduler, scene, Matrix4x4.Identity );
                scene.StageRoot?.Render( renderer.Scheduler, scene, Matrix4x4.Identity );
            }
            renderer.Scheduler.End( renderer, mSceneShader, scene );
        }
    }
}