using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    public class MotionEntry
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public class MotionSetEntry
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public List<MotionEntry> Motions { get; }

        internal void Read( EndianBinaryReader reader )
        {
            uint nameOffset = reader.ReadUInt32();
            uint motionNameOffsetsOffset = reader.ReadUInt32();
            int motionCount = reader.ReadInt32();
            uint motionIDsOffset = reader.ReadUInt32();

            Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

            Motions.Capacity = motionCount;
            reader.ReadAtOffsetAndSeekBack( motionNameOffsetsOffset, () =>
            {
                for ( int i = 0; i < motionCount; i++ )
                {
                    var motionEntry = new MotionEntry();
                    motionEntry.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                    Motions.Add( motionEntry );
                }
            } );

            reader.ReadAtOffsetAndSeekBack( motionIDsOffset, () =>
            {
                foreach ( var motionEntry in Motions )
                    motionEntry.ID = reader.ReadInt32();
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Name );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var motionEntry in Motions )
                    writer.AddStringToStringTable( motionEntry.Name );
            } );
            writer.Write( Motions.Count );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var motionEntry in Motions )
                    writer.Write( motionEntry.ID );
            } );
        }

        public MotionSetEntry()
        {
            Motions = new List<MotionEntry>();
        }
    }

    public class MotionDatabase : BinaryFile
    {
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public List<MotionSetEntry> MotionSets { get; }
        public List<string> BoneNames { get; }

        protected override void InternalRead( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                int version = reader.ReadInt32();
                uint motionSetsOffset = reader.ReadUInt32();
                uint motionSetIDsOffset = reader.ReadUInt32();
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

                reader.ReadAtOffset( motionSetIDsOffset, () =>
                {
                    foreach ( var motionSetEntry in MotionSets )
                        motionSetEntry.ID = reader.ReadInt32();
                } );

                reader.ReadAtOffset( boneNameOffsetsOffset, () =>
                {
                    BoneNames.Capacity = boneNameCount;
                    for ( int i = 0; i < boneNameCount; i++ )
                        BoneNames.Add( reader.ReadStringPtr( StringBinaryFormat.NullTerminated ) );
                } );
            }
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( 1 );
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var motionSetEntry in MotionSets )
                        motionSetEntry.Write( writer );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var motionSetEntry in MotionSets )
                        writer.Write( motionSetEntry.ID );
                } );
                writer.Write( MotionSets.Count );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );

                    foreach ( var boneName in BoneNames )
                        writer.AddStringToStringTable( boneName );
                } );
                writer.Write( BoneNames.Count );
                writer.WriteAlignmentPadding( 64 );
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public MotionDatabase()
        {
            MotionSets = new List<MotionSetEntry>();
            BoneNames = new List<string>();
        }
    }
}
