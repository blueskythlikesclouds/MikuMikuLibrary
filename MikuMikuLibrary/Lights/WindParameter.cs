using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Lights
{
    public class WindParameter : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load;

        public float Scale { get; set; }
        public float Cycle { get; set; }
        public Vector2 Rotation { get; set; }
        public float Bias { get; set; }
        public Vector2[] Spc { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            var paramReader = new ParameterReader( reader );

            while ( paramReader.Read() )
            {
                switch ( paramReader.HeadToken )
                {
                    case "scale":
                        Scale = paramReader.ReadSingle();
                        break;

                    case "cycle":
                        Cycle = paramReader.ReadSingle();
                        break;

                    case "rot":
                        Rotation = paramReader.ReadVector2();
                        break;

                    case "bias":
                        Bias = paramReader.ReadSingle();
                        break;

                    case "spc":
                        Spc[ paramReader.ReadInt32() ] = paramReader.ReadVector2();
                        break;
                }
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            throw new System.NotImplementedException();
        }

        public WindParameter()
        {
            Spc = new Vector2[ 16 ];
        }
    }
}