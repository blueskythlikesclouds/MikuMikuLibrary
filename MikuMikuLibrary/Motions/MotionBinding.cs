using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Skeletons;

namespace MikuMikuLibrary.Motions
{
    public class MotionBinding
    {
        public Motion Parent { get; }

        public List<BoneBinding> BoneBindings { get; }
        public BoneBinding GlobalTransformation { get; }

        public void Unbind( Skeleton skeleton, MotionDatabase motionDatabase = null )
        {
            Parent.KeySets.Clear();
            Parent.BoneInfos.Clear();

            foreach ( var boneBinding in skeleton.MotionBoneNames
                .Where( x =>
                    motionDatabase == null || motionDatabase.BoneNames.Contains( x, StringComparer.OrdinalIgnoreCase ) )
                .Select( x =>
                    BoneBindings.Find( y => y.Name.Equals( x, StringComparison.OrdinalIgnoreCase ) ) ??
                    new BoneBinding { Name = x } ).OrderByDescending( x => x.Name ) )
            {
                var bone = skeleton.GetBone( boneBinding.Name );
                if ( bone != null )
                {
                    if ( bone.Type != BoneType.Rotation )
                        UnbindPosition( boneBinding );

                    if ( bone.Type != BoneType.Position )
                        UnbindRotation( boneBinding );
                }

                AddBone( boneBinding.Name );
            }

            AddBone( "gblctr" );
            UnbindPosition( GlobalTransformation );

            AddBone( "kg_ya_ex" );
            UnbindRotation( GlobalTransformation );

            Parent.KeySets.Add( new KeySet() );

            void UnbindPosition( BoneBinding boneBinding )
            {
                Parent.KeySets.Add( boneBinding.Position?.X ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Position?.Y ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Position?.Z ?? new KeySet() );
            }

            void UnbindRotation( BoneBinding boneBinding )
            {
                Parent.KeySets.Add( boneBinding.Rotation?.X ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Rotation?.Y ?? new KeySet() );
                Parent.KeySets.Add( boneBinding.Rotation?.Z ?? new KeySet() );
            }

            void AddBone( string name )
            {
                Parent.BoneInfos.Add( new BoneInfo
                {
                    Name = name,
                    Id = motionDatabase?.BoneNames?.FindIndex(
                             x => x.Equals( name, StringComparison.OrdinalIgnoreCase ) ) ?? -1
                } );
            }
        }

        public void Merge( MotionBinding other )
        {
            foreach ( var boneBinding in other.BoneBindings )
            {
                var baseBoneBinding = BoneBindings.FirstOrDefault( x =>
                    x.Name.Equals( boneBinding.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( baseBoneBinding == null )
                    BoneBindings.Add( boneBinding );
                else
                    baseBoneBinding.Merge( boneBinding );
            }

            Parent.FrameCount = Math.Max( Parent.FrameCount, other.Parent.FrameCount );
        }

        public void Sort()
        {
            foreach ( var boneBinding in BoneBindings )
                boneBinding.Sort();
        }

        public MotionBinding( Motion parent )
        {
            Parent = parent;
            BoneBindings = new List<BoneBinding>();
            GlobalTransformation = new BoneBinding();
        }
    }
}