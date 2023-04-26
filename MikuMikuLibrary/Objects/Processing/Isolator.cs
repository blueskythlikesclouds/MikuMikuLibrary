using MikuMikuLibrary.Numerics;

namespace MikuMikuLibrary.Objects.Processing;

public class Isolator
{
    public static void Isolate(Mesh mesh)
    {
        foreach (var subMesh in mesh.SubMeshes)
        {
            if (subMesh.PrimitiveType == PrimitiveType.TriangleStrip)
            {
                subMesh.PrimitiveType = PrimitiveType.Triangles;
                subMesh.Indices = Stripifier.Unstripify(subMesh.Indices);
            }
        }

        int vertexCount = mesh.SubMeshes.Sum(x => x.Indices.Length);

        var positions = mesh.Positions != null ? new Vector3[vertexCount] : null;
        var normals = mesh.Normals != null ? new Vector3[vertexCount] : null;
        var tangents = mesh.Tangents != null ? new Vector4[vertexCount] : null;
        var texCoords0 = mesh.TexCoords0 != null ? new Vector2[vertexCount] : null;
        var texCoords1 = mesh.TexCoords1 != null ? new Vector2[vertexCount] : null;
        var texCoords2 = mesh.TexCoords2 != null ? new Vector2[vertexCount] : null;
        var texCoords3 = mesh.TexCoords3 != null ? new Vector2[vertexCount] : null;
        var colors0 = mesh.Colors0 != null ? new Vector4[vertexCount] : null;
        var colors1 = mesh.Colors1 != null ? new Vector4[vertexCount] : null;
        var blendWeights = mesh.BlendWeights != null ? new Vector4[vertexCount] : null;
        var blendIndices = mesh.BlendIndices != null ? new Vector4Int[vertexCount] : null;

        uint nextIndex = 0;

        foreach (var subMesh in mesh.SubMeshes)
        {
            for (int i = 0; i < subMesh.Indices.Length; i++)
            {
                uint index = subMesh.Indices[i];
                subMesh.Indices[i] = nextIndex;

                if (positions != null) positions[nextIndex] = mesh.Positions[index];
                if (normals != null) normals[nextIndex] = mesh.Normals[index];
                if (tangents != null) tangents[nextIndex] = mesh.Tangents[index];
                if (texCoords0 != null) texCoords0[nextIndex] = mesh.TexCoords0[index];
                if (texCoords1 != null) texCoords1[nextIndex] = mesh.TexCoords1[index];
                if (texCoords2 != null) texCoords2[nextIndex] = mesh.TexCoords2[index];
                if (texCoords3 != null) texCoords3[nextIndex] = mesh.TexCoords3[index];
                if (colors0 != null) colors0[nextIndex] = mesh.Colors0[index];
                if (colors1 != null) colors1[nextIndex] = mesh.Colors1[index];
                if (blendWeights != null) blendWeights[nextIndex] = mesh.BlendWeights[index];
                if (blendIndices != null) blendIndices[nextIndex] = mesh.BlendIndices[index];

                ++nextIndex;
            }
        }

        mesh.Positions = positions;
        mesh.Normals = normals;
        mesh.Tangents = tangents;
        mesh.TexCoords0 = texCoords0;
        mesh.TexCoords1 = texCoords1;
        mesh.TexCoords2 = texCoords2;
        mesh.TexCoords3 = texCoords3;
        mesh.Colors0 = colors0;
        mesh.Colors1 = colors1;
        mesh.BlendWeights = blendWeights;
        mesh.BlendIndices = blendIndices;
    }

    public static void Isolate(Object obj)
    {
        foreach (var mesh in obj.Meshes)
            Isolate(mesh);
    }
}