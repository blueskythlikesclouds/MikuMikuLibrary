using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Skeletons;

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

            writer.WriteNulls( 4 * sizeof( uint ) );
        }

        public void Load( Stream source, Skeleton skeleton, MotionDatabase motionDatabase, bool leaveOpen = false )
        {
            Load( source, leaveOpen );

            if ( skeleton == null || motionDatabase == null )
                return;

            foreach ( var motion in Motions )
                motion.Bind( skeleton, motionDatabase );
        }

        public void Load( string filePath, Skeleton skeleton, MotionDatabase motionDatabase )
        {
            using ( var stream = File.OpenRead( filePath ) ) 
                Load( stream, skeleton, motionDatabase );

            if ( motionDatabase == null )
                return;

            string motionSetName = Path.GetFileNameWithoutExtension( filePath );

            if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                motionSetName = motionSetName.Substring( 4 );

            var motionSetInfo = motionDatabase.GetMotionSetInfo( motionSetName );

            if ( motionSetInfo == null || Motions.Count != motionSetInfo.Motions.Count )
                return;

            for ( int i = 0; i < motionSetInfo.Motions.Count; i++ )
            {
                Motions[ i ].Name = motionSetInfo.Motions[ i ].Name;
                Motions[ i ].Id = motionSetInfo.Motions[ i ].Id;
            }
        }

        public void Save( Stream destination, Skeleton skeleton, MotionDatabase motionDatabase, bool leaveOpen = false )
        {
            if ( skeleton != null && motionDatabase != null )
            {
                foreach ( var motion in Motions.Where( x => x.HasBinding ) )
                    motion.Bind().Unbind( skeleton, motionDatabase );
            }

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, Skeleton skeleton, MotionDatabase motionDatabase )
        {
            using ( var stream = File.Create( filePath ) ) 
                Save( stream, skeleton, motionDatabase );
        }

        public MotionSet()
        {
            Motions = new List<Motion>();
        }
    }
}