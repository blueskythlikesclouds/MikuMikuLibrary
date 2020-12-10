using System.Collections.Generic;
using System.IO;
using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Lights
{
    public enum LightType
    {
        Off,
        Parallel,
        Point,
        Spot
    }

    public enum LightId
    {
        Character,
        Stage,
        Sun,
        Reflect,
        Shadow,
        CharacterColor,
        CharacterF,
        Projection
    }

    public class Light
    {
        public LightId Id { get; set; }
        public LightType Type { get; set; }

        public Vector4 Ambient { get; set; }
        public Vector4 Diffuse { get; set; }
        public Vector4 Specular { get; set; }
        public Vector4 Position { get; set; }

        public Vector3 SpotDirection { get; set; }
        public float SpotExponent { get; set; }
        public float SpotCutoff { get; set; }
        public float SpotAttenuationConstant { get; set; }
        public float SpotAttenuationLinear { get; set; }
        public float SpotAttenuationQuadratic { get; set; }

        public Vector4 ClipPlane { get; set; }
        public Vector3 ToneCurve { get; set; }

        internal void Read( ParameterReader paramReader )
        {
            while ( paramReader.Read() )
            {
                switch ( paramReader.HeadToken )
                {
                    case "type":
                        Type = ( LightType ) paramReader.ReadInt32();
                        break;

                    case "ambient":
                        Ambient = paramReader.ReadVector4();
                        break;
                    
                    case "diffuse":
                        Diffuse = paramReader.ReadVector4();
                        break;
                    
                    case "specular":
                        Specular = paramReader.ReadVector4();
                        break;
                    
                    case "position":
                        Position = paramReader.ReadVector4();
                        break;

                    case "spot_direction":
                        SpotDirection = paramReader.ReadVector3();
                        break;

                    case "spot_exponent":
                        SpotExponent = paramReader.ReadSingle();
                        break;

                    case "spot_cutoff":
                        SpotCutoff = paramReader.ReadSingle();
                        break;

                    case "attenuation":
                        SpotAttenuationConstant = paramReader.ReadSingle();
                        SpotAttenuationLinear = paramReader.ReadSingle();
                        SpotAttenuationQuadratic = paramReader.ReadSingle();
                        break;

                    case "clipplane":
                        ClipPlane = paramReader.ReadVector4();
                        break;

                    case "tonecurve":
                        ToneCurve = paramReader.ReadVector3();
                        break;

                    case "id_end":
                        if ( paramReader.ReadInt32() != ( int ) Id )
                            throw new InvalidDataException( $"Expected id end with id {( int ) Id}" );

                        return;
                }
            }
        }
    }

    public class LightGroup
    {
        public uint Id { get; set; }
        public List<Light> Lights { get; }

        internal void Read( ParameterReader paramReader )
        {
            while ( paramReader.Read() )
            {
                switch ( paramReader.HeadToken )
                {
                    case "id_start":
                    {
                        var light = new Light { Id = ( LightId ) paramReader.ReadInt32() };
                        {
                            light.Read( paramReader );
                        }

                        Lights.Add( light );

                        break;
                    }

                    case "group_start":
                        throw new InvalidDataException( "Expected group end before group start" );                        

                    case "group_end":
                        if ( paramReader.ReadUInt32() != Id )
                            throw new InvalidDataException( $"Expected group end with id {Id}" );

                        return;
                }
            }
        }

        public LightGroup()
        {
            Lights = new List<Light>();
        }
    }

    public class LightParameter : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load;

        public List<LightGroup> Groups { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            var paramReader = new ParameterReader( reader );

            while ( paramReader.Read() )
            {
                if ( paramReader.HeadToken != "group_start" )
                    continue;

                var lightGroup = new LightGroup { Id = paramReader.ReadUInt32() };
                {
                    lightGroup.Read( paramReader );
                }

                Groups.Add( lightGroup );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            throw new System.NotImplementedException();
        }

        public LightParameter()
        {
            Groups = new List<LightGroup>();
        }
    }
}