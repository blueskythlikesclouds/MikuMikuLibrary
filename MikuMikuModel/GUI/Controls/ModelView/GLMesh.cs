using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using SN = System.Numerics;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLMesh : IGLDraw
    {
        public int VertexArrayId { get; }

        public GLBuffer<SN.Vector3> PositionBuffer { get; }
        public GLBuffer<SN.Vector3> NormalBuffer { get; }
        public GLBuffer<SN.Vector4> TangentBuffer { get; }
        public GLBuffer<SN.Vector2> TexCoordBuffer { get; }
        public GLBuffer<SN.Vector2> TexCoord2Buffer { get; }
        public GLBuffer<Color> ColorBuffer { get; }

        public List<GLSubMesh> SubMeshes { get; }

        public void Draw( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "hasNormal", NormalBuffer != null );
            shaderProgram.SetUniform( "hasTexCoord", TexCoordBuffer != null );
            shaderProgram.SetUniform( "hasTexCoord2", TexCoord2Buffer != null );
            shaderProgram.SetUniform( "hasColor", ColorBuffer != null );
            shaderProgram.SetUniform( "hasTangent", TangentBuffer != null );

            GL.BindVertexArray( VertexArrayId );

            foreach ( var indexTable in SubMeshes )
                indexTable.Draw( shaderProgram );
        }

        private GLBuffer<T> GenerateVertexAttribute<T>( int index, T[] array, int count ) where T : struct
        {
            var buffer = new GLBuffer<T>( BufferTarget.ArrayBuffer, array, count * sizeof( float ),
                BufferUsageHint.StaticDraw );

            GL.VertexAttribPointer( index, count, VertexAttribPointerType.Float, false, buffer.Stride, 0 );
            GL.EnableVertexAttribArray( index );

            return buffer;
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
                PositionBuffer?.Dispose();
                NormalBuffer?.Dispose();
                TangentBuffer?.Dispose();
                TexCoordBuffer?.Dispose();
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

            PositionBuffer = GenerateVertexAttribute( 0, mesh.Vertices, 3 );

            if ( mesh.Normals != null )
                NormalBuffer = GenerateVertexAttribute( 1, mesh.Normals, 3 );

            if ( mesh.Tangents != null )
                TangentBuffer = GenerateVertexAttribute( 2, mesh.Tangents, 4 );

            if ( mesh.UVChannel1 != null )
                TexCoordBuffer = GenerateVertexAttribute( 3, mesh.UVChannel1, 2 );

            if ( mesh.UVChannel2 != null && !mesh.UVChannel1.SequenceEqual( mesh.UVChannel2 ) )
                TexCoord2Buffer = GenerateVertexAttribute( 4, mesh.UVChannel2, 2 );

            if ( mesh.Colors != null )
                ColorBuffer = GenerateVertexAttribute( 5, mesh.Colors, 4 );

            SubMeshes = new List<GLSubMesh>();
            foreach ( var subMesh in mesh.SubMeshes )
                SubMeshes.Add( new GLSubMesh( subMesh, materials ) );
        }
    }
}
