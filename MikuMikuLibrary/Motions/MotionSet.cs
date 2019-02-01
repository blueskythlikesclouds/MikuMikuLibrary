using System;
using System.Collections.Generic;
using System.IO;
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

        public void Load( string filePath, MotionDatabase motionDatabase )
        {
            base.Load( filePath );

            string motionSetName = Path.GetFileNameWithoutExtension( filePath );
            if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                motionSetName = motionSetName.Substring( 4 );

            var motionSetEntry = motionDatabase.GetMotionSet( motionSetName );
            if ( motionSetEntry == null || Motions.Count != motionSetEntry.Motions.Count )
                return;

            for ( int i = 0; i < motionSetEntry.Motions.Count; i++ )
            {
                Motions[ i ].Name = motionSetEntry.Motions[ i ].Name;
                Motions[ i ].ID = motionSetEntry.Motions[ i ].ID;
            }
        }

        public MotionSet()
        {
            Motions = new List<Motion>();
        }
    }
}
