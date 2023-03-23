using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Text;

public class TextFile : BinaryFile
{
    public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

    public string Text { get; set; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        var bytes = new byte[reader.Length];
        reader.Read(bytes);
        Text = Encoding.UTF8.GetString(bytes);
    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        writer.Write(Encoding.UTF8.GetBytes(Text));
    }
}