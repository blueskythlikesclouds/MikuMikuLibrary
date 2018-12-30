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
            get { return new[] { "bin", "spr" }; }
        }

        protected override bool CanImportCore( Stream source, string fileName )
        {
            var sig = new byte[ 4 ];
            source.Read( sig, 0, 4 );

            return ( sig[ 0 ] == 'S' && sig[ 1 ] == 'P' && sig[ 2 ] == 'R' && sig[ 3 ] == 'C' ) ||
                ( sig[ 0 ] == 0 && sig[ 1 ] == 0 && sig[ 2 ] == 0 && sig[ 3 ] == 0 );
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
