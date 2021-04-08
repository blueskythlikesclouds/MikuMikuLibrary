using System.Collections.Generic;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class MotionBlock : NodeBlock
    {
        public override string Signature => "MOT";

        public List<MotionNode> Nodes { get; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            long nameOffset = reader.ReadOffset();
            int count = reader.ReadInt32();
            long boneNamesOffset = reader.ReadOffset();
            long boneMatricesOffset = reader.ReadOffset();

            Nodes.Capacity = count;

            Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

            reader.ReadAtOffset( boneNamesOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    Nodes.Add( new MotionNode { Name = stringSet.ReadString( reader ) } );
            } );

            reader.ReadAtOffset( boneMatricesOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    Nodes[ i ].Transformation = reader.ReadMatrix4x4();
            } );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet, BinaryFormat format  )
        {
            writer.AddStringToStringTable( Name );
            writer.Write( Nodes.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var bone in Nodes )
                    stringSet.WriteString( writer, bone.Name );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var bone in Nodes )
                    writer.Write( bone.Transformation );
            } );
        }

        public MotionBlock()
        {
            Nodes = new List<MotionNode>();
        }
    }
}