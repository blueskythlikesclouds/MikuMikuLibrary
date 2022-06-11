// Code by Thatrandomlurker

using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.PostProcessTables;

public class LightSetting
{
    public string Name { get; set; }
    public uint ID { get; set; }
    public uint LightFlags { get; set; }
    public uint Type { get; set; }
    public Vector4 Ambient { get; set; }
    public Vector4 Diffuse { get; set; }
    public Vector4 Specular { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 ToneCurve { get; set; }

    internal void Read(EndianBinaryReader reader, BinaryFormat Format)
    {
        ID = reader.ReadUInt32();
        LightFlags = reader.ReadUInt32();
        Type = reader.ReadUInt32();
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(24);
        Ambient = reader.ReadVector4();
        Diffuse = reader.ReadVector4();
        Specular = reader.ReadVector4();
        Position = reader.ReadVector3();
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(28);
        ToneCurve = reader.ReadVector3();
        Name = "";
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(16);
    }

    internal void Write(EndianBinaryWriter writer, BinaryFormat Format)
    {
        writer.Write(ID);
        writer.Write(LightFlags);
        writer.Write(Type);
        if (Format == BinaryFormat.X)
            writer.WriteNulls(24);
        writer.Write(Ambient);
        writer.Write(Diffuse);
        writer.Write(Specular);
        writer.Write(Position);
        if (Format == BinaryFormat.X)
            writer.WriteNulls(28);
        writer.Write(ToneCurve);
        if (Format == BinaryFormat.X)
            writer.WriteNulls(16);
    }
}

public class LightSection
{
    public string Name { get; set; }
    public List<LightSetting> LightSettings { get; }

    internal void Read(EndianBinaryReader reader, BinaryFormat Format)
    {
        uint LightSettingCount = reader.ReadUInt32();
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(4);
        uint LightSettingOffset = reader.ReadUInt32();
        if (Format == BinaryFormat.X)
            reader.SeekCurrent(12);

        reader.ReadAtOffset(LightSettingOffset, () =>
        {
            for (int i = 0; i < LightSettingCount; i++)
            {
                LightSetting LightEntry = new LightSetting();
                LightEntry.Read(reader, Format);
                LightEntry.Name = $"Light Setting {i}";
                LightSettings.Add(LightEntry);
            }
        });
    }

    internal void Write(EndianBinaryWriter writer, BinaryFormat Format)
    {
        writer.Write(LightSettings.Count);
        writer.WriteOffset(16, AlignmentMode.Left, () =>
        {
            foreach (var LightSetting in LightSettings)
            {
                LightSetting.Write(writer, Format);
            }
        });
    }

    public LightSection()
    {
        LightSettings = new List<LightSetting>();
    }
}

public class LightTable : BinaryFile
{
    public override BinaryFileFlags Flags =>
        BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

    public uint Unk { get; set; }

    public List<LightSection> LightSections { get; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        Unk = reader.ReadUInt32();
        uint LightSectionCount = reader.ReadUInt32();
        uint LightSectionsOffset = reader.ReadUInt32();

        reader.ReadAtOffset(LightSectionsOffset, () =>
        {
            for (int i = 0; i < LightSectionCount; i++)
            {
                LightSection LightTable = new LightSection();
                LightTable.Read(reader, Format);
                LightTable.Name = $"Light Section {i}";
                LightSections.Add(LightTable);
            }
        });

    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        writer.Write(Unk);
        writer.Write(LightSections.Count);
        writer.WriteOffset(16, AlignmentMode.Left, () =>
        {
            foreach (var LightSection in LightSections)
            {
                LightSection.Write(writer, Format);
            }
        });
    }

    public LightTable()
    {
        LightSections = new List<LightSection>();
    }
}