using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Stages;

public struct StageObjects
{
    public StageObjectInfo Ground { get; set; }
    public StageObjectInfo Unknown { get; set; }
    public StageObjectInfo Sky { get; set; }
    public StageObjectInfo Shadow { get; set; }
    public StageObjectInfo Reflect { get; set; }
    public StageObjectInfo Refract { get; set; }

    internal static StageObjects ReadClassic(EndianBinaryReader reader)
    {
        StageObjects objects = default;
        objects.Ground = StageObjectInfo.ReadClassic(reader);
        objects.Unknown = StageObjectInfo.ReadClassic(reader);
        objects.Sky = StageObjectInfo.ReadClassic(reader);
        objects.Shadow = StageObjectInfo.ReadClassic(reader);
        objects.Reflect = StageObjectInfo.ReadClassic(reader);
        objects.Refract = StageObjectInfo.ReadClassic(reader);
        return objects;
    }

    internal void WriteClassic(EndianBinaryWriter writer)
    {
        Ground.WriteClassic(writer);
        Unknown.WriteClassic(writer);
        Sky.WriteClassic(writer);
        Shadow.WriteClassic(writer);
        Reflect.WriteClassic(writer);
        Refract.WriteClassic(writer);
    }

    internal static StageObjects ReadModern(EndianBinaryReader reader)
    {
        StageObjects objects = default;
        objects.Ground = StageObjectInfo.ReadModern(reader);
        objects.Sky = StageObjectInfo.ReadModern(reader);
        objects.Shadow = StageObjectInfo.ReadModern(reader);
        objects.Reflect = StageObjectInfo.ReadModern(reader);
        objects.Refract = StageObjectInfo.ReadModern(reader);
        return objects;
    }

    internal void WriteModern(EndianBinaryWriter writer)
    {
        Ground.WriteModern(writer);
        Sky.WriteModern(writer);
        Shadow.WriteModern(writer);
        Reflect.WriteModern(writer);
        Refract.WriteModern(writer);
    }
}