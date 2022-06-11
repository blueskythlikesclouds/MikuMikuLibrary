using MikuMikuLibrary.Objects;
using OpenTK.Graphics.OpenGL;

using PrimitiveType = MikuMikuLibrary.Objects.PrimitiveType;

namespace MikuMikuModel.GUI.Controls.ModelView;

public class GLSubMesh
{
    public Vector3 Center { get; }
    public GLBuffer<uint> ElementBuffer { get; }
    public PrimitiveType PrimitiveType { get; }
    public GLMaterial Material { get; }

    public void Dispose()
    {
        ElementBuffer.Dispose();
        Material.Dispose();
    }

    public GLSubMesh(SubMesh subMesh, List<GLMaterial> materials)
    {
        Center = subMesh.BoundingSphere.Center;
        ElementBuffer = new GLBuffer<uint>(BufferTarget.ElementArrayBuffer, subMesh.Indices,
            BufferUsageHint.StaticDraw);

        PrimitiveType = (PrimitiveType)subMesh.PrimitiveType;
        Material = materials[(int)subMesh.MaterialIndex];
    }
}