using MikuMikuLibrary.Sprites;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public class SpriteSetFormatModule : FormatModule<SpriteSet>
    {
        public override FormatModuleFlags Flags
        {
            get { return FormatModuleFlags.Import | FormatModuleFlags.Export; }
        }

        public override string Name
        {
            get { return "Sprite Container"; }
        }

        public override string[] Extensions
        {
            get { return new[] { "bin" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            return source.ReadByte() == 0 && source.ReadByte() == 0 && source.ReadByte() == 0 && source.ReadByte() == 0;
        }

        protected override void ExportCore( SpriteSet obj, Stream destination, string fileName )
        {
            obj.Save( destination, true );
        }

        protected override SpriteSet ImportCore( Stream source, string fileName )
        {
            return SpriteSet.Load<SpriteSet>( source, true );
        }
    }
}
