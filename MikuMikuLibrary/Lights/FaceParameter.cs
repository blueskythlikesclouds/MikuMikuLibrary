using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Lights
{
    public class FaceParameter : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load;

        public float Offset { get; set; } = 1.0f;
        public float Scale { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            var paramReader = new ParameterReader( reader );

            while ( paramReader.Read() )
            {
                switch ( paramReader.HeadToken )
                {
                    case "offset": 
                        Offset = paramReader.ReadSingle();
                        break;

                    case "scale":
                        Scale = paramReader.ReadSingle();
                        break;

                    case "position":
                        Position = paramReader.ReadVector3();
                        break;

                    case "direction":
                        Direction = paramReader.ReadVector3();
                        break;
                }
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            throw new System.NotImplementedException();
        }
    }
}