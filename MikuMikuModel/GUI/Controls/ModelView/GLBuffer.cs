using OpenTK.Graphics.OpenGL;
using System;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLBuffer<T> : IDisposable where T : struct
    {
        public T[] Array { get; }
        public int ID { get; }
        public BufferTarget Target { get; }
        public BufferUsageHint UsageHint { get; }
        public int Stride { get; }

        public int Length
        {
            get { return Array.Length; }
        }

        public void Bind()
        {
            GL.BindBuffer( Target, ID );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected void Dispose( bool disposing )
        {
            if ( disposing ) { }

            GL.DeleteBuffer( ID );
        }

        ~GLBuffer()
        {
            Dispose( false );
        }

        public GLBuffer( BufferTarget target, T[] array, int stride, BufferUsageHint usageHint )
        {
            Array = array;
            ID = GL.GenBuffer();
            Target = target;
            UsageHint = usageHint;
            Stride = stride;

            GL.BindBuffer( Target, ID );
            GL.BufferData( Target, Length * Stride, Array, UsageHint );
        }
    }
}
