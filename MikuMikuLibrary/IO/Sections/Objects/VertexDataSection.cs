using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects;

[Section("OVTX")]
public class VertexDataSection : Section<object>
{
    private readonly List<Mesh> mMeshes;
    private long mCurrentOffset;

    public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;

    public long AddSubMesh(Mesh mesh, int stride)
    {
        long current = mCurrentOffset;
        {
            mMeshes.Add(mesh);
            mCurrentOffset += mesh.Positions.Length * stride;
            mCurrentOffset = AlignmentHelper.Align(mCurrentOffset, 4);
        }

        return current;
    }

    protected override void Read(object data, EndianBinaryReader reader, long length)
    {
    }

    protected override void Write(object data, EndianBinaryWriter writer)
    {
        foreach (var mesh in mMeshes)
        {
            for (int i = 0; i < mesh.Positions.Length; i++)
            {
                writer.Write((Vector3)mesh.Positions?[i]);
                writer.Write(mesh.Normals?[i] ?? Vector3.Zero, VectorBinaryFormat.Int16);
                writer.WriteNulls(sizeof(ushort));
                writer.Write(mesh.Tangents?[i] ?? Vector4.Zero, VectorBinaryFormat.Int16);
                writer.Write(mesh.TexCoords0?[i] ?? Vector2.Zero, VectorBinaryFormat.Half);
                writer.Write(mesh.TexCoords1?[i] ?? mesh.TexCoords0?[i] ?? Vector2.Zero, VectorBinaryFormat.Half);
                writer.Write(mesh.Colors0?[i] ?? Vector4.One, VectorBinaryFormat.Half);

                if (mesh.BlendWeights == null)
                    continue;

                ref var blendWeights = ref mesh.BlendWeights[i];
                ref var blendIndices = ref mesh.BlendIndices[i];

                writer.Write((ushort)(blendIndices.X >= 0 ? blendWeights.X * 32767f : 0));
                writer.Write((ushort)(blendIndices.Y >= 0 ? blendWeights.Y * 32767f : 0));
                writer.Write((ushort)(blendIndices.Z >= 0 ? blendWeights.Z * 32767f : 0));
                writer.Write((ushort)(blendIndices.W >= 0 ? blendWeights.W * 32767f : 0));

                writer.Write((byte)(blendIndices.X >= 0 ? blendIndices.X * 3 : 0));
                writer.Write((byte)(blendIndices.Y >= 0 ? blendIndices.Y * 3 : 0));
                writer.Write((byte)(blendIndices.Z >= 0 ? blendIndices.Z * 3 : 0));
                writer.Write((byte)(blendIndices.W >= 0 ? blendIndices.W * 3 : 0));
            }
        }
    }

    public VertexDataSection(SectionMode mode, object data = null) : base(mode, data)
    {
        if (mode == SectionMode.Write)
            mMeshes = new List<Mesh>();
    }
}