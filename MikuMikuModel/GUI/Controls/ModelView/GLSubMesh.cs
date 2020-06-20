using System.Collections.Generic;
using MikuMikuLibrary.Objects;
using OpenTK.Graphics.OpenGL;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLSubMesh : IDrawable
    {
        public GLBuffer<uint> ElementBuffer { get; }
        public PrimitiveType PrimitiveType { get; }
        public GLMaterial Material { get; }

        public void Draw( GLShaderProgram shaderProgram )
        {
            Material.Bind( shaderProgram );
            ElementBuffer.Bind();

            GL.DrawElements( PrimitiveType, ElementBuffer.Length, DrawElementsType.UnsignedInt, 0 );
        }

        public void Dispose()
        {
            ElementBuffer.Dispose();
            Material.Dispose();
        }

        public GLSubMesh( SubMesh subMesh, List<GLMaterial> materials )
        {
            ElementBuffer = new GLBuffer<uint>( BufferTarget.ElementArrayBuffer, subMesh.Indices,
                BufferUsageHint.StaticDraw );

            PrimitiveType = ( PrimitiveType ) subMesh.PrimitiveType;
            Material = materials[ ( int ) subMesh.MaterialIndex ];
        }
    }
}