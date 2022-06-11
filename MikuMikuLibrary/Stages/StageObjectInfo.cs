using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Stages;

public struct StageObjectInfo
{
    public uint Id { get; set; }
    public uint SetId { get; set; }

    internal static StageObjectInfo ReadClassic(EndianBinaryReader reader)
    {
        StageObjectInfo objectInfo = default;
        objectInfo.Id = reader.ReadUInt16();
        objectInfo.SetId = reader.ReadUInt16();
        return objectInfo;
    }

    internal void WriteClassic(EndianBinaryWriter writer)
    {
        writer.Write((ushort)Id);
        writer.Write((ushort)SetId);
    }

    internal static StageObjectInfo ReadModern(EndianBinaryReader reader)
    {
        StageObjectInfo objectInfo = default;
        objectInfo.SetId = reader.ReadUInt32();
        objectInfo.Id = reader.ReadUInt32();
        return objectInfo;
    }

    internal void WriteModern(EndianBinaryWriter writer)
    {
        writer.Write(SetId);
        writer.Write(Id);
    }
}