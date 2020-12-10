using System;
using System.Numerics;
using System.Runtime.InteropServices;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Rendering
{
    [StructLayout( LayoutKind.Sequential )]
    public struct CameraData
    {
        public Matrix4x4 View;
        public Matrix4x4 Projection;
        public Matrix4x4 LightViewProjection;
        public Vector4 ViewPosition;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct IBLData
    {
        public Matrix4x4 IrradianceR;
        public Matrix4x4 IrradianceG;
        public Matrix4x4 IrradianceB;

        public Matrix4x4 IBLSpace;

        public Vector4 FrontLight;
        public Vector4 BackLight;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct LightData
    {
        public Vector4 Direction;
        public Vector4 Diffuse;
        public Vector4 Ambient;
        public Vector4 Specular;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct SceneData
    {
        public IBLData IBL;
        public LightData CharaLight;
        public LightData StageLight;
    }

    [Flags]
    public enum ShaderFlags : uint
    {
        None = 0,
        Diffuse = 1 << 0,
        Ambient = 1 << 1,
        Normal = 1 << 2,
        Specular = 1 << 3,
        Transparency = 1 << 4,
        Environment = 1 << 5,
        ToonCurve = 1 << 6,

        AnisoDirectionNormal = 1 << 7,
        AnisoDirectionU = 1 << 8,
        AnisoDirectionV = 1 << 9,
        AnisoDirectionRadial = 1 << 10,

        PunchThrough = 1 << 11
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct MaterialData
    {
        public Matrix4x4 DiffuseTransformation;
        public Matrix4x4 AmbientTransformation;

        public Color Diffuse;
        public Color Ambient;
        public Color Specular;
        public Color Emission;

        public Vector4 FresnelCoefficientAndShininess;

        public ShaderFlags ShaderFlags;
        public uint ShaderName; // See MikuMikuLibrary.Materials.Material.ShaderNames
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct ToneMapData
    {
        public Vector4 Exposure;
        public Vector4 ToneOffset;
        public Vector4 ToneScale;
        public Vector4 FadeColorAndFunc;
    }
}
