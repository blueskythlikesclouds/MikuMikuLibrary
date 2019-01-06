using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public class MeshExEntry
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

    public class MeshExBlockConstraint : MeshExBlock
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

    public class MeshExBlockMotion : MeshExBlock
    {
        public override string Signature => "MOT";

        public string Name { get; set; }
        public List<int> BoneIDs { get; }
        public List<Matrix4x4> BoneMatrices { get; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            long nameOffset = reader.ReadOffset();
            int count = reader.ReadInt32();
            long boneIDsOffset = reader.ReadOffset();
            long boneMatricesOffset = reader.ReadOffset();

            BoneIDs.Capacity = BoneMatrices.Capacity = count;

            Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

            reader.ReadAtOffset( boneIDsOffset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    BoneIDs.Add( reader.ReadInt32() );
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
            writer.Write( BoneIDs.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in BoneIDs )
                    writer.Write( value );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in BoneMatrices )
                    writer.Write( value );
            } );
        }

        public MeshExBlockMotion()
        {
            BoneIDs = new List<int>();
            BoneMatrices = new List<Matrix4x4>();
        }
    }

    public class MeshExBlockExpression : MeshExBlock
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

        public MeshExBlockExpression()
        {
            Expressions = new List<string>();
        }
    }

    public class MeshExBlockOsage : MeshExBlock
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
            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.WriteNulls( 40 );
        }
    }

    public abstract class MeshExBlock
    {
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

    public class MeshExOsageBoneEntry
    {
        public int BoneID { get; set; }
        public float Field00 { get; set; }
        public int NameIndex { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            BoneID = reader.ReadInt32();
            Field00 = reader.ReadSingle();
            NameIndex = reader.ReadInt32();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( BoneID );
            writer.Write( Field00 );
            writer.Write( NameIndex );
        }
    }

    public class MeshExData
    {
        public const int BYTE_SIZE = 0x60;

        public List<MeshExOsageBoneEntry> OsageBones { get; }
        public List<string> OsageNames { get; }
        public List<MeshExBlock> ExBlocks { get; }
        public List<string> BoneNames { get; }
        public List<MeshExEntry> Entries { get; }

        internal void Read( EndianBinaryReader reader )
        {
            int osageNameCount = reader.ReadInt32();
            int osageBoneCount = reader.ReadInt32();
            reader.SeekCurrent( 4 );
            long osageBonesOffset = reader.ReadOffset();
            long osageNamesOffset = reader.ReadOffset();
            long exBlocksOffset = reader.ReadOffset();
            int boneNameCount = reader.ReadInt32();
            long boneNamesOffset = reader.ReadOffset();
            long exEntriesOffset = reader.ReadOffset();

            reader.ReadAtOffset( osageBonesOffset, () =>
            {
                OsageBones.Capacity = osageBoneCount;
                for ( int i = 0; i < osageBoneCount; i++ )
                {
                    var osageBone = new MeshExOsageBoneEntry();
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

            reader.ReadAtOffset( exBlocksOffset, () =>
            {
                while ( true )
                {
                    string exBlockSignature = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    long exBlockDataOffset = reader.ReadOffset();

                    if ( exBlockDataOffset == 0 )
                        break;

                    MeshExBlock exBlock = null;

                    reader.ReadAtOffset( exBlockDataOffset, () =>
                    {
                        switch ( exBlockSignature )
                        {
                            case "OSG":
                                exBlock = new MeshExBlockOsage();
                                break;
                            case "EXP":
                                exBlock = new MeshExBlockExpression();
                                break;
                            case "MOT":
                                exBlock = new MeshExBlockMotion();
                                break;
                            case "CNS":
                                exBlock = new MeshExBlockConstraint();
                                break;
                            default:
                                Debug.WriteLine( $"WARNING: Unknown ex-block type {exBlockSignature}" );
                                break;
                        }

                        if ( exBlock != null )
                        {
                            exBlock.Read( reader );
                            ExBlocks.Add( exBlock );
                        }
                    } );
                }
            } );

            reader.ReadAtOffset( boneNamesOffset, () =>
            {
                BoneNames.Capacity = boneNameCount;
                for ( int i = 0; i < boneNameCount; i++ )
                    BoneNames.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( exEntriesOffset, () =>
            {
                while ( true )
                {
                    var exEntry = new MeshExEntry();
                    exEntry.Read( reader );

                    if ( exEntry.Field00 == 0 )
                        break;

                    Entries.Add( exEntry );
                }
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( OsageNames.Count );
            writer.Write( OsageBones.Count );
            writer.WriteNulls( 4 );
            writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
            {
                foreach ( var osageBone in OsageBones )
                    osageBone.Write( writer );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in OsageNames )
                    writer.AddStringToStringTable( value );
            } );
            writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
            {
                foreach ( var exBlock in ExBlocks )
                {
                    writer.AddStringToStringTable( exBlock.Signature );
                    writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () => exBlock.Write( writer ) );
                }
                writer.WriteNulls( 8 );
            } );
            writer.Write( BoneNames.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in BoneNames )
                    writer.AddStringToStringTable( value );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var entry in Entries )
                    entry.Write( writer );
                writer.WriteNulls( 12 );
            } );
            writer.WriteNulls( 28 );
        }

        public MeshExData()
        {
            OsageBones = new List<MeshExOsageBoneEntry>();
            ExBlocks = new List<MeshExBlock>();
            OsageNames = new List<string>();
            BoneNames = new List<string>();
            Entries = new List<MeshExEntry>();
        }
    }
}
