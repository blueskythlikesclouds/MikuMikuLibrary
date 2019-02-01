using System;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Sprites;

namespace MikuMikuModel.Modules.Sprites
{
    public class SpriteSetModule : FormatModule<SpriteSet>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Sprite Set";
        public override string[] Extensions => new[] { "bin", "spr" };

        public override bool Match( string fileName ) =>
            fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase )
                ? fileName.StartsWith( "spr_", StringComparison.OrdinalIgnoreCase ) // TODO: Should this check be done?
                : base.Match( fileName );

        public override bool Match( byte[] buffer ) =>
            buffer[ 0 ] == 'S' && buffer[ 1 ] == 'P' && buffer[ 2 ] == 'R' && buffer[ 3 ] == 'C' ||
            buffer[ 0 ] == 0 && buffer[ 1 ] == 0 && buffer[ 2 ] == 0 && buffer[ 3 ] == 0;

        public override SpriteSet Import( string filePath ) => 
            BinaryFile.Load<SpriteSet>( filePath );

        protected override SpriteSet ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<SpriteSet>( source, true );

        protected override void ExportCore( SpriteSet obj, Stream destination, string fileName ) =>
            obj.Save( destination, true );
    }
}