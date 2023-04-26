using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Objects;
using MikuMikuLibrary.Numerics;

namespace MikuMikuLibrary.Objects;

[Flags]
public enum MeshFlags
{
    FaceCameraPosition = 1 << 1,
    FaceCameraView = 1 << 3
}

public class Mesh
{
    public BoundingSphere BoundingSphere { get; set; }
    public List<SubMesh> SubMeshes { get; }
    public Vector3[] Positions { get; set; }
    public Vector3[] Normals { get; set; }
    public Vector4[] Tangents { get; set; }
    public Vector2[] TexCoords0 { get; set; }
    public Vector2[] TexCoords1 { get; set; }
    public Vector2[] TexCoords2 { get; set; }
    public Vector2[] TexCoords3 { get; set; }
    public Vector4[] Colors0 { get; set; }
    public Vector4[] Colors1 { get; set; }
    public Vector4[] BlendWeights { get; set; }
    public Vector4Int[] BlendIndices { get; set; }
    public MeshFlags Flags { get; set; }
    public string Name { get; set; }

    public static int GetByteSize(BinaryFormat format)
    {
        switch (format)
        {
            case BinaryFormat.DT:
            case BinaryFormat.F:
            case BinaryFormat.FT:
            case BinaryFormat.F2nd:
                return 0xD8;

            case BinaryFormat.X:
                return 0x130;
        }

        throw new ArgumentException(nameof(format));
    }

