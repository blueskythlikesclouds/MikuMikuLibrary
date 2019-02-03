using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Motions
{
    public class MotionSet : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

        public List<Motion> Motions { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            while ( reader.Position < reader.Length )
            {
                long current = reader.Position;
                {
                    if ( reader.ReadOffset() == 0 )
                        break;
                }
                reader.SeekBegin( current );

                var motion = new Motion();
                {
                    motion.Read( reader );
                    Motions.Add( motion );
                }
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            foreach ( var motion in Motions )
                motion.Write( writer );

            writer.WriteNulls( 16 );
        }

        public void Load( Stream source, SkeletonEntry skeletonEntry, MotionDatabase motionDatabase, bool leaveOpen = false )
        {
            Load( source, leaveOpen );

            if ( skeletonEntry == null || motionDatabase == null )
                return;

            foreach ( var motion in Motions )
                motion.GetController( skeletonEntry, motionDatabase );
        }

        public void Load( string filePath, SkeletonEntry skeletonEntry, MotionDatabase motionDatabase )
        {
            using ( var stream = File.OpenRead( filePath ) )
                Load( stream, skeletonEntry, motionDatabase );

            if ( motionDatabase == null )
                return;

            string motionSetName = Path.GetFileNameWithoutExtension( filePath );
            if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                motionSetName = motionSetName.Substring( 4 );

            var motionSetEntry = motionDatabase.GetMotionSet( motionSetName );
            if ( motionSetEntry == null || Motions.Count != motionSetEntry.Motions.Count )
                return;

            for ( int i = 0; i < motionSetEntry.Motions.Count; i++ )
            {
                Motions[ i ].Name = motionSetEntry.Motions[ i ].Name;
                Motions[ i ].Id = motionSetEntry.Motions[ i ].Id;
            }
        }

        public void Save( Stream destination, SkeletonEntry skeletonEntry, MotionDatabase motionDatabase, bool leaveOpen = false )
        {
            if ( skeletonEntry != null && motionDatabase != null )
            {
                foreach ( var motion in Motions.Where( x => x.HasController ) )
                    motion.GetController().Update( skeletonEntry, motionDatabase );
            }

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, SkeletonEntry skeletonEntry, MotionDatabase motionDatabase )
        {
            using ( var stream = File.Create( filePath ) )
                Save( stream, skeletonEntry, motionDatabase );
        }

        public MotionSet()
        {
            Motions = new List<Motion>();
        }
    }
}
