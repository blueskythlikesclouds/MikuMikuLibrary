using System;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuLibrary.Rendering.Buffers
{
    public sealed class UniformBuffer<T> where T : unmanaged
    {
        private bool mDisposed;

        public int Id { get; }
        public int BindingIndex { get; }

        public unsafe void SetData( State state, T data )
        {
            state.BindBuffer( BufferTarget.UniformBuffer, Id );
            GL.BufferSubData( BufferTarget.UniformBuffer, IntPtr.Zero, sizeof( T ), ref data );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            GL.DeleteBuffer( Id );
            GC.SuppressFinalize( this );
        }

        public unsafe UniformBuffer( State state, int bindingIndex )
        {
            Id = GL.GenBuffer();
            BindingIndex = bindingIndex;

            state.BindBuffer( BufferTarget.UniformBuffer, Id );
            GL.BufferData( BufferTarget.UniformBuffer, sizeof( T ), IntPtr.Zero, BufferUsageHint.DynamicDraw );
            GL.BindBufferRange( BufferRangeTarget.UniformBuffer, BindingIndex, Id, IntPtr.Zero, sizeof( T ) );
        }

        ~UniformBuffer()
        {
            Dispose();
        }
    }
}