using System.Collections.Generic;
using System.IO;
using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Lights
{
    public enum FogType
    {
        None,
        Linear,
        Exp,
        Exp2
    }

    public class FogGroup
    {
        public uint Id { get; set; }
        public FogType Type { get; set; }
        public float Density { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public Vector4 Color { get; set; }

        internal void Read( ParameterReader paramReader )
        {
            while ( paramReader.Read() )
            {
                switch ( paramReader.HeadToken )
                {
                    case "type":
                        Type = ( FogType ) paramReader.ReadInt32();
                        break;

                    case "density":
                        Density = paramReader.ReadSingle();
                        break;

                    case "linear":
                        Start = paramReader.ReadSingle();
                        End = paramReader.ReadSingle();
                        break;

                    case "color":
                        Color = paramReader.ReadVector4();
                        break;

                    case "group_start":
                        throw new InvalidDataException( "Expected group end before group start" );                        

                    case "group_end":
                        if ( paramReader.ReadUInt32() != Id )
                            throw new InvalidDataException( $"Expected group end with id {Id}" );

                        return;
                }
            }
        }
    }

    public class FogParameter : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load;

        public List<FogGroup> Groups { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            var paramReader = new ParameterReader( reader );

            while ( paramReader.Read() )
            {
                if ( paramReader.HeadToken != "group_start" ) 
                    continue;

                var group = new FogGroup { Id = paramReader.ReadUInt32() };
                {
                    group.Read( paramReader );
                }

                Groups.Add( group );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            throw new System.NotImplementedException();
        }

        public FogParameter()
        {
            Groups = new List<FogGroup>();
        }
    }
}