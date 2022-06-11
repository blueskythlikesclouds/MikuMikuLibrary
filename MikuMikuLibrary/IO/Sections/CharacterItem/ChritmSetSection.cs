using MikuMikuLibrary.CharacterItem;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.CharacterItem;

[Section("ITEM")]
public class CharacterItemSection : BinaryFileSection<CharacterItemTable>
{
    public override SectionFlags Flags => SectionFlags.None;

    public override Encoding Encoding { get; } = Encoding.GetEncoding("utf-8");

    public CharacterItemSection(SectionMode mode, CharacterItemTable data = null) : base(mode, data)
    {
    }
}