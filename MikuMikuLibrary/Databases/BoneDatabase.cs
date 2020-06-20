using System.Collections.Generic;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Skeletons;

namespace MikuMikuLibrary.Databases
{
    public class BoneDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<Skeleton> Skeletons { get; }

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
                        var skeleton = new Skeleton();
                        skeleton.Read( reader );
                        Skeletons.Add( skeleton );
                    } );
                }
            } );

            reader.ReadAtOffset( skeletonNamesOffset, () =>
            {
                foreach ( var skeleton in Skeletons )
                    skeleton.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( 0x09102720 );
            writer.Write( Skeletons.Count );
            writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
            {
                foreach ( var skeleton in Skeletons )
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () => skeleton.Write( writer ) );
            } );
            writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
            {
                foreach ( var skeleton in Skeletons )
                    writer.AddStringToStringTable( skeleton.Name );
            } );
            writer.WriteNulls( 5 * sizeof( uint ) );
        }

        public BoneDatabase()
        {
            Skeletons = new List<Skeleton>();
        }
    }
}