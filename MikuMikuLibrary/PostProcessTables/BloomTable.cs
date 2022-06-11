// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.PostProcessTables;

public class BloomSetting
{
    public uint Flag { get; set; }
    public string Name { get; set; }
    public Vector3 Color { get; set; }
    public Vector3 Brightpass { get; set; }
    public float Range { get; set; }

    internal void Read(EndianBinaryReader reader, BinaryFormat format)
    {
        Flag = reader.ReadUInt32();
        Color = reader.ReadVector3();
        Brightpass = reader.ReadVector3();
        Range = reader.ReadSingle();
        if (format == BinaryFormat.X)
            reader.SeekCurrent(12);
    }

    internal void Write(EndianBinaryWriter writer, BinaryFormat format)
    {
        writer.Write(Flag);
        writer.Write(Color);
        writer.Write(Brightpass);
        writer.Write(Range);
        if (format == BinaryFormat.X)
            writer.WriteNulls(12);
    }
}

public class BloomTable : BinaryFile
{
    public override BinaryFileFlags Flags =>
        BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

    public List<BloomSetting> BloomTableEntries { get; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {

        if (Format == BinaryFormat.F2nd)
        {
            uint BloomSettingCount = reader.ReadUInt32();
            uint startOffset = reader.ReadUInt32();
            reader.ReadAtOffset(startOffset, () =>
            {
                for (int i = 0; i < BloomSettingCount; i++)
                {
                    BloomSetting nBloomSetting = new BloomSetting();
                    nBloomSetting.Read(reader, Format);
                    nBloomSetting.Name = $"Bloom Setting {i}";
                    BloomTableEntries.Add(nBloomSetting);
                }
            });
        }

        else if (Format == BinaryFormat.X)
        {
            ulong BloomSettingCount = reader.ReadUInt64();
            long startOffset = reader.ReadInt64();
            reader.ReadAtOffset(startOffset, () =>
            {
                for (ulong i = 0; i < BloomSettingCount; i++)
                {
                    BloomSetting nBloomSetting = new BloomSetting();
                    nBloomSetting.Read(reader, Format);
                    nBloomSetting.Name = $"Bloom Setting {i}";
                    BloomTableEntries.Add(nBloomSetting);
                }
            });
        }

    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        if (Format == BinaryFormat.F2nd)
        {
            writer.Write(BloomTableEntries.Count);
            writer.WriteOffset(() =>
            {
                foreach (var Setting in BloomTableEntries)
                    Setting.Write(writer, Format);
            });
            writer.WriteNulls(8);
        }
        else if (Format == BinaryFormat.X)
            throw new NotImplementedException("X writing not implemented yet");

    }

    public BloomTable()
    {
        BloomTableEntries = new List<BloomSetting>();
    }
}