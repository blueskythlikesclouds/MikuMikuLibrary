using System;
using System.Numerics;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Rendering.Buffers;
using MikuMikuLibrary.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Arrays
{
    public sealed class VertexArray : IDisposable
    {
        private bool mDisposed;

        public int Id { get; }

        public VertexBuffer<Vector3> Positions { get; }
        public VertexBuffer<Vector3> Normals { get; }
        public VertexBuffer<Vector4> Tangents { get; }
        public VertexBuffer<Vector2> TexCoords0 { get; }
        public VertexBuffer<Vector2> TexCoords1 { get; }
        public VertexBuffer<Vector2> TexCoords2 { get; }
        public VertexBuffer<Vector2> TexCoords3 { get; }
        public VertexBuffer<Color> Colors0 { get; }
        public VertexBuffer<Color> Colors1 { get; }

        public void Bind( State state )
        {
            state.BindVertexArray( Id );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        private void Dispose( bool disposing )
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            if ( disposing )
            {
                Positions?.Dispose();
                Normals?.Dispose();
                Tangents?.Dispose();
                TexCoords0?.Dispose();
                TexCoords1?.Dispose();
                TexCoords2?.Dispose();
                TexCoords3?.Dispose();
                Colors0?.Dispose();
                Colors1?.Dispose();
            }

            GL.DeleteVertexArray( Id );
        }

        public VertexArray(
            State state,
            Vector3[] positions,
            Vector3[] normals = null,
            Vector4[] tangents = null,
            Vector2[] texCoords0 = null,
            Vector2[] texCoords1 = null,
            Vector2[] texCoords2 = null,
            Vector2[] texCoords3 = null,
            Color[] colors0 = null,
            Color[] colors1 = null
        )
        {
            if ( positions == null )
                throw new ArgumentNullException( nameof( positions ) );

            Id = GL.GenVertexArray();
            state.BindVertexArray( Id );

            Positions = new VertexBuffer<Vector3>( state, 0, positions );

            if ( normals != null )
                Normals = new VertexBuffer<Vector3>( state, 1, normals );

            if ( tangents != null )
                Tangents = new VertexBuffer<Vector4>( state, 2, tangents );

            if ( texCoords0 != null )
                TexCoords0 = new VertexBuffer<Vector2>( state, 3, texCoords0 );

            if ( texCoords1 != null )
                TexCoords1 = new VertexBuffer<Vector2>( state, 4, texCoords1 );

            if ( texCoords2 != null )
                TexCoords2 = new VertexBuffer<Vector2>( state, 5, texCoords2 );

            if ( texCoords3 != null )
                TexCoords3 = new VertexBuffer<Vector2>( state, 6, texCoords3 );

            if ( colors0 != null )
                Colors0 = new VertexBuffer<Color>( state, 7, colors0 );

            if ( colors1 != null )
                Colors1 = new VertexBuffer<Color>( state, 8, colors1 );
        }

        public VertexArray( State state, Mesh mesh )
            : this(
                state,
                mesh.Positions,
                mesh.Normals,
                mesh.Tangents,
                mesh.TexCoords0,
                mesh.TexCoords1,
                mesh.TexCoords2,
                mesh.TexCoords3,
                mesh.Colors0,
                mesh.Colors1
            )
        {

        }

        ~VertexArray()
        {
            Dispose( false );
        }
    }
}