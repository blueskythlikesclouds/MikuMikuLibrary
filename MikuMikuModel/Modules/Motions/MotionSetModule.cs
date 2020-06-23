using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.Modules.Motions
{
    public class MotionSetModule : FormatModule<MotionSet>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Motion Set (Classic)", "bin", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( string fileName )
        {
            return Path.GetFileNameWithoutExtension( fileName )
                .StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) && base.Match( fileName );
        }

        public override MotionSet Import( string filePath )
        {
            var motion = new MotionSet();
            {
                motion.Load( filePath,
                    ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                    ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase );
            }
            return motion;
        }

        protected override MotionSet ImportCore( Stream source, string fileName )
        {
            var motion = new MotionSet();
            {
                motion.Load( source,
                    ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                    ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase, true );
            }
            return motion;
        }

        protected override void ExportCore( MotionSet model, Stream destination, string fileName )
        {
            model.Save( destination,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase, true );
        }
    }
}