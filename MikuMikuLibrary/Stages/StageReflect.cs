using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Stages;

public class StageReflect
{
    public StageReflectRefractResolution ResolutionMode { get; set; }
    public int BlurNum { get; set; }
    public StageReflectBlurFilter BlurFilter { get; set; }

    internal static StageReflect Read(EndianBinaryReader reader)
    {
        StageReflect reflect = new StageReflect();
        reflect.ResolutionMode = (StageReflectRefractResolution)reader.ReadInt32();
        reflect.BlurNum = reader.ReadInt32();
        reflect.BlurFilter = (StageReflectBlurFilter)reader.ReadInt32();
        return reflect;
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.Write((int)ResolutionMode);
        writer.Write(BlurNum);
        writer.Write((int)BlurFilter);
    }
}