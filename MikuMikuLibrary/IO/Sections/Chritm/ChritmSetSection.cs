using System.Text;
using MikuMikuLibrary.Chritm;
using MikuMikuLibrary.IO.Sections.IO;

namespace MikuMikuLibrary.IO.Sections.Chritm
{
    [Section("ITEM")]
    public class AetSetSection : BinaryFileSection<CharacterItemTable>
    {
        public override SectionFlags Flags => SectionFlags.None;

        public override Encoding Encoding { get; } = Encoding.GetEncoding("utf-8");

        public AetSetSection(SectionMode mode, CharacterItemTable data = null) : base(mode, data)
        {
        }
    }
}