using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Databases;

namespace MikuMikuLibrary.Motions
{
    public class MotionController
    {
        public Motion Parent { get; }
        public List<KeyController> KeyControllers { get; }

        public void Update( SkeletonEntry skeletonEntry, MotionDatabase motionDatabase = null )
        {
            Parent.KeySets.Clear();
            Parent.BoneInfos.Clear();

            foreach ( var keyController in KeyControllers )
            {
                if ( motionDatabase != null && !motionDatabase.BoneNames.Contains( keyController.Name ) )
                    continue;

                var boneEntry = skeletonEntry.GetBoneEntry( keyController.Name );
                if ( boneEntry != null )
                {
                    if ( boneEntry.Type != BoneType.Rotation )
                    {
                        Parent.KeySets.Add( keyController.Position?.X ?? new KeySet() );
                        Parent.KeySets.Add( keyController.Position?.Y ?? new KeySet() );
                        Parent.KeySets.Add( keyController.Position?.Z ?? new KeySet() );
                    }

                    if ( boneEntry.Type != BoneType.Position )
                    {
                        Parent.KeySets.Add( keyController.Rotation?.X ?? new KeySet() );
                        Parent.KeySets.Add( keyController.Rotation?.Y ?? new KeySet() );
                        Parent.KeySets.Add( keyController.Rotation?.Z ?? new KeySet() );
                    }
                }
                else if ( !skeletonEntry.BoneNames2.Contains( keyController.Name ) )
                {
                    Parent.KeySets.Add( keyController.Position?.X ?? new KeySet() );
                    Parent.KeySets.Add( keyController.Position?.Y ?? new KeySet() );
                    Parent.KeySets.Add( keyController.Position?.Z ?? new KeySet() );
                }

                Parent.BoneInfos.Add( new BoneInfo
                {
                    Name = keyController.Name,
                    Id = motionDatabase?.BoneNames?.IndexOf( keyController.Name ) ?? -1,
                } );
            }

            Parent.KeySets.Add( new KeySet() );
        }

        public void Merge( MotionController other )
        {
            foreach ( var keyController in other.KeyControllers )
            {
                var baseKeyController = KeyControllers.FirstOrDefault( x =>
                    x.Name.Equals( keyController.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( baseKeyController == null )
                    KeyControllers.Add( keyController );
                else
                    baseKeyController.Merge( keyController );
            }

            Parent.FrameCount = Math.Max( Parent.FrameCount, other.Parent.FrameCount );
        }

        public void Sort()
        {
            foreach ( var keyController in KeyControllers )
                keyController.Sort();
        }

        public MotionController( Motion parent )
        {
            Parent = parent;
            KeyControllers = new List<KeyController>();
        }
    }
}