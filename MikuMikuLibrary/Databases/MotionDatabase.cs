using System;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Databases
{
    public class MotionEntry
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class MotionSetEntry
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<MotionEntry> Motions { get; }

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
                    var motionEntry = new MotionEntry();
                    motionEntry.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    Motions.Add( motionEntry );
                }
            } );

            reader.ReadAtOffset( motionIdsOffset, () =>
            {
                foreach ( var motionEntry in Motions )
                    motionEntry.Id = reader.ReadInt32();
            } );
        }

        internal void Write( EndianBinaryWriter writer, bool first, bool last )
        {
            int alignment = first ? 16 : 4;
        
            writer.AddStringToStringTable( Name );
            writer.ScheduleWriteOffset( alignment, AlignmentMode.Left, () =>
            {
                foreach ( var motionEntry in Motions )
                    writer.AddStringToStringTable( motionEntry.Name );
                
                if ( last )
                    writer.WriteOffset( 0 );
            } );
            writer.Write( Motions.Count );
            writer.ScheduleWriteOffset( 1, alignment, AlignmentMode.Left, () =>
            {
                foreach ( var motionEntry in Motions )
                    writer.Write( motionEntry.Id );
                    
                if ( last )
                    writer.Write( 0 );
            } );
        }

        public MotionSetEntry()
        {
            Motions = new List<MotionEntry>();
        }
    }

    public class MotionDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

        public List<MotionSetEntry> MotionSets { get; }
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
                    var motionSetEntry = new MotionSetEntry();
                    motionSetEntry.Read( reader );
                    MotionSets.Add( motionSetEntry );
                }
            } );

            reader.ReadAtOffset( motionSetIdsOffset, () =>
            {
                foreach ( var motionSetEntry in MotionSets )
                    motionSetEntry.Id = reader.ReadInt32();
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
                foreach ( var motionSetEntry in MotionSets )
                {
                    bool first = motionSetEntry == MotionSets[ 0 ];
                    bool last = motionSetEntry == MotionSets[ MotionSets.Count - 1 ];
                    
                    motionSetEntry.Write( writer, first, last );
                }
                
                writer.WriteNulls( 16 );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var motionSetEntry in MotionSets )
                    writer.Write( motionSetEntry.Id );
            } );
            writer.Write( MotionSets.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( string boneName in BoneNames )
                    writer.AddStringToStringTable( boneName );
            } );
            writer.Write( BoneNames.Count );
            writer.WriteAlignmentPadding( 64 );
        }

        public MotionSetEntry GetMotionSet( string name ) =>
            MotionSets.FirstOrDefault( x => x.Name.Equals( name, StringComparison.OrdinalIgnoreCase ) );

        public MotionDatabase()
        {
            MotionSets = new List<MotionSetEntry>();
            BoneNames = new List<string>();
        }
    }
}
