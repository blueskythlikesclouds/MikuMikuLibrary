// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.PostProcessTables;

public class DOFSetting
{
    public string Name { get; set; }
    public uint SettingFlags { get; set; }
    public float Focus { get; set; }
    public float FocusRange { get; set; }
    public float FuzzingRange { get; set; }
    public float Ratio { get; set; }
    public float Quality { get; set; }

    internal void Read(EndianBinaryReader reader)
    {
        SettingFlags = reader.ReadUInt32();
        Focus = reader.ReadSingle();
        FocusRange = reader.ReadSingle();
        FuzzingRange = reader.ReadSingle();
        Ratio = reader.ReadSingle();
        Quality = reader.ReadSingle();
        Name = "";
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.Write(SettingFlags);
        writer.Write(Focus);
        writer.Write(FocusRange);
        writer.Write(FuzzingRange);
        writer.Write(Ratio);
        writer.Write(Quality);
    }

}

public class DOFTable : BinaryFile
{
    public override BinaryFileFlags Flags =>
        BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

    public List<DOFSetting> DOFEntries { get; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        uint DOFSettingCount = reader.ReadUInt32();
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(4);
        uint DOFSettingOffset = reader.ReadUInt32();
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(4);

        reader.ReadAtOffset(DOFSettingOffset, () =>
        {
            for (int i = 0; i < DOFSettingCount; i++)
            {
                DOFSetting DofEntry = new DOFSetting();
                DofEntry.Read(reader);
                DofEntry.Name = $"DOF Setting {i}";
                DOFEntries.Add(DofEntry);
                if (Format == BinaryFormat.X)
                {
                    reader.SeekCurrent(4);
                }
            }
        });

    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        writer.Write(DOFEntries.Count);
        writer.WriteOffset(16, AlignmentMode.Left, () =>
        {
            foreach (var DOFSetting in DOFEntries)
            {
                DOFSetting.Write(writer);
            }
        });
    }

    public DOFTable()
    {
        DOFEntries = new List<DOFSetting>();
    }
}