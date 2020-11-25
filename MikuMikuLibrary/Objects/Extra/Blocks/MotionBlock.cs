using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class MotionBlock : NodeBlock
    {
        public override string Signature => "MOT";

        public List<MotionBone> Bones { get; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            long nameOffset = reader.ReadOffset();
            int count = reader.ReadInt32();
            long boneNamesOffset = reader.ReadOffset();
            long boneMatricesOffset = reader.ReadOffset();

            Bones.Capacity = count;

            Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

            reader.ReadAtOffset( boneNamesOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    Bones.Add( new MotionBone { Name = stringSet.ReadString( reader ) } );
            } );

            reader.ReadAtOffset( boneMatricesOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    Bones[ i ].Transformation = reader.ReadMatrix4x4();
            } );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( Name );
            writer.Write( Bones.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var bone in Bones )
                    stringSet.WriteString( writer, bone.Name );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.Transformation );
            } );
        }

        public MotionBlock()
        {
            Bones = new List<MotionBone>();
        }
    }
}