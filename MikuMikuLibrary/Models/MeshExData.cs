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
        public override string Kind
        {
            get { return "CNS"; }
        }

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
            Field10 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Field11 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
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
        public override string Kind
        {
            get { return "MOT"; }
        }

        public string Field10 { get; set; }
        public List<int> Field11 { get; }
        public List<Matrix4x4> Field12 { get; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            long field10Offset = reader.ReadOffset();
            int count = reader.ReadInt32();
            long field11Offset = reader.ReadOffset();
            long field12Offset = reader.ReadOffset();

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

        internal override void WriteBody( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Field10 );
            writer.Write( Field11.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Field11 )
                    writer.Write( value );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
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

    public class MeshExBlockExpression : MeshExBlock
    {
        public override string Kind
        {
            get { return "EXP"; }
        }

        public string Field10 { get; set; }
        public List<string> Field11 { get; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            long field10Offset = reader.ReadOffset();
            int field11Count = reader.ReadInt32();

            Field10 = reader.ReadStringAtOffset( field10Offset, StringBinaryFormat.NullTerminated );

            Field11.Capacity = field11Count;
            for ( int i = 0; i < field11Count; i++ )
                Field11.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
        }

        internal override void WriteBody( EndianBinaryWriter writer )
        {
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

    public class MeshExBlockOsage : MeshExBlock
    {
        public override string Kind
        {
            get { return "OSG"; }
        }

        public int Field10 { get; set; }
        public int Field11 { get; set; }
        public int Field12 { get; set; }
        public int Field13 { get; set; }

        internal override void ReadBody( EndianBinaryReader reader )
        {
            if ( reader.AddressSpace == IO.AddressSpace.Int64 )
                reader.SeekCurrent( 4 );

            Field10 = reader.ReadInt32();
            Field11 = reader.ReadInt32();
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadInt32();
        }

        internal override void WriteBody( EndianBinaryWriter writer )
        {
            writer.Write( Field10 );
            writer.Write( Field11 );
            writer.Write( Field12 );
            writer.Write( Field13 );
            writer.WriteNulls( 40 );
        }
    }

    public abstract class MeshExBlock
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
            ReadBody( reader );
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
            WriteBody( writer );
        }

        internal abstract void ReadBody( EndianBinaryReader reader );
        internal abstract void WriteBody( EndianBinaryWriter writer );
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
        public List<MeshExBlock> ExBlocks { get; }
        public List<string> Strings2 { get; }
        public List<MeshExEntry> Entries { get; }

        internal void Read( EndianBinaryReader reader )
        {
            int string1Count = reader.ReadInt32();
            int osageCount = reader.ReadInt32();
            reader.SeekCurrent( 4 );
            long osageBonesOffset = reader.ReadOffset();
            long string1OffsetsOffset = reader.ReadOffset();
            long exBlocksOffset = reader.ReadOffset();
            int string2Count = reader.ReadInt32();
            long string2OffsetsOffset = reader.ReadOffset();
            long exEntriesOffset = reader.ReadOffset();

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
                    long exBlockDataOffset = reader.ReadOffset();

                    if ( exBlockDataOffset == 0 )
                        break;

                    MeshExBlock exBlock = null;

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
            writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
            {
                foreach ( var osageBone in Osages )
                    osageBone.Write( writer );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Strings1 )
                    writer.AddStringToStringTable( value );
            } );
            writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
            {
                foreach ( var exBlock in ExBlocks )
                {
                    writer.AddStringToStringTable( exBlock.Kind );
                    writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () => exBlock.Write( writer ) );
                }
                writer.WriteNulls( 8 );
            } );
            writer.Write( Strings2.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var value in Strings2 )
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
            Osages = new List<MeshExOsageEntry>();
            ExBlocks = new List<MeshExBlock>();
            Strings1 = new List<string>();
            Strings2 = new List<string>();
            Entries = new List<MeshExEntry>();
        }
    }
}
