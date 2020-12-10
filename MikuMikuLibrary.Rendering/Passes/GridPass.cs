using System;
using System.Collections.Generic;
using MikuMikuLibrary.Rendering.Arrays;
using MikuMikuLibrary.Rendering.Buffers;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace MikuMikuLibrary.Rendering.Passes
{
    public class GridPass : Pass
    {
        private Vector3 mGridXColor = new Vector3( 142.0f / 255.0f, 66.0f / 255.0f, 76.0f / 255.0f );
        private Vector3 mGridZColor = new Vector3( 97.0f / 255.0f, 129.0f / 255.0f, 51.0f / 255.0f );
        private Vector3 mGridInnerColor = new Vector3( 83.0f / 255.0f, 83.0f / 255.0f, 83.0f / 255.0f );
        private Vector3 mGridOuterColor = new Vector3( 75.0f / 255.0f, 75.0f / 255.0f, 75.0f / 255.0f );

        private Shader mSceneGridShader;

        public int mVertexArrayId;
        private VertexBuffer<Vector4> mVertexBuffer;

        public override void Initialize( Renderer renderer )
        {
            if ( mSceneGridShader != null )
                return;

            mSceneGridShader = renderer.CreateShader( "Scene/SceneGrid" );

            const float gridSize = 100.0f;
            const float gridSpacing = 0.5f;

            var vertices = new List<Vector4>( ( int ) ( gridSize / gridSpacing * 4 ) );

            for ( float i = -gridSize; i <= gridSize; i += gridSpacing )
            {
                int attrX;
                int attrZ;

                // TODO: What's an actual good way to do this?

                if ( Math.Abs( i ) < 0.001f )
                {
                    attrX = 0;
                    attrZ = 1;
                }

                else if ( Math.Abs( i % ( gridSpacing * 5.0f ) ) < 0.001f )
                    attrX = attrZ = 2;

                else
                    attrX = attrZ = 3;

                vertices.Add( new Vector4( i, 0, -gridSize, attrX ) );
                vertices.Add( new Vector4( i, 0, gridSize, attrX ) );
                vertices.Add( new Vector4( -gridSize, 0, i, attrZ ) );
                vertices.Add( new Vector4( gridSize, 0, i, attrZ ) );
            }

            mVertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray( mVertexArrayId );

            mVertexBuffer = new VertexBuffer<Vector4>( renderer.State, 0, vertices.ToArray() );
        }

        public override void Render( Renderer renderer,Camera camera, Scene scene, Effect effect  )
        {
            renderer.State.Blend( true );
            renderer.State.BlendFunc( BlendingFactor.SrcColor, BlendingFactor.One );

            renderer.State.DepthTest( true );
            renderer.State.DepthMask( false );
            renderer.State.DepthFunc( DepthFunction.Less );

            mSceneGridShader.Use( renderer.State );
            mSceneGridShader.SetUniform( "uInnerColor", mGridInnerColor );
            mSceneGridShader.SetUniform( "uOuterColor", mGridOuterColor );
            mSceneGridShader.SetUniform( "uXColor", mGridXColor );
            mSceneGridShader.SetUniform( "uZColor", mGridZColor );

            renderer.State.BindFramebuffer( renderer.ToneMapFramebuffer );

            renderer.State.BindVertexArray( mVertexArrayId );
            GL.DrawArrays( PrimitiveType.Lines, 0, mVertexBuffer.Length );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                GL.DeleteVertexArray( mVertexArrayId );
                mVertexBuffer.Dispose();
            }

            base.Dispose( disposing );
        }
    }
}