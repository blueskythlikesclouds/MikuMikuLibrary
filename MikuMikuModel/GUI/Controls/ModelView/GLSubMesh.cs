using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using SN = System.Numerics;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLSubMesh : IGLDraw
    {
        public int VertexArrayId { get; }

        public GLBuffer<SN.Vector3> PositionBuffer { get; }
        public GLBuffer<SN.Vector3> NormalBuffer { get; }
        public GLBuffer<SN.Vector4> TangentBuffer { get; }
        public GLBuffer<SN.Vector2> TexCoordBuffer { get; }
        public GLBuffer<SN.Vector2> TexCoord2Buffer { get; }
        public GLBuffer<Color> ColorBuffer { get; }

        public List<GLIndexTable> IndexTables { get; }

        public void Draw( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "hasNormal", NormalBuffer != null );
            shaderProgram.SetUniform( "hasTexCoord", TexCoordBuffer != null );
            shaderProgram.SetUniform( "hasTexCoord2", TexCoord2Buffer != null );
            shaderProgram.SetUniform( "hasColor", ColorBuffer != null );
            shaderProgram.SetUniform( "hasTangent", TangentBuffer != null );

            GL.BindVertexArray( VertexArrayId );

            foreach ( var indexTable in IndexTables )
                indexTable.Draw( shaderProgram );
        }

        private GLBuffer<T> GenerateVertexAttribute<T>( int index, T[] array, int count ) where T : struct
        {
            var buffer = new GLBuffer<T>( BufferTarget.ArrayBuffer, array, count * sizeof( float ), BufferUsageHint.StaticDraw );

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

                foreach ( var indexTable in IndexTables )
                    indexTable.Dispose();
            }

            GL.DeleteVertexArray( VertexArrayId );
        }

        ~GLSubMesh()
        {
            Dispose( false );
        }

        public GLSubMesh( SubMesh subMesh, List<GLMaterial> materials )
        {
            if ( subMesh.Vertices == null )
                throw new ArgumentException( "Submesh must have vertices", nameof( subMesh ) );

            VertexArrayId = GL.GenVertexArray();
            GL.BindVertexArray( VertexArrayId );

            PositionBuffer = GenerateVertexAttribute( 0, subMesh.Vertices, 3 );

            if ( subMesh.Normals != null )
                NormalBuffer = GenerateVertexAttribute( 1, subMesh.Normals, 3 );

            if ( subMesh.Tangents != null )
                TangentBuffer = GenerateVertexAttribute( 2, subMesh.Tangents, 4 );

            if ( subMesh.UVChannel1 != null )
                TexCoordBuffer = GenerateVertexAttribute( 3, subMesh.UVChannel1, 2 );

            if ( subMesh.UVChannel2 != null && !subMesh.UVChannel1.SequenceEqual( subMesh.UVChannel2 ) )
                TexCoord2Buffer = GenerateVertexAttribute( 4, subMesh.UVChannel2, 2 );

            if ( subMesh.Colors != null )
                ColorBuffer = GenerateVertexAttribute( 5, subMesh.Colors, 4 );


            IndexTables = new List<GLIndexTable>();
            foreach ( var indexTable in subMesh.IndexTables )
                IndexTables.Add( new GLIndexTable( indexTable, materials ) );
        }
    }
}
