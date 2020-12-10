using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering
{
    public class Scheduler
    {
        private readonly List<RenderCommand> mOpaqueRenderCommands;
        private readonly List<RenderCommand> mTransparentRenderCommands;

        private Matrix4x4 mCameraView;
        private Matrix4x4 mCameraProjection;
        private Vector3 mCameraPosition;
        private Frustum mFrustum;

        public void Begin( Camera camera )
        {
            mOpaqueRenderCommands.Clear();
            mTransparentRenderCommands.Clear();

            mCameraPosition = camera.Position;
            mCameraView = camera.GetView();
            mCameraProjection = camera.GetProjection();
            mFrustum = new Frustum( mCameraProjection, mCameraView );
        }

        public void Render( RenderCommand renderCommand )
        {
            if ( !mFrustum.Intersect( renderCommand.BoundingSphere ) )
                return;

            if ( renderCommand.Material.IsTransparent() )
                mTransparentRenderCommands.Add( renderCommand );
            else
                mOpaqueRenderCommands.Add( renderCommand );
        }

        public void End( Renderer renderer, Shader shader, Scene scene )
        {
            shader.Use( renderer.State );

            renderer.CameraUniformBuffer.SetData( renderer.State, new CameraData
            {
                View = mCameraView,
                Projection = mCameraProjection,
                LightViewProjection = renderer.ShadowLightViewProjection,
                ViewPosition = new Vector4( mCameraPosition, 1.0f )
            } );

            renderer.ShadowMapFilteredTexture?.Bind( renderer.State, Sampler.ShadowMap.TextureUnit );
            renderer.SSSLowFilteredTexture?.Bind( renderer.State, Sampler.SSS.TextureUnit );

            renderer.State.DepthTest( true );
            renderer.State.DepthFunc( DepthFunction.Less );

            if ( mOpaqueRenderCommands.Count > 0 )
            {
                renderer.State.Blend( false );
                renderer.State.DepthMask( true );

                foreach ( var item in mOpaqueRenderCommands )
                    DoScheduledRender( item );
            }

            if ( mTransparentRenderCommands.Count > 0 )
            {
                renderer.State.Blend( true );
                renderer.State.DepthMask( false );

                mTransparentRenderCommands.Sort( ( x, y ) =>
                {
                    float xDistance = Vector3.Distance( x.BoundingSphere.Center, mCameraPosition );
                    float yDistance = Vector3.Distance( y.BoundingSphere.Center, mCameraPosition );

                    return yDistance.CompareTo( xDistance );
                } );

                foreach ( var item in mTransparentRenderCommands )
                    DoScheduledRender( item );
            }

            void DoScheduledRender( RenderCommand scheduledRender )
            {
                shader.SetUniform( "uModel", scheduledRender.Transformation );

                scheduledRender.Material.Bind( renderer, shader, scene );
                scheduledRender.VertexArray.Bind( renderer.State );
                scheduledRender.ElementArray.Bind( renderer.State );
                scheduledRender.ElementArray.Render();
            }
        }

        public Scheduler()
        {
            mOpaqueRenderCommands = new List<RenderCommand>( 128 );
            mTransparentRenderCommands = new List<RenderCommand>( 128 );
        }
    }
}