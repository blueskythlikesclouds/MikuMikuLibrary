using MikuMikuLibrary.IO.Common;
using System;
using System.Diagnostics;
using System.Collections.Generic;
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

    public class MeshExBlockConstraint : MeshExBlockBase
    {
        public override string Kind
        {
            get { return "CNS"; }
        }

        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public List<string> Field12 { get; set; }

        internal override void Read( EndianBinaryReader reader )
        {
            base.Read( reader );

            Field10 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Field11 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );

            int count = reader.ReadInt32();
            for ( int i = 0; i < count; i++ )
                Field12.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
        }

        internal override void Write( EndianBinaryWriter writer )
        {
            base.Write( writer );

            writer.AddStringToStringTable( Field10 );
            writer.AddStringToStringTable( Field11 );
            writer.Write( Field12.Count );

            foreach ( var value in Field12 )
                writer.AddStringToStringTable( value );

            writer.WriteNulls( ( 11 - Field12.Count ) * 4 );
        }

        public MeshExBlockConstraint()
        {
            Field12 = new List<string>( 11 );
        }
    }

    public class MeshExBlockMotion : MeshExBlockBase
    {
        public override string Kind
        {
            get { return "MOT"; }
        }

        public string Field10 { get; set; }
        public List<int> Field11 { get; }
        public List<Matrix4x4> Field12 { get; }

        internal override void Read( EndianBinaryReader reader )
        {
            base.Read( reader );

            uint field10Offset = reader.ReadUInt32();
            int count = reader.ReadInt32();
            uint field11Offset = reader.ReadUInt32();
            uint field12Offset = reader.ReadUInt32();

            Field11.Capacity = Field12.Capacity = count;

            Field10 = reader.ReadStringAtOffset( field10Offset, StringBinaryFormat.NullTerminated );

            reader.ReadAtOffset( field11Offset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    Field11.Add( reader.ReadInt32() );
            } );

            reader.ReadAtOffset( field12Offset, () =>
            {
                for ( int i = 0; i < count; i++ )
                    Field12.Add( reader.ReadMatrix4x4() );
            } );
        }

        internal override void Write( EndianBinaryWriter writer )
        {
            base.Write( writer );

            writer.AddStringToStringTable( Field10 );
            writer.Write( Field11.Count );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Field11 )
                    writer.Write( value );
            } );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Field12 )
                    writer.Write( value );
            } );
        }

        public MeshExBlockMotion()
        {
            Field11 = new List<int>();
            Field12 = new List<Matrix4x4>();
        }
    }

    public class MeshExBlockExpression : MeshExBlockBase
    {
        public override string Kind
        {
            get { return "EXP"; }
        }

        public string Field10 { get; set; }
        public List<string> Field11 { get; }

        internal override void Read( EndianBinaryReader reader )
        {
            base.Read( reader );

            uint field10Offset = reader.ReadUInt32();
            int field11Count = reader.ReadInt32();

            Field10 = reader.ReadStringAtOffset( field10Offset, StringBinaryFormat.NullTerminated );

            Field11.Capacity = field11Count;
            for ( int i = 0; i < field11Count; i++ )
                Field11.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
        }

        internal override void Write( EndianBinaryWriter writer )
        {
            base.Write( writer );

            writer.AddStringToStringTable( Field10 );
            writer.Write( Field11.Count );

            foreach ( var str in Field11 )
                writer.AddStringToStringTable( str );

            writer.WriteNulls( ( 12 - Field11.Count ) * 4 );
        }

        public MeshExBlockExpression()
        {
            Field11 = new List<string>();
        }
    }

    public class MeshExBlockOsage : MeshExBlockBase
    {
        public override string Kind
        {
            get { return "OSG"; }
        }

        public int Field10 { get; set; }
        public int Field11 { get; set; }
        public int Field12 { get; set; }
        public int Field13 { get; set; }

        internal override void Read( EndianBinaryReader reader )
        {
            base.Read( reader );

            Field10 = reader.ReadInt32();
            Field11 = reader.ReadInt32();
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadInt32();
        }

        internal override void Write( EndianBinaryWriter writer )
        {
            base.Write( writer );

            writer.Write( Field10 );
            writer.Write( Field11 );
            writer.Write( Field12 );
            writer.Write( Field13 );
            writer.WriteNulls( 40 );
        }
    }

    public abstract class MeshExBlockBase
    {
        public abstract string Kind { get; }

        public string Field00 { get; set; }
        public float Field01 { get; set; }
        public float Field02 { get; set; }
        public float Field03 { get; set; }
        public float Field04 { get; set; }
        public float Field05 { get; set; }
        public float Field06 { get; set; }
        public float Field07 { get; set; }
        public float Field08 { get; set; }
        public float Field09 { get; set; }

        internal virtual void Read( EndianBinaryReader reader )
        {
            Field00 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Field01 = reader.ReadSingle();
            Field02 = reader.ReadSingle();
            Field03 = reader.ReadSingle();
            Field04 = reader.ReadSingle();
            Field05 = reader.ReadSingle();
            Field06 = reader.ReadSingle();
            Field07 = reader.ReadSingle();
            Field08 = reader.ReadSingle();
            Field09 = reader.ReadSingle();
        }

        internal virtual void Write( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.Write( Field04 );
            writer.Write( Field05 );
            writer.Write( Field06 );
            writer.Write( Field07 );
            writer.Write( Field08 );
            writer.Write( Field09 );
        }
    }

    public class MeshExOsageEntry
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

    public class MeshExData
    {
        public const int ByteSize = 0x60;

        public List<MeshExOsageEntry> Osages { get; }
        public List<string> Strings1 { get; }
        public List<MeshExBlockBase> ExBlocks { get; }
        public List<string> Strings2 { get; }
        public List<MeshExEntry> Entries { get; }

        internal void Read( EndianBinaryReader reader )
        {
            int string1Count = reader.ReadInt32();
            int osageCount = reader.ReadInt32();
            reader.SeekCurrent( 4 );
            uint osageBonesOffset = reader.ReadUInt32();
            uint string1OffsetsOffset = reader.ReadUInt32();
            uint exBlocksOffset = reader.ReadUInt32();
            int string2Count = reader.ReadInt32();
            uint string2OffsetsOffset = reader.ReadUInt32();
            uint exEntriesOffset = reader.ReadUInt32();

            reader.ReadAtOffset( osageBonesOffset, () =>
            {
                Osages.Capacity = osageCount;
                for ( int i = 0; i < osageCount; i++ )
                {
                    var osageBone = new MeshExOsageEntry();
                    osageBone.Read( reader );
                    Osages.Add( osageBone );
                }
            } );

            reader.ReadAtOffset( string1OffsetsOffset, () =>
            {
                Strings1.Capacity = string1Count;
                for ( int i = 0; i < string1Count; i++ )
                    Strings1.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( exBlocksOffset, () =>
            {
                while ( true )
                {
                    string exBlockKind = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                    uint exBlockDataOffset = reader.ReadUInt32();

                    if ( exBlockDataOffset == 0 )
                        break;

                    MeshExBlockBase exBlock = null;

                    reader.ReadAtOffsetAndSeekBack( exBlockDataOffset, () =>
                    {
                        switch ( exBlockKind )
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
                                Debug.WriteLine( $"WARNING: Unknown ex-block type {exBlockKind}" );
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

            reader.ReadAtOffset( string2OffsetsOffset, () =>
            {
                Strings2.Capacity = string2Count;
                for ( int i = 0; i < string2Count; i++ )
                    Strings2.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
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
            writer.Write( Strings1.Count );
            writer.Write( Osages.Count );
            writer.WriteNulls( 4 );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var osageBone in Osages )
                    osageBone.Write( writer );
            } );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Strings1 )
                    writer.AddStringToStringTable( value );
            } );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var exBlock in ExBlocks )
                {
                    writer.AddStringToStringTable( exBlock.Kind );
                    writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () => exBlock.Write( writer ) );
                }
                writer.WriteNulls( 8 );
            } );
            writer.Write( Strings2.Count );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Strings2 )
                    writer.AddStringToStringTable( value );
            } );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var entry in Entries )
                    entry.Write( writer );
                writer.WriteNulls( 12 );
            } );
            writer.WriteNulls( 28 );
        }

        public MeshExData()
        {
            Osages = new List<MeshExOsageEntry>();
            ExBlocks = new List<MeshExBlockBase>();
            Strings1 = new List<string>();
            Strings2 = new List<string>();
            Entries = new List<MeshExEntry>();
        }
    }
}
