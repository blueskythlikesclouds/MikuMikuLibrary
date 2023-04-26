namespace MikuMikuLibrary.Objects.Processing;

public class Splitter
{
    public const int MAX_VERTICES = 0xFFFF;
    public const int MAX_BONES = 0x40;

    public static void Split(Object obj)
    {
        var vertexIndices = new HashSet<uint>();
        var boneIndices = new HashSet<ushort>();

        for (int meshIndex = 0; meshIndex < obj.Meshes.Count; meshIndex++)
        {
            var mesh = obj.Meshes[meshIndex];

            bool canSkip = mesh.Positions.Length <= MAX_VERTICES &&
                           mesh.SubMeshes.All(x => x.BoneIndices == null || x.BoneIndices.Length <= MAX_BONES);

            if (canSkip)
                continue;

            foreach (var subMesh in mesh.SubMeshes)
            {
                if (subMesh.PrimitiveType == PrimitiveType.TriangleStrip)
                {
                    subMesh.PrimitiveType = PrimitiveType.Triangles;
                    subMesh.Indices = Stripifier.Unstripify(subMesh.Indices);
                }

                for (int i = 0; i < subMesh.Indices.Length; i += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        vertexIndices.Add(subMesh.Indices[i + j]);

                        if (mesh.BlendIndices != null)
                        {
                            ref var blendIndices = ref mesh.BlendIndices[subMesh.Indices[i + j]];

                            if (blendIndices.X >= 0) boneIndices.Add(subMesh.BoneIndices[blendIndices.X]);
                            if (blendIndices.Y >= 0) boneIndices.Add(subMesh.BoneIndices[blendIndices.Y]);
                            if (blendIndices.Z >= 0) boneIndices.Add(subMesh.BoneIndices[blendIndices.Z]);
                            if (blendIndices.W >= 0) boneIndices.Add(subMesh.BoneIndices[blendIndices.W]);
                        }
                    }

                    if (vertexIndices.Count > MAX_VERTICES || boneIndices.Count > MAX_BONES)
                    {
                        var meshCopy = new Mesh
                        {
                            BoundingSphere = mesh.BoundingSphere,
                            Positions = mesh.Positions,
                            Normals = mesh.Normals,
                            Tangents = mesh.Tangents,
                            TexCoords0 = mesh.TexCoords0,
                            TexCoords1 = mesh.TexCoords1,
                            TexCoords2 = mesh.TexCoords2,
                            TexCoords3 = mesh.TexCoords3,
                            Colors0 = mesh.Colors0,
                            Colors1 = mesh.Colors1,
                            BlendWeights = mesh.BlendWeights,
                            BlendIndices = mesh.BlendIndices,
                            Flags = mesh.Flags,
                            Name = mesh.Name
                        };

                        var subMeshCopy = new SubMesh
                        {
                            BoundingSphere = subMesh.BoundingSphere,
                            MaterialIndex = subMesh.MaterialIndex,
                            TexCoordIndices = subMesh.TexCoordIndices,
                            BoneIndices = subMesh.BoneIndices,
                            BonesPerVertex = subMesh.BonesPerVertex,
                            PrimitiveType = subMesh.PrimitiveType,
                            IndexFormat = subMesh.IndexFormat,
                            Flags = subMesh.Flags,
                            IndexOffset = subMesh.IndexOffset,
                            BoundingBox = subMesh.BoundingBox
                        };

                        meshCopy.SubMeshes.Add(subMeshCopy);

                        subMeshCopy.Indices = subMesh.Indices.AsSpan(i).ToArray();
                        subMesh.Indices = subMesh.Indices.AsSpan(0, i).ToArray();

                        obj.Meshes.Add(meshCopy);

                        break;
                    }
                }
                boneIndices.Clear();
            }
            vertexIndices.Clear();
        }

        // Fix duplicate names and remove empty meshes.
        var meshNames = new Dictionary<string, int>();

        foreach (var mesh in obj.Meshes)
        {
            if (meshNames.TryGetValue(mesh.Name, out int index))
            {
                ++meshNames[mesh.Name];
                mesh.Name += $".{index:D3}";
            }
            else
            {
                meshNames[mesh.Name] = 1;
            }

            mesh.SubMeshes.RemoveAll(x => x.Indices == null || x.Indices.Length == 0);
        }

        obj.Meshes.RemoveAll(x => x.SubMeshes.Count == 0);
    }
}