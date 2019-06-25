using OpenTK.Graphics.OpenGL;
using System;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLBuffer<T> : IDisposable where T : struct
    {
        public T[] Array { get; }
        public int Id { get; }
        public BufferTarget Target { get; }
        public BufferUsageHint UsageHint { get; }
        public int Stride { get; }

        public int Length => Array.Length;

        public void Bind()
        {
            GL.BindBuffer( Target, Id );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected void Dispose( bool disposing )
        {
            if ( disposing )
            {
            }

            GL.DeleteBuffer( Id );
        }

        ~GLBuffer()
        {
            Dispose( false );
        }

        public GLBuffer( BufferTarget target, T[] array, int stride, BufferUsageHint usageHint )
        {
            Array = array;
            Id = GL.GenBuffer();
            Target = target;
            UsageHint = usageHint;
            Stride = stride;

            GL.BindBuffer( Target, Id );
            GL.BufferData( Target, Length * Stride, Array, UsageHint );
        }
    }
}
