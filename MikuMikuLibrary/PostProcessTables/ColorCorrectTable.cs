// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.PostProcessTables;

public class ColorCorrectSetting
{
    public string Name { get; set; }
    public float Hue { get; set; }
    public float Saturation { get; set; }
    public float Lightness { get; set; }
    public float Exposure { get; set; }
    public Vector3 Gamma { get; set; }
    public float Contrast { get; set; }
    public uint Flag { get; set; }

    internal void Read(EndianBinaryReader reader)
    {
        Flag = reader.ReadUInt32();
        Hue = reader.ReadSingle();
        Saturation = reader.ReadSingle();
        Lightness = reader.ReadSingle();
        Exposure = reader.ReadSingle();
        Gamma = reader.ReadVector3();
        Contrast = reader.ReadSingle();
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.Write(Flag);
        writer.Write(Hue);
        writer.Write(Saturation);
        writer.Write(Lightness);
        writer.Write(Exposure);
        writer.Write(Gamma);
        writer.Write(Contrast);
    }

}

public class ColorCorrectTable : BinaryFile
{
    public override BinaryFileFlags Flags =>
        BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

    public List<ColorCorrectSetting> ColorCorrectTableEntries { get; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        if (Format == BinaryFormat.F2nd)
        {
            uint ColorCorrectSettingCount = reader.ReadUInt32();
            uint startOffset = reader.ReadUInt32();

            reader.ReadAtOffset(startOffset, () =>
            {
                for (int i = 0; i < ColorCorrectSettingCount; i++)
                {
                    ColorCorrectSetting nColorCorrectSetting = new ColorCorrectSetting();
                    nColorCorrectSetting.Read(reader);
                    nColorCorrectSetting.Name = $"Color Correct Setting {i}";
                    ColorCorrectTableEntries.Add(nColorCorrectSetting);
                }
            });

        }

        else if (Format == BinaryFormat.X)
        {
            ulong ColorCorrectSettingCount = reader.ReadUInt64();
            long startOffset = reader.ReadInt64();

            reader.ReadAtOffset(startOffset, () =>
            {
                for (ulong i = 0; i < ColorCorrectSettingCount; i++)
                {
                    ColorCorrectSetting nColorCorrectSetting = new ColorCorrectSetting();
                    nColorCorrectSetting.Read(reader);
                    nColorCorrectSetting.Name = $"Color Correct Setting {i}";
                    ColorCorrectTableEntries.Add(nColorCorrectSetting);
                }
            });
        }
    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        if (Format == BinaryFormat.F2nd)
        {
            writer.Write(ColorCorrectTableEntries.Count);

            writer.WriteOffset(() =>
            {
                foreach (var Setting in ColorCorrectTableEntries)
                    Setting.Write(writer);
            });

            writer.WriteNulls(8);
        }

        else if (Format == BinaryFormat.X)
        {
            writer.Write(ColorCorrectTableEntries.Count);
            writer.WriteOffset(() =>
            {
                foreach (var Setting in ColorCorrectTableEntries)
                    Setting.Write(writer);
            });
        }
    }

    public ColorCorrectTable()
    {
        ColorCorrectTableEntries = new List<ColorCorrectSetting>();
    }
}