    public Vector2[] GetTexCoordsChannel(int index)
    {
        switch (index)
        {
            case 0: return TexCoords0;
            case 1: return TexCoords1;
            case 2: return TexCoords2;
            case 3: return TexCoords3;

            default:
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public void SetTexCoordsChannel(int index, Vector2[] texCoords)
    {
        switch (index)
        {
            case 0:
                TexCoords0 = texCoords;
                break;

            case 1:
                TexCoords1 = texCoords;
                break;

            case 2:
                TexCoords2 = texCoords;
                break;

            case 3:
                TexCoords3 = texCoords;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public Vector4[] GetColorsChannel(int index)
    {
        switch (index)
        {
            case 0: return Colors0;
            case 1: return Colors1;

            default:
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public void SetColorsChannel(int index, Vector4[] colors)
    {
        switch (index)
        {
            case 0:
                Colors0 = colors;
                break;

            case 1:
                Colors1 = colors;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
    
    internal void Read(EndianBinaryReader reader, ObjectSection section = null)
    {
        reader.SeekCurrent(4); // Unused flags

        BoundingSphere = reader.ReadBoundingSphere();

        int subMeshCount = reader.ReadInt32();
        long subMeshesOffset = reader.ReadOffset();

        var vertexFormat = (VertexFormatAttributes)reader.ReadUInt32();
        int vertexSize = reader.ReadInt32();
        int vertexCount = reader.ReadInt32();
        var attributeOffsets = reader.ReadOffsets(20);

        Flags = (MeshFlags)reader.ReadInt32();

        uint attributeFlags = reader.ReadUInt32();

        reader.SkipNulls(6 * sizeof(uint));

        Name = reader.ReadString(StringBinaryFormat.FixedLength, 64);

        reader.ReadAtOffset(subMeshesOffset, () =>
        {
            SubMeshes.Capacity = subMeshCount;

            for (int i = 0; i < subMeshCount; i++)
            {
                var subMesh = new SubMesh();
                subMesh.Read(reader, section);
                SubMeshes.Add(subMesh);
            }
        });

        // Modern Format
        if ((vertexFormat & VertexFormatAttributes.UsesModernStorage) != 0)
            ReadVertexAttributesModern();

        else
            ReadVertexAttributesClassic();

        void ReadVertexAttributesClassic()
        {
            Vector4[] blendWeights = null;
            Vector4[] blendIndices = null;

            for (int i = 0; i < attributeOffsets.Length; i++)
            {
                var attribute = (VertexFormatAttributes)(1 << i);

                reader.ReadAtOffsetIf((vertexFormat & attribute) != 0, attributeOffsets[i], () =>
                {
                    switch (attribute)
                    {
                        case VertexFormatAttributes.Position:
                            Positions = reader.ReadVector3s(vertexCount);
                            break;

                        case VertexFormatAttributes.Normal:
                            Normals = reader.ReadVector3s(vertexCount);
                            break;

                        case VertexFormatAttributes.Tangent:
                            Tangents = reader.ReadVector4s(vertexCount);
                            break;

                        case VertexFormatAttributes.TexCoord0:
                            TexCoords0 = reader.ReadVector2s(vertexCount);
                            break;

                        case VertexFormatAttributes.TexCoord1:
                            TexCoords1 = reader.ReadVector2s(vertexCount);
                            break;

                        case VertexFormatAttributes.TexCoord2:
                            TexCoords2 = reader.ReadVector2s(vertexCount);
                            break;

                        case VertexFormatAttributes.TexCoord3:
                            TexCoords3 = reader.ReadVector2s(vertexCount);
                            break;

                        case VertexFormatAttributes.Color0:
                            Colors0 = reader.ReadVector4s(vertexCount);
                            break;

                        case VertexFormatAttributes.Color1:
                            Colors1 = reader.ReadVector4s(vertexCount);
                            break;

                        case VertexFormatAttributes.BlendWeight:
                            blendWeights = reader.ReadVector4s(vertexCount);
                            break;

                        case VertexFormatAttributes.BlendIndices:
                            blendIndices = reader.ReadVector4s(vertexCount);
                            break;

                        default:
                            Console.WriteLine("Unhandled vertex format element: {0}", attribute);
                            break;
                    }
                });
            }

            if (blendWeights != null && blendIndices != null)
            {
                BlendWeights = blendWeights;
                BlendIndices = new Vector4Int[vertexCount];

                for (int i = 0; i < vertexCount; i++)
                {
                    ref var weights = ref BlendWeights[i];
                    ref var indices = ref blendIndices[i];

                    weights = weights.NormalizeSum();

                    BlendIndices[i] = new Vector4Int
                    {
                        X = weights.X > 0.0f && indices.X >= 0.0f ? (int)(indices.X / 3.0f + 0.5f) : -1,
                        Y = weights.Y > 0.0f && indices.Y >= 0.0f ? (int)(indices.Y / 3.0f + 0.5f) : -1,
                        Z = weights.Z > 0.0f && indices.Z >= 0.0f ? (int)(indices.Z / 3.0f + 0.5f) : -1,
                        W = weights.W > 0.0f && indices.W >= 0.0f ? (int)(indices.W / 3.0f + 0.5f) : -1,
                    };
                }
            }
        }

        void ReadVertexAttributesModern()
        {
            Positions = new Vector3[vertexCount];
            Normals = new Vector3[vertexCount];
            Tangents = new Vector4[vertexCount];
            TexCoords0 = new Vector2[vertexCount];
            TexCoords1 = new Vector2[vertexCount];

            if (attributeFlags == 10)
            {
                TexCoords2 = new Vector2[vertexCount];
                TexCoords3 = new Vector2[vertexCount];
            }
            else if (attributeFlags == 6)
                TexCoords2 = new Vector2[vertexCount];

            Colors0 = new Vector4[vertexCount];

            if (attributeFlags == 4)
            {
                BlendWeights = new Vector4[vertexCount];
                BlendIndices = new Vector4Int[vertexCount];
            }

            bool hasTangents = false;

            EndianBinaryReader vertexReader;
            long baseOffset;

            if (section != null)
            {
                vertexReader = section.VertexData.Reader;
                baseOffset = section.VertexData.DataOffset;
            }
            else
            {
                vertexReader = reader;
                baseOffset = reader.BaseOffset;
            }

            long current = reader.Position;

            for (int i = 0; i < vertexCount; i++)
            {
                vertexReader.SeekBegin(baseOffset + attributeOffsets[13] + vertexSize * i);

                Positions[i] = vertexReader.ReadVector3();
                Normals[i] = vertexReader.ReadVector3(VectorBinaryFormat.Int16);
                vertexReader.SeekCurrent(2);
                Tangents[i] = vertexReader.ReadVector4(VectorBinaryFormat.Int16);
                TexCoords0[i] = vertexReader.ReadVector2(VectorBinaryFormat.Half);
                TexCoords1[i] = vertexReader.ReadVector2(VectorBinaryFormat.Half);

                if (attributeFlags == 10)
                {
                    TexCoords2[i] = vertexReader.ReadVector2(VectorBinaryFormat.Half);
                    TexCoords3[i] = vertexReader.ReadVector2(VectorBinaryFormat.Half);
                }
                else if (attributeFlags == 6)
                    TexCoords2[i] = vertexReader.ReadVector2(VectorBinaryFormat.Half);

                Colors0[i] = vertexReader.ReadVector4(VectorBinaryFormat.Half);

                if (attributeFlags == 4)
                {
                    ref var blendWeights = ref BlendWeights[i];
                    ref var blendIndices = ref BlendIndices[i];

                    blendWeights = vertexReader.ReadVector4(VectorBinaryFormat.Int16)
                        .NormalizeSum();

                    blendIndices = new Vector4Int(
                        reader.ReadByte() / 3,
                        reader.ReadByte() / 3,
                        reader.ReadByte() / 3, 
                        reader.ReadByte() / 3);

                    if (blendWeights.X <= 0.0f) blendIndices.X = -1;
                    if (blendWeights.Y <= 0.0f) blendIndices.Y = -1;
                    if (blendWeights.Z <= 0.0f) blendIndices.Z = -1;
                    if (blendWeights.W <= 0.0f) blendIndices.W = -1;
                }

                // Checks to get rid of useless data after reading
                if (Tangents[i] != Vector4.Zero) hasTangents = true;
            }

            if (!hasTangents) Tangents = null;

            reader.SeekBegin(current);
        }
    }

    internal void Write(EndianBinaryWriter writer, ObjectSection section = null)
    {
        writer.Write(0);
        writer.Write(BoundingSphere);
        writer.Write(SubMeshes.Count);

        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (var subMesh in SubMeshes)
                subMesh.Write(writer, section);
        });

        int vertexSize = 0;
        VertexFormatAttributes vertexFormat = default;

        if (section != null)
        {
            vertexFormat = VertexFormatAttributes.UsesModernStorage;
            vertexSize = BlendWeights != null ? 56 : 44;
        }

        else
        {
            if (Positions != null)
            {
                vertexFormat |= VertexFormatAttributes.Position;
                vertexSize += 12;
            }

            if (Normals != null)
            {
                vertexFormat |= VertexFormatAttributes.Normal;
                vertexSize += 12;
            }

            if (Tangents != null)
            {
                vertexFormat |= VertexFormatAttributes.Tangent;
                vertexSize += 16;
            }

            if (TexCoords0 != null)
            {
                vertexFormat |= VertexFormatAttributes.TexCoord0;
                vertexSize += 8;
            }

            if (TexCoords1 != null)
            {
                vertexFormat |= VertexFormatAttributes.TexCoord1;
                vertexSize += 8;
            }

            if (TexCoords2 != null)
            {
                vertexFormat |= VertexFormatAttributes.TexCoord2;
                vertexSize += 8;
            }

            if (TexCoords3 != null)
            {
                vertexFormat |= VertexFormatAttributes.TexCoord3;
                vertexSize += 8;
            }

            if (Colors0 != null)
            {
                vertexFormat |= VertexFormatAttributes.Color0;
                vertexSize += 16;
            }

            if (Colors1 != null)
            {
                vertexFormat |= VertexFormatAttributes.Color1;
                vertexSize += 16;
            }

            if (BlendWeights != null)
            {
                vertexFormat |= VertexFormatAttributes.BlendWeight | VertexFormatAttributes.BlendIndices;
                vertexSize += 32;
            }
        }

        writer.Write((int)vertexFormat);
        writer.Write(vertexSize);
        writer.Write(Positions.Length);

        if (section != null)
            WriteVertexAttributesModern();

        else
            WriteVertexAttributesClassic();

        writer.Write((int)Flags);

        writer.Write(section != null ? (BlendWeights != null ? 4 : 2) : 0);

        writer.WriteNulls(6 * sizeof(uint)); // Reserved

        writer.Write(Name, StringBinaryFormat.FixedLength, 64);

        void WriteVertexAttributesClassic()
        {
            for (int i = 0; i < 20; i++)
            {
                var attribute = (VertexFormatAttributes)(1 << i);

                writer.WriteOffsetIf((vertexFormat & attribute) != 0, (int)attribute + 1, 4, AlignmentMode.Left, () =>
                {
                    switch (attribute)
                    {
                        case VertexFormatAttributes.Position:
                            writer.Write(Positions);
                            break;

                        case VertexFormatAttributes.Normal:
                            writer.Write(Normals);
                            break;

                        case VertexFormatAttributes.Tangent:
                            writer.Write(Tangents);
                            break;

                        case VertexFormatAttributes.TexCoord0:
                            writer.Write(TexCoords0);
                            break;

                        case VertexFormatAttributes.TexCoord1:
                            writer.Write(TexCoords1);
                            break;

                        case VertexFormatAttributes.TexCoord2:
                            writer.Write(TexCoords2);
                            break;

                        case VertexFormatAttributes.TexCoord3:
                            writer.Write(TexCoords3);
                            break;

                        case VertexFormatAttributes.Color0:
                            writer.Write(Colors0);
                            break;

                        case VertexFormatAttributes.Color1:
                            writer.Write(Colors1);
                            break;

                        case VertexFormatAttributes.BlendWeight:
                            writer.Write(BlendWeights);
                            break;

                        case VertexFormatAttributes.BlendIndices:
                            foreach (ref var indices in BlendIndices.AsSpan())
                            {
                                writer.Write(indices.X < 0 ? -1.0f : indices.X * 3.0f);
                                writer.Write(indices.Y < 0 ? -1.0f : indices.Y * 3.0f);
                                writer.Write(indices.Z < 0 ? -1.0f : indices.Z * 3.0f);
                                writer.Write(indices.W < 0 ? -1.0f : indices.W * 3.0f);
                            }

                            break;
                    }
                });
            }
        }

        void WriteVertexAttributesModern()
        {
            int byteSize = writer.AddressSpace.GetByteSize();

            writer.Align(byteSize);
            writer.WriteNulls(13 * byteSize);
            writer.WriteOffset(section.VertexData.AddSubMesh(this, vertexSize));
            writer.WriteNulls(6 * byteSize);
        }
    }

    public Mesh()
    {
        SubMeshes = new List<SubMesh>();
    }

    [Flags]
    private enum VertexFormatAttributes
    {
        Position = 1 << 0,
        Normal = 1 << 1,
        Tangent = 1 << 2,
        TexCoord0 = 1 << 4,
        TexCoord1 = 1 << 5,
        TexCoord2 = 1 << 6,
        TexCoord3 = 1 << 7,
        Color0 = 1 << 8,
        Color1 = 1 << 9,
        BlendWeight = 1 << 10,
        BlendIndices = 1 << 11,
        UsesModernStorage = 1 << 31
    }
}