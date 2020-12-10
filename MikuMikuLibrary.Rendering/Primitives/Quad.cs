using System;
using System.Numerics;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Rendering.Arrays;

namespace MikuMikuLibrary.Rendering.Primitives
{
    public sealed class Quad : IDisposable
    {
        public VertexArray VertexArray { get; }
        public ElementArray ElementArray { get; }

        public void Dispose()
        {
            VertexArray.Dispose();
            ElementArray.Dispose();
        }

        public Quad( State state )
        {
            VertexArray = new VertexArray( state,
                new[]
                {
                    new Vector3( -1.0f, -1.0f, 0.0f ),
                    new Vector3( -1.0f, 1.0f, 0.0f ),
                    new Vector3( 1.0f, 1.0f, 0.0f ),
                    new Vector3( 1.0f, -1.0f, 0.0f )
                } );

            ElementArray = new ElementArray( state, PrimitiveType.Triangles, new uint[]
            {
                0, 2, 1,
                0, 3, 2
            } );
        }
    }
}