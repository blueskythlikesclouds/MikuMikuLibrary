using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
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

    public class ConstraintBlock : NodeBlock
    {
        public override string Signature => "CNS";

        public int Field11 { get; set; }
        public string SourceBoneName { get; set; }

        public IConstraintData Data { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            string type = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field11 = reader.ReadInt32();
            SourceBoneName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

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

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( Enum.GetName( typeof( ConstraintType ), Data.Type ) );
            writer.AddStringToStringTable( Name );
            writer.Write( Field11 );
            writer.AddStringToStringTable( SourceBoneName );
            Data.Write( writer );
        }

        // Obsolete properties

        [Obsolete( "This property is obsolete. Please use Name instead." ), Browsable( false )]
        public string BoneName
        {
            get => Name;
            set => Name = value;
        }
    }

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

    public class DirectionConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Direction;

        public int Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public string Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public float Field34 { get; set; }
        public float Field38 { get; set; }

        public void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadInt32();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
        }

        public void Write( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field04 );
            writer.Write( Field08 );
            writer.Write( Field0C );
            writer.Write( Field10 );
            writer.Write( Field14 );
            writer.Write( Field18 );
            writer.Write( Field1C );
            writer.AddStringToStringTable( Field20 );
            writer.Write( Field24 );
            writer.Write( Field28 );
            writer.Write( Field2C );
            writer.Write( Field30 );
            writer.Write( Field34 );
            writer.Write( Field38 );
        }
    }

    public class PositionConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Position;

        public float Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public string Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public float Field34 { get; set; }
        public float Field38 { get; set; }
        public float Field3C { get; set; }
        public float Field40 { get; set; }
        public float Field44 { get; set; }
        public float Field48 { get; set; }

        public void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
            Field3C = reader.ReadSingle();
            Field40 = reader.ReadSingle();
            Field44 = reader.ReadSingle();
            Field48 = reader.ReadSingle();
        }

        public void Write( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field04 );
            writer.Write( Field08 );
            writer.Write( Field0C );
            writer.Write( Field10 );
            writer.Write( Field14 );
            writer.Write( Field18 );
            writer.Write( Field1C );
            writer.AddStringToStringTable( Field20 );
            writer.Write( Field24 );
            writer.Write( Field28 );
            writer.Write( Field2C );
            writer.Write( Field30 );
            writer.Write( Field34 );
            writer.Write( Field38 );
            writer.Write( Field3C );
            writer.Write( Field40 );
            writer.Write( Field44 );
            writer.Write( Field48 );
        }
    }

    public class DistanceConstraintData : IConstraintData
    {
        public ConstraintType Type => ConstraintType.Distance;

        public float Field00 { get; set; }
        public float Field04 { get; set; }
        public float Field08 { get; set; }
        public float Field0C { get; set; }
        public float Field10 { get; set; }
        public float Field14 { get; set; }
        public float Field18 { get; set; }
        public float Field1C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public float Field2C { get; set; }
        public float Field30 { get; set; }
        public float Field34 { get; set; }
        public float Field38 { get; set; }
        public float Field3C { get; set; }
        public float Field40 { get; set; }
        public float Field44 { get; set; }
        public float Field48 { get; set; }
        public float Field4C { get; set; }

        public void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field0C = reader.ReadSingle();
            Field10 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field1C = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field2C = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field38 = reader.ReadSingle();
            Field3C = reader.ReadSingle();
            Field40 = reader.ReadSingle();
            Field44 = reader.ReadSingle();
            Field48 = reader.ReadSingle();
            Field4C = reader.ReadSingle();
        }

        public void Write( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field04 );
            writer.Write( Field08 );
            writer.Write( Field0C );
            writer.Write( Field10 );
            writer.Write( Field14 );
            writer.Write( Field18 );
            writer.Write( Field1C );
            writer.Write( Field20 );
            writer.Write( Field24 );
            writer.Write( Field28 );
            writer.Write( Field2C );
            writer.Write( Field30 );
            writer.Write( Field34 );
            writer.Write( Field38 );
            writer.Write( Field3C );
            writer.Write( Field40 );
            writer.Write( Field44 );
            writer.Write( Field48 );
            writer.Write( Field4C );
        }
    }
}