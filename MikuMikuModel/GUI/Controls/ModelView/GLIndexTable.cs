using MikuMikuLibrary.Models;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

using GLPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using MMLPrimitiveType = MikuMikuLibrary.Models.PrimitiveType;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLIndexTable : IGLDraw
    {
        public GLBuffer<ushort> ElementBuffer { get; }
        public GLPrimitiveType PrimitiveType { get; }
        public GLMaterial Material { get; }

        public void Draw( GLShaderProgram shaderProgram )
        {
            Material.Bind( shaderProgram );
            ElementBuffer.Bind();

            GL.DrawElements( PrimitiveType, ElementBuffer.Length, DrawElementsType.UnsignedShort, 0 );
        }

        public void Dispose()
        {
            ElementBuffer.Dispose();
            Material.Dispose();
        }

        public GLIndexTable( IndexTable indexTable, List<GLMaterial> materials )
        {
            ElementBuffer = new GLBuffer<ushort>( BufferTarget.ElementArrayBuffer, indexTable.Indices, sizeof( ushort ), BufferUsageHint.StaticDraw );

            if ( indexTable.PrimitiveType == MMLPrimitiveType.TriangleStrip )
                PrimitiveType = GLPrimitiveType.TriangleStrip;
            else
                PrimitiveType = GLPrimitiveType.Triangles;

            Material = materials[ indexTable.MaterialIndex ];
        }
    }
}
