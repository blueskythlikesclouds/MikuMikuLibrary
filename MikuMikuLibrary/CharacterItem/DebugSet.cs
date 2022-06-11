using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.CharacterItem;

public class DebugSet
{
    public ulong ID { get; set; }
    public string Name { get; set; }
    public List<int> Parts { get; }

    internal void Read(EndianBinaryReader reader)
    {
        ID = reader.ReadUInt64();
        Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
        for (int i = 0; i < 25; i++)
        {
            Parts.Add(reader.ReadInt32());
        }
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.Write(ID);
        writer.WriteStringOffset(Name);
        for (int i = 0; i < 25; i++)
        {
            writer.Write(Parts[i]);
        }
    }

    public DebugSet()
    {
        Parts = new List<int>();
    }
}