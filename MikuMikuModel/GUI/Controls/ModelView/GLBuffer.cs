using System;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLBuffer<T> : IDisposable where T : unmanaged
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
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public void Bind()
        {
            GL.BindBuffer( Target, Id );
        }

        protected void Dispose( bool disposing )
        {
            if ( mDisposed )
                return;

            GL.DeleteBuffer( Id );
            GL.Finish();

            GC.RemoveMemoryPressure( Stride * Array.Length );

            mDisposed = true;
        }

        ~GLBuffer()
        {
            Dispose( false );
        }

        public unsafe GLBuffer( BufferTarget target, T[] array, BufferUsageHint usageHint )
        {
            Array = array;
            Id = GL.GenBuffer();
            Target = target;
            UsageHint = usageHint;
            Stride = sizeof( T );

            GL.BindBuffer( Target, Id );
            GL.BufferData( Target, Length * Stride, Array, UsageHint );
            
            GC.AddMemoryPressure( Stride * Array.Length );
        }
    }
}