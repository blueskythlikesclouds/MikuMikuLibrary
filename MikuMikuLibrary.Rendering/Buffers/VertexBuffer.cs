using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Buffers
{
    public sealed class VertexBuffer<T> : ArrayBuffer<T> where T : unmanaged
    {
        public VertexBuffer( State state, int index, T[] array ) : base( state, BufferTarget.ArrayBuffer, array, BufferUsageHint.StaticDraw )
        {
            GL.VertexAttribPointer( index, Stride / sizeof( float ), VertexAttribPointerType.Float, false, Stride, 0 );
            GL.EnableVertexAttribArray( index );
        }
    }
}
