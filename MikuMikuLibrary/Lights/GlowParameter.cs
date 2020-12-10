using System.Collections;
using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Lights
{
    public enum ToneMapType
    {
        YccExponent,
        RgbLinear,
        RgbLinear2
    }

    public enum FadeBlendFunc
    {
        Over,
        Multiply,
        Plus
    }

    public class GlowParameter : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load;

        public float Exposure { get; set; } = 2.0f;

        public float Gamma { get; set; } = 1.0f;

        public uint SaturatePower { get; set; } = 1u;
        public float SaturateCoefficient { get; set; } = 1.0f;

        public float FlarePower { get; set; }
        public float FlareShaft { get; set; }
        public float FlareGhost { get; set; }

        public Vector3 GlareRadius { get; set; } = Vector3.One;
        public Vector3 GlareIntensity { get; set; } = Vector3.One;

        public bool AutoExposure { get; set; } = true;

        public ToneMapType ToneMapType { get; set; }

        public Vector3 FadeColor { get; set; }
        public FadeBlendFunc FadeBlendFunc { get; set; }

        public Vector3 ToneTransStart { get; set; }
        public Vector3 ToneTransEnd { get; set; } = Vector3.One;

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            var paramReader = new ParameterReader( reader );

            while ( paramReader.Read() )
            {
                switch ( paramReader.HeadToken )
                {
                    case "exposure":
                        Exposure = paramReader.ReadSingle();
                        break;

                    case "gamma":
                        Gamma = paramReader.ReadSingle();
                        break;

                    case "saturate_power":
                        SaturatePower = paramReader.ReadUInt32();
                        break;

                    case "saturate_coef":
                        SaturateCoefficient = paramReader.ReadSingle();
                        break;

                    case "flare":
                        FlarePower = paramReader.ReadSingle();
                        FlareShaft = paramReader.ReadSingle();
                        FlareGhost = paramReader.ReadSingle();
                        break;

                    case "sigma":
                        GlareRadius = paramReader.ReadVector3();
                        break;

                    case "intensity":
                        GlareIntensity = paramReader.ReadVector3();
                        break;

                    case "auto_exposure":
                        AutoExposure = paramReader.ReadBoolean();
                        break;

                    case "tone_map_type":
                        ToneMapType = ( ToneMapType ) paramReader.ReadInt32();
                        break;

                    case "fade_color":
                        FadeColor = paramReader.ReadVector3();
                        FadeBlendFunc = ( FadeBlendFunc ) paramReader.ReadInt32();
                        break;

                    case "tone_transform":
                        ToneTransStart = paramReader.ReadVector3();
                        ToneTransEnd = paramReader.ReadVector3();
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