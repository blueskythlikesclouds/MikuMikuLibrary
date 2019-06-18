using System;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.Objects
{
    public class ExEntry
    {
        public int Field00 { get; set; }
        public int Field01 { get; set; }
        public float Field02 { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadInt32();
            Field01 = reader.ReadInt32();
            Field02 = reader.ReadSingle();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
        }
    }

    public class ExConstraintNode : ExNode
    {
        public override string Signature => "CNS";

        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public int Field12 { get; set; }
        public string Field13 { get; set; }
        public float Field14 { get; set; }
        public float Field15 { get; set; }
        public float Field16 { get; set; }
        public float Field17 { get; set; }
        public float Field18 { get; set; }
        public float Field19 { get; set; }
        public float Field20 { get; set; }
        public float Field21 { get; set; }
        public float Field22 { get; set; }
        public float Field23 { get; set; }
        public float Field24 { get; set; }
        public float Field25 { get; set; }
        public float Field26 { get; set; }
        public float Field27 { get; set; }
        public float Field28 { get; set; }
        public float Field29 { get; set; }
        public float Field30 { get; set; }
        public float Field31 { get; set; }
        public float Field32 { get; set; }
        public float Field33 { get; set; }
        public float Field34 { get; set; }
        public float Field35 { get; set; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            Field10 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field11 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field14 = reader.ReadSingle();
            Field15 = reader.ReadSingle();
            Field16 = reader.ReadSingle();
            Field17 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field19 = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field21 = reader.ReadSingle();
            Field22 = reader.ReadSingle();
            Field23 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field25 = reader.ReadSingle();
            Field26 = reader.ReadSingle();
            Field27 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field29 = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field31 = reader.ReadSingle();
            Field32 = reader.ReadSingle();
            Field33 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field35 = reader.ReadSingle();
        }

        internal override void WriteBody( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Field10 );
            writer.AddStringToStringTable( Field11 );
            writer.Write( Field12 );
            writer.AddStringToStringTable( Field13 );
            writer.Write( Field14 );
            writer.Write( Field15 );
            writer.Write( Field16 );
            writer.Write( Field17 );
            writer.Write( Field18 );
            writer.Write( Field19 );
            writer.Write( Field20 );
            writer.Write( Field21 );
            writer.Write( Field22 );
            writer.Write( Field23 );
            writer.Write( Field24 );
            writer.Write( Field25 );
            writer.Write( Field26 );
            writer.Write( Field27 );
            writer.Write( Field28 );
            writer.Write( Field29 );
            writer.Write( Field30 );
            writer.Write( Field31 );
            writer.Write( Field32 );
            writer.Write( Field33 );
            writer.Write( Field34 );
            writer.Write( Field35 );
        }
    }

    public class ExMotionNode : ExNode
    {
        public override string Signature => "MOT";

        public string Name { get; set; }
        public List<int> BoneIds { get; }
        public List<Matrix4x4> BoneMatrices { get; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            long nameOffset = reader.ReadOffset();
            int count = reader.ReadInt32();
            long boneIdsOffset = reader.ReadOffset();
            long boneMatricesOffset = reader.ReadOffset();

            BoneIds.Capacity = BoneMatrices.Capacity = count;

            Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

            reader.ReadAtOffset( boneIdsOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    BoneIds.Add( reader.ReadInt32() );
            } );

            reader.ReadAtOffset( boneMatricesOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    BoneMatrices.Add( reader.ReadMatrix4x4() );
            } );
        }

        internal override void WriteBody( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Name );
            writer.Write( BoneIds.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in BoneIds )
                    writer.Write( value );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in BoneMatrices )
                    writer.Write( value );
            } );
        }

        public ExMotionNode()
        {
            BoneIds = new List<int>();
            BoneMatrices = new List<Matrix4x4>();
        }
    }

    public class ExExpressionNode : ExNode
    {
        public override string Signature => "EXP";

        public string BoneName { get; set; }
        public List<string> Expressions { get; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            long boneNameOffset = reader.ReadOffset();
            int expressionCount = reader.ReadInt32();

            BoneName = reader.ReadStringAtOffset( boneNameOffset, StringBinaryFormat.NullTerminated );

            Expressions.Capacity = expressionCount;
            for ( int i = 0; i < expressionCount; i++ )
                Expressions.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
        }

        internal override void WriteBody( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( BoneName );
            writer.Write( Expressions.Count );

            foreach ( var str in Expressions )
                writer.AddStringToStringTable( str );

            writer.WriteNulls( ( 12 - Expressions.Count ) * 4 );
        }

        public ExExpressionNode()
        {
            Expressions = new List<string>();
        }
    }

    public class ExOsageNode : ExNode
    {
        public override string Signature => "OSG";

        public int Field00 { get; set; }
        public int Field01 { get; set; }
        public int Field02 { get; set; }
        public int Field03 { get; set; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SeekCurrent( 4 );

            Field00 = reader.ReadInt32();
            Field01 = reader.ReadInt32();
            Field02 = reader.ReadInt32();
            Field03 = reader.ReadInt32();
        }

        internal override void WriteBody( EndianBinaryWriter writer )
        {
            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 4 );

            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.WriteNulls( 40 );
        }
    }

    public abstract class ExNode
    {
        internal static readonly IReadOnlyDictionary<string, Func<ExNode>> sNodeFactory =
            new Dictionary<string, Func<ExNode>>
            {
                { "OSG", () => new ExOsageNode() },
                { "EXP", () => new ExExpressionNode() },
                { "MOT", () => new ExMotionNode() },
                { "CNS", () => new ExConstraintNode() }
            };

        public abstract string Signature { get; }

        public string ParentName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        internal virtual void Read( EndianBinaryReader reader )
        {
            ParentName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Position = reader.ReadVector3();
            Rotation = reader.ReadVector3();
            Scale = reader.ReadVector3();
            ReadBody( reader );
        }

        internal virtual void Write( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( ParentName );
            writer.Write( Position );
            writer.Write( Rotation );
            writer.Write( Scale );
            WriteBody( writer );
        }

        internal abstract void ReadBody( EndianBinaryReader reader );
        internal abstract void WriteBody( EndianBinaryWriter writer );
    }

    public class ExOsageBone
    {
        public int Field00 { get; set; }
        public float Field01 { get; set; }
        public int Field02 { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadInt32();
            Field01 = reader.ReadSingle();
            Field02 = reader.ReadInt32();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
        }
    }

    public class ExData
    {
        public const int BYTE_SIZE = 0x60;

        public List<ExOsageBone> OsageBones { get; }
        public List<string> OsageNames { get; }
        public List<ExNode> Nodes { get; }
        public List<string> BoneNames { get; }
        public List<ExEntry> Entries { get; }

        internal void Read( EndianBinaryReader reader )
        {
            int osageNameCount = reader.ReadInt32();
            int osageBoneCount = reader.ReadInt32();
            reader.SeekCurrent( 4 );
            long osageBonesOffset = reader.ReadOffset();
            long osageNamesOffset = reader.ReadOffset();
            long nodesOffset = reader.ReadOffset();
            int boneNameCount = reader.ReadInt32();
            long boneNamesOffset = reader.ReadOffset();
            long entriesOffset = reader.ReadOffset();

            reader.ReadAtOffset( osageBonesOffset, () =>
            {
                OsageBones.Capacity = osageBoneCount;
                for ( int i = 0; i < osageBoneCount; i++ )
                {
                    var osageBone = new ExOsageBone();
                    osageBone.Read( reader );
                    OsageBones.Add( osageBone );
                }
            } );

            reader.ReadAtOffset( osageNamesOffset, () =>
            {
                OsageNames.Capacity = osageNameCount;
                for ( int i = 0; i < osageNameCount; i++ )
                    OsageNames.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( nodesOffset, () =>
            {
                while ( true )
                {
                    string nodeSignature = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    long nodeOffset = reader.ReadOffset();

                    if ( nodeOffset == 0 )
                        break;

                    reader.ReadAtOffset( nodeOffset, () =>
                    {
                        if ( !ExNode.sNodeFactory.TryGetValue( nodeSignature, out var nodeConstructor ) )
                            return;

                        var node = nodeConstructor();
                        node.Read( reader );
                        Nodes.Add( node );
                    } );
                }
            } );

            reader.ReadAtOffset( boneNamesOffset, () =>
            {
                BoneNames.Capacity = boneNameCount;
                for ( int i = 0; i < boneNameCount; i++ )
                    BoneNames.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( entriesOffset, () =>
            {
                while ( true )
                {
                    var entry = new ExEntry();
                    entry.Read( reader );

                    if ( entry.Field00 == 0 )
                        break;

                    Entries.Add( entry );
                }
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( OsageNames.Count );
            writer.Write( OsageBones.Count );
            writer.WriteNulls( 4 );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var osageBone in OsageBones )
                    osageBone.Write( writer );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in OsageNames )
                    writer.AddStringToStringTable( value );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var node in Nodes )
                {
                    writer.AddStringToStringTable( node.Signature );
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () => node.Write( writer ) );
                }

                writer.WriteNulls( 8 );
            } );
            writer.Write( BoneNames.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in BoneNames )
                    writer.AddStringToStringTable( value );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var entry in Entries )
                    entry.Write( writer );
                writer.WriteNulls( 12 );
            } );
            writer.WriteNulls( writer.AddressSpace == AddressSpace.Int64 ? 32 : 28 );
        }

        public ExData()
        {
            OsageBones = new List<ExOsageBone>();
            Nodes = new List<ExNode>();
            OsageNames = new List<string>();
            BoneNames = new List<string>();
            Entries = new List<ExEntry>();
        }
    }
}
