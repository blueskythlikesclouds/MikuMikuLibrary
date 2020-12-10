using System;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Rendering.Buffers;
using OpenTK.Graphics.OpenGL;
using GLPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using MMLPrimitiveType = MikuMikuLibrary.Objects.PrimitiveType;

namespace MikuMikuLibrary.Rendering.Arrays
{
    public sealed class ElementArray : IDisposable
    {
        public MMLPrimitiveType PrimitiveType { get; }
        public ArrayBuffer<uint> Elements { get; }

        public void Bind( State state )
        {
            Elements.Bind( state );
        }

        public void Render()
        {
            GL.DrawElements( ( GLPrimitiveType ) PrimitiveType, Elements.Length, DrawElementsType.UnsignedInt, 0 );
        }

        public void Dispose()
        {
            Elements.Dispose();
        }

        public ElementArray( State state, MMLPrimitiveType primitiveType, uint[] elements )
        {
            PrimitiveType = primitiveType;
            Elements = new ArrayBuffer<uint>( state, BufferTarget.ElementArrayBuffer, elements, BufferUsageHint.StaticCopy );
        }

        public ElementArray( State state, SubMesh subMesh ) : this( state, subMesh.PrimitiveType, subMesh.Indices )
        {
        }
    }
}