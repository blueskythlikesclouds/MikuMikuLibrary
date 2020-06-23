using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.Motions;
using MikuMikuModel.Configurations;

namespace MikuMikuModel.Modules.Motions
{
    public class MotionModule : FormatModule<Motion>
    {
        public override IReadOnlyList<FormatExtension> Extensions { get; } = new[]
        {
            new FormatExtension( "Motion (Modern)", "mot", FormatExtensionFlags.Import | FormatExtensionFlags.Export )
        };

        public override bool Match( byte[] buffer )
        {
            return buffer[ 0 ] == 'M' && buffer[ 1 ] == 'O' && buffer[ 2 ] == 'T' && buffer[ 3 ] == 'C';
        }

        protected override Motion ImportCore( Stream source, string fileName )
        {
            var motion = new Motion();
            {
                motion.Load( source,
                    ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ], true );
            }
            return motion;
        }

        protected override void ExportCore( Motion model, Stream destination, string fileName )
        {
            model.Save( destination,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ], true );
        }
    }
}