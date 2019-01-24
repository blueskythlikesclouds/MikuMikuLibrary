using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var motion = new Motion();
                if ( !motion.Read( reader ) )
                    break;

                Motions.Add( motion );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            foreach ( var motion in Motions )
                motion.Write( writer );

            writer.WriteNulls( 16 );
        }

        public MotionSet()
        {
            Motions = new List<Motion>();
        }
    }
}
