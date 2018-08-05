using MikuMikuLibrary.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    public class BoneEntry
    {
        [Flags]
        public enum BoneEnum
        {
            Flag01 = 0b001,
            Flag02 = 0b010,
            Flag04 = 0b100,
        }

        public BoneEnum Field00 { get; set; }
        public bool HasParent { get; set; }
        public byte ParentNameIndex { get; set; }
        public byte Field01 { get; set; }
        public byte PairNameIndex { get; set; }
        public byte Field02 { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Field00 = ( BoneEnum )reader.ReadByte();
            HasParent = reader.ReadBoolean();
            ParentNameIndex = reader.ReadByte();
            Field01 = reader.ReadByte();
            PairNameIndex = reader.ReadByte();
            Field02 = reader.ReadByte();
            reader.SeekCurrent( 2 );
            Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( byte )Field00 );
            writer.Write( HasParent );
            writer.Write( ParentNameIndex );
            writer.Write( Field01 );
            writer.Write( PairNameIndex );
            writer.Write( Field02 );
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
            uint bonesOffset = reader.ReadUInt32();
            int positionCount = reader.ReadInt32();
            uint positionsOffset = reader.ReadUInt32();
            uint field02Offset = reader.ReadUInt32();
            int boneName1Count = reader.ReadInt32();
            uint boneNames1Offset = reader.ReadUInt32();
            int boneName2Count = reader.ReadInt32();
            uint boneNames2Offset = reader.ReadUInt32();
            uint parentIndicesOffset = reader.ReadUInt32();

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
                    BoneNames1.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
            } );

            reader.ReadAtOffset( boneNames2Offset, () =>
            {
                BoneNames2.Capacity = boneName2Count;
                for ( int i = 0; i < boneName2Count; i++ )
                    BoneNames2.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
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
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var boneEntry in Bones )
                    boneEntry.Write( writer );

                writer.Write( 255 );
                writer.Write( 255 );
                writer.AddStringToStringTable( "End" );
            } );
            writer.Write( Positions.Count );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var position in Positions )
                    writer.Write( position );
            } );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () => writer.Write( Field02 ) );
            writer.Write( BoneNames1.Count );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var boneName1 in BoneNames1 )
                    writer.AddStringToStringTable( boneName1 );
            } );
            writer.Write( BoneNames2.Count );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var boneName2 in BoneNames2 )
                    writer.AddStringToStringTable( boneName2 );
            } );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var parentID in ParentIndices )
                    writer.Write( parentID );
            } );
            writer.WriteNulls( 32 );
        }

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
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public List<SkeletonEntry> Skeletons { get; }

        protected override void Read( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                uint signature = reader.ReadUInt32();
                int skeletonCount = reader.ReadInt32();
                uint skeletonsOffset = reader.ReadUInt32();
                uint skeletonNamesOffset = reader.ReadUInt32();

                reader.ReadAtOffset( skeletonsOffset, () =>
                {
                    Skeletons.Capacity = skeletonCount;
                    for ( int i = 0; i < skeletonCount; i++ )
                    {
                        reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
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
                        skeletonEntry.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                } );
            }
        }

        protected override void Write( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( 0x09102720 );
                writer.Write( Skeletons.Count );
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
                {
                    foreach ( var skeletonEntry in Skeletons )
                        writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () => skeletonEntry.Write( writer ) );
                } );
                writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
                {
                    foreach ( var skeletonEntry in Skeletons )
                        writer.AddStringToStringTable( skeletonEntry.Name );
                } );
                writer.WriteNulls( 20 );
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public BoneDatabase()
        {
            Skeletons = new List<SkeletonEntry>();
        }
    }
}
