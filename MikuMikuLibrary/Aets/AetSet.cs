using System;
using System.Collections.Generic;
using System.Text;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Aets
{
    public class AetSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public override Encoding Encoding { get; } =
            Encoding.GetEncoding( "shift-jis" );

        public List<Scene> Scenes { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            while ( reader.Position < reader.Length )
            {
                long offset = reader.ReadOffset();

                if ( offset == 0 )
                    return;

                reader.ReadAtOffset( offset, () =>
                {
                    var scene = new Scene();
                    scene.Read( reader );

                    Scenes.Add( scene );
                } );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            foreach ( var scene in Scenes )
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () => scene.Write( writer ) );

            writer.WriteNulls( writer.AddressSpace.GetByteSize() );
        }

        public AetSet()
        {
            Scenes = new List<Scene>();
        }
    }
}