using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    public class MotionInfo
    {
        public string Name { get; set; }
        public uint Id { get; set; }
    }

    public class MotionSetInfo
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        public List<MotionInfo> Motions { get; }

        internal void Read( EndianBinaryReader reader )
        {
            uint nameOffset = reader.ReadUInt32();
            uint motionNameOffsetsOffset = reader.ReadUInt32();
            int motionCount = reader.ReadInt32();
            uint motionIdsOffset = reader.ReadUInt32();

            Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

            Motions.Capacity = motionCount;

            reader.ReadAtOffset( motionNameOffsetsOffset, () =>
            {
                for ( int i = 0; i < motionCount; i++ )
                {
                    Motions.Add( new MotionInfo
                    {
                        Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated )
                    } );
                }
            } );

            reader.ReadAtOffset( motionIdsOffset, () =>
            {
                foreach ( var motionInfo in Motions )
                    motionInfo.Id = reader.ReadUInt32();
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Name );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var motionInfo in Motions )
                    writer.AddStringToStringTable( motionInfo.Name );
            } );
            writer.Write( Motions.Count );
            writer.ScheduleWriteOffset( 1, 4, AlignmentMode.Left, () =>
            {
                foreach ( var motionInfo in Motions )
                    writer.Write( motionInfo.Id );
            } );
        }

        public MotionSetInfo()
        {
            Motions = new List<MotionInfo>();
        }
    }

    public class MotionDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

        public List<MotionSetInfo> MotionSets { get; }
        public List<string> BoneNames { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int version = reader.ReadInt32();
            uint motionSetsOffset = reader.ReadUInt32();
            uint motionSetIdsOffset = reader.ReadUInt32();
            int motionSetCount = reader.ReadInt32();
            uint boneNameOffsetsOffset = reader.ReadUInt32();
            int boneNameCount = reader.ReadInt32();

            reader.ReadAtOffset( motionSetsOffset, () =>
            {
                MotionSets.Capacity = motionSetCount;

                for ( int i = 0; i < motionSetCount; i++ )
                {
                    var motionSetInfo = new MotionSetInfo();
                    motionSetInfo.Read( reader );
                    MotionSets.Add( motionSetInfo );
                }
            } );

            reader.ReadAtOffset( motionSetIdsOffset, () =>
            {
                foreach ( var motionSetInfo in MotionSets )
                    motionSetInfo.Id = reader.ReadUInt32();
            } );

            reader.ReadAtOffset( boneNameOffsetsOffset, () =>
            {
                BoneNames.Capacity = boneNameCount;

                for ( int i = 0; i < boneNameCount; i++ )
                    BoneNames.Add( reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( 1 );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var motionSetInfo in MotionSets )
                    motionSetInfo.Write( writer );

                writer.WriteNulls( 4 * sizeof( uint ) );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var motionSetInfo in MotionSets )
                    writer.Write( motionSetInfo.Id );
            } );
            writer.Write( MotionSets.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( string boneName in BoneNames )
                    writer.AddStringToStringTable( boneName );
            } );
            writer.Write( BoneNames.Count );
            writer.Align( 64 );
        }

        public MotionSetInfo GetMotionSetInfo( string name ) => 
            MotionSets.FirstOrDefault( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );

        public MotionDatabase()
        {
            MotionSets = new List<MotionSetInfo>();
            BoneNames = new List<string>();
        }
    }
}