using System;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MikuMikuLibrary.Databases
{
    public enum BoneType
    {
        Rotation = 0,
        Type1 = 1,
        Position = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6,
    };

    public class BoneEntry
    {
        public BoneType Type { get; set; }
        public bool HasParent { get; set; }
        public byte Field01 { get; set; }
        public byte Field02 { get; set; }
        public byte Field03 { get; set; }
        public byte Field04 { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Type = ( BoneType ) reader.ReadByte();
            HasParent = reader.ReadBoolean();
            Field01 = reader.ReadByte();
            Field02 = reader.ReadByte();
            Field03 = reader.ReadByte();
            Field04 = reader.ReadByte();
            reader.SeekCurrent( 2 );
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( byte ) Type );
            writer.Write( HasParent );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.Write( Field04 );
            writer.WriteNulls( 2 );
            writer.AddStringToStringTable( Name );
        }
    }

    public class SkeletonEntry
    {
        public string Name { get; set; }
        public List<BoneEntry> Bones { get; }
        public List<Vector3> Positions { get; }
        public int Field02 { get; set; }
        public List<string> BoneNames1 { get; }
        public List<string> BoneNames2 { get; }
        public List<short> ParentIndices { get; }

        internal void Read( EndianBinaryReader reader )
        {
            long bonesOffset = reader.ReadOffset();
            int positionCount = reader.ReadInt32();
            long positionsOffset = reader.ReadOffset();
            long field02Offset = reader.ReadOffset();
            int boneName1Count = reader.ReadInt32();
            long boneNames1Offset = reader.ReadOffset();
            int boneName2Count = reader.ReadInt32();
            long boneNames2Offset = reader.ReadOffset();
            long parentIndicesOffset = reader.ReadOffset();

            reader.ReadAtOffset( bonesOffset, () =>
            {
                while ( true )
                {
                    var boneEntry = new BoneEntry();
                    boneEntry.Read( reader );

                    if ( boneEntry.Name == "End" )
                        break;

                    Bones.Add( boneEntry );
                }
            } );

            reader.ReadAtOffset( positionsOffset, () =>
            {
                Positions.Capacity = positionCount;
                for ( int i = 0; i < positionCount; i++ )
                    Positions.Add( reader.ReadVector3() );
            } );

            reader.ReadAtOffset( field02Offset, () =>
            {
                Field02 = reader.ReadInt32();
            } );

            reader.ReadAtOffset( boneNames1Offset, () =>
            {
                BoneNames1.Capacity = boneName1Count;
                for ( int i = 0; i < boneName1Count; i++ )
                    BoneNames1.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( boneNames2Offset, () =>
            {
                BoneNames2.Capacity = boneName2Count;
                for ( int i = 0; i < boneName2Count; i++ )
                    BoneNames2.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( parentIndicesOffset, () =>
            {
                ParentIndices.Capacity = boneName2Count;
                for ( int i = 0; i < boneName2Count; i++ )
                    ParentIndices.Add( reader.ReadInt16() );
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var boneEntry in Bones )
                    boneEntry.Write( writer );

                writer.Write( ( byte ) 255 );
                writer.Write( ( byte ) 0 );
                writer.Write( ( byte ) 0 );
                writer.Write( ( byte ) 0 );
                writer.Write( ( byte ) 255 );
                writer.Write( ( byte ) 0 );
                writer.Write( ( byte ) 0 );
                writer.Write( ( byte ) 0 );
                writer.AddStringToStringTable( "End" );
            } );
            writer.Write( Positions.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var position in Positions )
                    writer.Write( position );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () => writer.Write( Field02 ) );
            writer.Write( BoneNames1.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var boneName1 in BoneNames1 )
                    writer.AddStringToStringTable( boneName1 );
            } );
            writer.Write( BoneNames2.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var boneName2 in BoneNames2 )
                    writer.AddStringToStringTable( boneName2 );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var parentId in ParentIndices )
                    writer.Write( parentId );
            } );
            writer.WriteNulls( 32 );
        }

        public BoneEntry GetBoneEntry( string boneName ) =>
            Bones.FirstOrDefault( x => x.Name.Equals( boneName, StringComparison.OrdinalIgnoreCase ) );

        public SkeletonEntry()
        {
            Bones = new List<BoneEntry>();
            Positions = new List<Vector3>();
            BoneNames1 = new List<string>();
            BoneNames2 = new List<string>();
            ParentIndices = new List<short>();
        }
    }

    public class BoneDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<SkeletonEntry> Skeletons { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            uint signature = reader.ReadUInt32();
            int skeletonCount = reader.ReadInt32();
            long skeletonsOffset = reader.ReadOffset();
            long skeletonNamesOffset = reader.ReadOffset();

            reader.ReadAtOffset( skeletonsOffset, () =>
            {
                Skeletons.Capacity = skeletonCount;
                for ( int i = 0; i < skeletonCount; i++ )
                {
                    reader.ReadAtOffset( reader.ReadUInt32(), () =>
                    {
                        var skeletonEntry = new SkeletonEntry();
                        skeletonEntry.Read( reader );
                        Skeletons.Add( skeletonEntry );
                    } );
                }
            } );

            reader.ReadAtOffset( skeletonNamesOffset, () =>
            {
                foreach ( var skeletonEntry in Skeletons )
                    skeletonEntry.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( 0x09102720 );
            writer.Write( Skeletons.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var skeletonEntry in Skeletons )
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () => skeletonEntry.Write( writer ) );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var skeletonEntry in Skeletons )
                    writer.AddStringToStringTable( skeletonEntry.Name );
            } );
            writer.WriteNulls( 20 );
        }

        public BoneDatabase()
        {
            Skeletons = new List<SkeletonEntry>();
        }
    }
}
