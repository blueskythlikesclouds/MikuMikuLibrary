using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public enum ConstraintType
    {
        Orientation,
        Direction,
        Position,
        Distance
    }

    public enum Coupling
    {
        Rigid = 1,
        Soft = 3
    }

    public class ConstraintBlock : NodeBlock
    {
        public override string Signature => "CNS";

        public Coupling Coupling { get; set; }
        public string SourceNodeName { get; set; }

        public IConstraintData Data { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            string type = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Coupling = ( Coupling ) reader.ReadInt32();
            SourceNodeName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            switch ( type )
            {
                case "Orientation":
                    Data = new OrientationConstraintData();
                    break;

                case "Direction":
                    Data = new DirectionConstraintData();
                    break;

                case "Position":
                    Data = new PositionConstraintData();
                    break;

                case "Distance":
                    Data = new DistanceConstraintData();
                    break;

                default:
                    throw new InvalidDataException( $"Unrecognized constraint type: {type}" );
            }

            Data.Read( reader );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet, BinaryFormat format  )
        {
            writer.AddStringToStringTable( Enum.GetName( typeof( ConstraintType ), Data.Type ) );
            writer.AddStringToStringTable( Name );
            writer.Write( ( int ) Coupling );
            writer.AddStringToStringTable( SourceNodeName );
            Data.Write( writer );
        }
    }

    [TypeConverter( typeof(ExpandableObjectConverter) )]
    public interface IConstraintData
    {
        ConstraintType Type { get; }

        void Read( EndianBinaryReader reader );
        void Write( EndianBinaryWriter writer );
    }

    public class OrientationConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Orientation;

        public Vector3 Offset { get; set; }

        public void Read( EndianBinaryReader reader )
        {
            Offset = reader.ReadVector3();
        }

        public void Write( EndianBinaryWriter writer )
        {
            writer.Write( Offset );
        }
    }

    [TypeConverter( typeof(ExpandableObjectConverter) )]
    public class UpVectorData
    {
        public bool Active { get; set; }
        public float Roll { get; set; }
        public Vector3 AffectedAxis { get; set; }
        public Vector3 PointAt { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Active = reader.ReadInt32() != 0;
            Roll = reader.ReadSingle();
            AffectedAxis = reader.ReadVector3();
            PointAt = reader.ReadVector3();
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Active ? 1 : 0 );
            writer.Write( Roll );
            writer.Write( AffectedAxis );
            writer.Write( PointAt );
            writer.AddStringToStringTable( Name );
        }
    }

    public class DirectionConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Direction;

        public UpVectorData UpVector { get; }
        public Vector3 AlignAxis { get; set; }
        public Vector3 TargetOffset { get; set; }

        public void Read( EndianBinaryReader reader )
        {
            UpVector.Read( reader );
            AlignAxis = reader.ReadVector3();
            TargetOffset = reader.ReadVector3();
        }

        public void Write( EndianBinaryWriter writer )
        {
            UpVector.Write( writer );
            writer.Write( AlignAxis );
            writer.Write( TargetOffset );
        }

        public DirectionConstraintData()
        {
            UpVector = new UpVectorData();
        }
    }

    [TypeConverter( typeof(ExpandableObjectConverter) )]
    public class AttachPointData
    {
        public bool AffectedByOrientation { get; set; }
        public bool AffectedByScaling { get; set; }
        public Vector3 Offset { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            AffectedByOrientation = reader.ReadInt32() != 0;
            AffectedByScaling = reader.ReadInt32() != 0;
            Offset = reader.ReadVector3();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( AffectedByOrientation ? 1 : 0 );
            writer.Write( AffectedByScaling ? 1 : 0 );
            writer.Write( Offset );
        }
    }

    public class PositionConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Position;

        public UpVectorData UpVector { get; }
        public AttachPointData ConstrainedObject { get; }
        public AttachPointData ConstrainingObject { get; }

        public void Read( EndianBinaryReader reader )
        {
            UpVector.Read( reader );
            ConstrainedObject.Read( reader );
            ConstrainingObject.Read( reader );
        }

        public void Write( EndianBinaryWriter writer )
        {
            UpVector.Write( writer );
            ConstrainedObject.Write( writer );
            ConstrainingObject.Write( writer );
        }

        public PositionConstraintData()
        {
            UpVector = new UpVectorData();
            ConstrainedObject = new AttachPointData();
            ConstrainingObject = new AttachPointData();
        }
    }

    public class DistanceConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Distance;

        public UpVectorData UpVector { get; }
        public float Distance { get; set; }
        public AttachPointData ConstrainedObject { get; }
        public AttachPointData ConstrainingObject { get; }

        public void Read( EndianBinaryReader reader )
        {
            UpVector.Read( reader );
            Distance = reader.ReadSingle();
            ConstrainedObject.Read( reader );
            ConstrainingObject.Read( reader );
        }

        public void Write( EndianBinaryWriter writer )
        {
            UpVector.Write( writer );
            writer.Write( Distance );
            ConstrainedObject.Write( writer );
            ConstrainingObject.Write( writer );
        }

        public DistanceConstraintData()
        {
            UpVector = new UpVectorData();
            ConstrainedObject = new AttachPointData();
            ConstrainingObject = new AttachPointData();
        }
    }
}