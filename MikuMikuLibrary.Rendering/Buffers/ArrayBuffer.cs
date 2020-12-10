using System;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Buffers
{
    public class ArrayBuffer<T> : IDisposable where T : unmanaged
    {
        private bool mDisposed;

        public T[] Array { get; }
        public int Id { get; }
        public BufferTarget Target { get; }
        public BufferUsageHint UsageHint { get; }
        public int Stride { get; }
        public int Length => Array.Length;

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            GL.DeleteBuffer( Id );
            GL.Finish();

            GC.SuppressFinalize( this );
        }

        public void Bind( State state )
        {
            state.BindBuffer( Target, Id );
        }

        public unsafe ArrayBuffer( State state, BufferTarget target, T[] array, BufferUsageHint usageHint )
        {
            Array = array;
            Id = GL.GenBuffer();
            Target = target;
            UsageHint = usageHint;
            Stride = sizeof( T );

            state.BindBuffer( Target, Id );
            GL.BufferData( Target, Length * Stride, Array, UsageHint );
            
            GC.AddMemoryPressure( Stride * Array.Length );
        }

        ~ArrayBuffer()
        {
            Dispose();
        }
    }
}