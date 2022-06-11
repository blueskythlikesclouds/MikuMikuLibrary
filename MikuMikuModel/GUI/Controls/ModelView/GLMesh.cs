using MikuMikuLibrary.Objects;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView;

public class GLMesh : IDrawable
{
    private bool mDisposed;

    public int VertexArrayId { get; }

    public GLBuffer<Vector3> PositionBuffer { get; }
    public GLBuffer<Vector3> NormalBuffer { get; }
    public GLBuffer<Vector4> TangentBuffer { get; }
    public GLBuffer<Vector2> TexCoord0Buffer { get; }
    public GLBuffer<Vector2> TexCoord1Buffer { get; }
    public GLBuffer<Vector4> Color0Buffer { get; }

    public List<GLSubMesh> SubMeshes { get; }

    public void Submit(List<DrawCommand> opaqueCommands, List<DrawCommand> transparentCommands)
    {
        foreach (var subMesh in SubMeshes)
        {
            var drawCommand = new DrawCommand { Mesh = this, SubMesh = subMesh };

            if (subMesh.Material.IsTransparent)
                transparentCommands.Add(drawCommand);
            else
                opaqueCommands.Add(drawCommand);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private GLBuffer<T> GenerateVertexAttribute<T>(int index, T[] array) where T : unmanaged
    {
        var buffer = new GLBuffer<T>(BufferTarget.ArrayBuffer, array, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(index, buffer.Stride / sizeof(float), VertexAttribPointerType.Float, false, buffer.Stride, 0);
        GL.EnableVertexAttribArray(index);

        return buffer;
    }

    protected void Dispose(bool disposing)
    {
        if (mDisposed)
            return;

        if (disposing)
        {
            PositionBuffer?.Dispose();
            NormalBuffer?.Dispose();
            TangentBuffer?.Dispose();
            TexCoord0Buffer?.Dispose();
            TexCoord1Buffer?.Dispose();
            Color0Buffer?.Dispose();

            foreach (var subMesh in SubMeshes)
                subMesh.Dispose();
        }

        GL.DeleteVertexArray(VertexArrayId);
        GL.Finish();

        mDisposed = true;
    }

    ~GLMesh()
    {
        Dispose(false);
    }

    public GLMesh(Mesh mesh, List<GLMaterial> materials)
    {
        if (mesh.Positions == null)
            throw new ArgumentException("Submesh must have vertices", nameof(mesh));

        VertexArrayId = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayId);

        PositionBuffer = GenerateVertexAttribute(0, mesh.Positions);

        if (mesh.Normals != null)
            NormalBuffer = GenerateVertexAttribute(1, mesh.Normals);

        if (mesh.Tangents != null)
            TangentBuffer = GenerateVertexAttribute(2, mesh.Tangents);

        if (mesh.TexCoords0 != null)
            TexCoord0Buffer = GenerateVertexAttribute(3, mesh.TexCoords0);

        if (mesh.TexCoords1 != null)
            TexCoord1Buffer = GenerateVertexAttribute(4, mesh.TexCoords1);

        if (mesh.Colors0 != null)
            Color0Buffer = GenerateVertexAttribute(5, mesh.Colors0);

        SubMeshes = new List<GLSubMesh>();

        foreach (var subMesh in mesh.SubMeshes)
            SubMeshes.Add(new GLSubMesh(subMesh, materials));
    }
}