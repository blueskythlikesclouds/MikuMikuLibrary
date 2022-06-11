using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Stages;

public class StageRefract
{
    public StageReflectRefractResolution Mode { get; set; }

    internal static StageRefract Read(EndianBinaryReader reader)
    {
        StageRefract refract = new StageRefract();
        refract.Mode = (StageReflectRefractResolution)reader.ReadInt32();
        return refract;
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.Write((int)Mode);
    }
}