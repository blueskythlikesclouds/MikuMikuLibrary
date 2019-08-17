using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;
using OpenTK.Graphics.OpenGL;
using Sn = System.Numerics;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLMesh : IDrawable
    {
        public int VertexArrayId { get; }

        public GLBuffer<Sn.Vector3> PositionBuffer { get; }
        public GLBuffer<Sn.Vector3> NormalBuffer { get; }
        public GLBuffer<Sn.Vector4> TangentBuffer { get; }
        public GLBuffer<Sn.Vector2> TexCoord1Buffer { get; }
        public GLBuffer<Sn.Vector2> TexCoord2Buffer { get; }
        public GLBuffer<Color> ColorBuffer { get; }

        public List<GLSubMesh> SubMeshes { get; }

        public void Draw( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "uHasNormal", NormalBuffer != null );
            shaderProgram.SetUniform( "uHasTexCoord1", TexCoord1Buffer != null );
            shaderProgram.SetUniform( "uHasTexCoord2", TexCoord2Buffer != null );
            shaderProgram.SetUniform( "uHasColor", ColorBuffer != null );
            shaderProgram.SetUniform( "uHasTangent", TangentBuffer != null );

            GL.BindVertexArray( VertexArrayId );

            foreach ( var indexTable in SubMeshes )
                indexTable.Draw( shaderProgram );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        private GLBuffer<T> GenerateVertexAttribute<T>( int index, T[] array ) where T : unmanaged
        {
            var buffer = new GLBuffer<T>( BufferTarget.ArrayBuffer, array, BufferUsageHint.StaticDraw );

            GL.VertexAttribPointer( index, buffer.Stride / sizeof( float ), VertexAttribPointerType.Float, false,
                buffer.Stride, 0 );
            GL.EnableVertexAttribArray( index );

            return buffer;
        }

        protected void Dispose( bool disposing )
        {
            if ( disposing )
            {
                PositionBuffer?.Dispose();
                NormalBuffer?.Dispose();
                TangentBuffer?.Dispose();
                TexCoord1Buffer?.Dispose();
                TexCoord2Buffer?.Dispose();
                ColorBuffer?.Dispose();

                foreach ( var indexTable in SubMeshes )
                    indexTable.Dispose();
            }

            GL.DeleteVertexArray( VertexArrayId );
        }

        ~GLMesh()
        {
            Dispose( false );
        }

        public GLMesh( Mesh mesh, List<GLMaterial> materials )
        {
            if ( mesh.Vertices == null )
                throw new ArgumentException( "Submesh must have vertices", nameof( mesh ) );

            VertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray( VertexArrayId );

            PositionBuffer = GenerateVertexAttribute( 0, mesh.Vertices );

            if ( mesh.Normals != null )
                NormalBuffer = GenerateVertexAttribute( 1, mesh.Normals );

            if ( mesh.Tangents != null )
                TangentBuffer = GenerateVertexAttribute( 2, mesh.Tangents );

            if ( mesh.UVChannel1 != null )
                TexCoord1Buffer = GenerateVertexAttribute( 3, mesh.UVChannel1 );

            if ( mesh.UVChannel2 != null && !mesh.UVChannel1.SequenceEqual( mesh.UVChannel1 ) )
                TexCoord2Buffer = GenerateVertexAttribute( 4, mesh.UVChannel2 );

            if ( mesh.Colors != null )
                ColorBuffer = GenerateVertexAttribute( 5, mesh.Colors );

            SubMeshes = new List<GLSubMesh>();
            foreach ( var subMesh in mesh.SubMeshes )
                SubMeshes.Add( new GLSubMesh( subMesh, materials ) );
        }
    }
}