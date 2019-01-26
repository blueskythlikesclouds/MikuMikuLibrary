using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Databases;

namespace MikuMikuLibrary.Motions
{
    public class MotionController
    {
        public int FrameCount { get; set; }
        public List<KeyController> KeyControllers { get; }

        public Motion ToMotion( SkeletonEntry skeletonEntry, MotionDatabase motionDatabase )
        {
            var motion = new Motion { FrameCount = FrameCount };
            {
                var boneNames = motionDatabase.BoneNames;
                foreach ( var keyController in KeyControllers.OrderBy( x => boneNames.IndexOf( x.Target ) ) )
                {
                    var boneEntry = skeletonEntry.GetBoneEntry( keyController.Target );
                    if ( boneEntry == null )
                        continue;

                    motion.BoneIndices.Add( boneNames.IndexOf( keyController.Target ) );

                    if ( boneEntry.Field00 >= 3 )
                    {
                        motion.KeySets.Add( keyController.Position?.X ?? new KeySet() );
                        motion.KeySets.Add( keyController.Position?.Y ?? new KeySet() );
                        motion.KeySets.Add( keyController.Position?.Z ?? new KeySet() );
                    }

                    motion.KeySets.Add( keyController.Rotation?.X ?? new KeySet() );
                    motion.KeySets.Add( keyController.Rotation?.Y ?? new KeySet() );
                    motion.KeySets.Add( keyController.Rotation?.Z ?? new KeySet() );
                }
            }
            return motion;
        }

        public MotionController()
        {
            KeyControllers = new List<KeyController>();
        }
    }
}