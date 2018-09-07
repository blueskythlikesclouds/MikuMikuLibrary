using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLSubMesh : IGLDraw
    {
        public int VertexArrayID { get; }

        public GLBuffer<System.Numerics.Vector3> PositionBuffer { get; }
        public GLBuffer<System.Numerics.Vector3> NormalBuffer { get; }
        public GLBuffer<System.Numerics.Vector4> TangentBuffer { get; }
        public GLBuffer<System.Numerics.Vector2> TexCoordBuffer { get; }
        public GLBuffer<System.Numerics.Vector2> TexCoord2Buffer { get; }
        public GLBuffer<Color> ColorBuffer { get; }

        public List<GLIndexTable> IndexTables { get; }

        public void Draw( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "hasNormal", NormalBuffer != null );
            shaderProgram.SetUniform( "hasTexCoord", TexCoordBuffer != null );
            shaderProgram.SetUniform( "hasTexCoord2", TexCoord2Buffer != null );
            shaderProgram.SetUniform( "hasColor", ColorBuffer != null );
            shaderProgram.SetUniform( "hasTangent", TangentBuffer != null );

            GL.BindVertexArray( VertexArrayID );

            foreach ( var indexTable in IndexTables )
                indexTable.Draw( shaderProgram );

            // Draw tangents
            //if ( TangentBuffer != null )
            //{
            //    shaderProgram.SetUniform( "hasNormal", false );
            //    shaderProgram.SetUniform( "hasTexCoord", false );
            //    shaderProgram.SetUniform( "hasColor", false );
            //    shaderProgram.SetUniform( "hasTangent", false );
            //    shaderProgram.SetUniform( "hasDiffuseTexture", false );
            //    shaderProgram.SetUniform( "specularColor", new Vector3( 0, 0, 0 ) );

            //    for ( int i = 0; i < TangentBuffer.Length; i++ )
            //    {

            //        var p = new Vector3( PositionBuffer.Array[ i ].X,
            //                             PositionBuffer.Array[ i ].Y,
            //                             PositionBuffer.Array[ i ].Z );

            //        var t = new Vector3( TangentBuffer.Array[ i ].X,
            //                             TangentBuffer.Array[ i ].Y,
            //                             TangentBuffer.Array[ i ].Z );

            //        var n = new Vector3( NormalBuffer.Array[ i ].X,
            //                             NormalBuffer.Array[ i ].Y,
            //                             NormalBuffer.Array[ i ].Z );

            //        t = Vector3.Normalize( t - n * Vector3.Dot( n, t ) );

            //        var b = Vector3.Normalize( Vector3.Cross( n, t ) );

            //        // Tangent
            //        shaderProgram.SetUniform( "diffuseColor", new Vector3( 1, 0, 0 ) );

            //        GL.Begin( PrimitiveType.Lines );
            //        GL.Vertex3( p );
            //        GL.Vertex3( p + t * 0.01f );
            //        GL.End();

            //        // Binormal
            //        shaderProgram.SetUniform( "diffuseColor", new Vector3( 0, 1, 0 ) );

            //        GL.Begin( PrimitiveType.Lines );
            //        GL.Vertex3( p );
            //        GL.Vertex3( p + b * 0.01f );
            //        GL.End();

            //        // Normal
            //        shaderProgram.SetUniform( "diffuseColor", new Vector3( 0, 0, 1 ) );

            //        GL.Begin( PrimitiveType.Lines );
            //        GL.Vertex3( p );
            //        GL.Vertex3( p + n * 0.01f );
            //        GL.End();
            //    }
            //}
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

            GL.DeleteVertexArray( VertexArrayID );
        }

        ~GLSubMesh()
        {
            Dispose( false );
        }

        public GLSubMesh( SubMesh subMesh, List<GLMaterial> materials )
        {
            if ( subMesh.Vertices == null )
                throw new ArgumentException( "Submesh must have vertices", nameof( subMesh ) );

            VertexArrayID = GL.GenVertexArray();
            GL.BindVertexArray( VertexArrayID );

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
