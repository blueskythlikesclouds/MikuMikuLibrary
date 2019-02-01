using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Databases;

namespace MikuMikuLibrary.Motions
{
    public class MotionController
    {
        public string Name { get; set; }
        public int FrameCount { get; set; }
        public List<KeyController> KeyControllers { get; }

        public Motion ToMotion( SkeletonEntry skeletonEntry, MotionDatabase motionDatabase = null )
        {
            var motion = new Motion { FrameCount = FrameCount };
            {
                var keyControllers = motionDatabase != null
                    ? ( IEnumerable<KeyController> ) KeyControllers.OrderBy( x =>
                        motionDatabase.BoneNames.IndexOf( x.Target ) )
                    : KeyControllers;

                foreach ( var keyController in keyControllers )
                {
                    var boneEntry = skeletonEntry.GetBoneEntry( keyController.Target );
                    if ( boneEntry == null )
                        continue;

                    if ( boneEntry.Field00 >= 3 )
                    {
                        motion.KeySets.Add( keyController.Position?.X ?? new KeySet() );
                        motion.KeySets.Add( keyController.Position?.Y ?? new KeySet() );
                        motion.KeySets.Add( keyController.Position?.Z ?? new KeySet() );
                    }

                    motion.KeySets.Add( keyController.Rotation?.X ?? new KeySet() );
                    motion.KeySets.Add( keyController.Rotation?.Y ?? new KeySet() );
                    motion.KeySets.Add( keyController.Rotation?.Z ?? new KeySet() );

                    motion.BoneInfos.Add( new BoneInfo
                    {
                        Name = keyController.Target,
                        ID = motionDatabase?.BoneNames?.IndexOf( keyController.Target ) ?? -1,
                    } );
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