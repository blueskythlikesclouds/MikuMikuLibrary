using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Lights;
using MikuMikuLibrary.Misc;
using OpenTK.Graphics;

namespace MikuMikuLibrary.Rendering.Extensions
{
    public static unsafe class MMLEx
    {
        public static Color4 ToGL( this Color value ) => *( Color4* ) &value;

        public static LightData ToLightData( this Light light ) =>
            new LightData
            {
                Diffuse = light.Diffuse,
                Ambient = light.Ambient,
                Specular = light.Specular,
                Direction = new Vector4( Vector3.Normalize( light.Position.To3D() ), 0.0f )
            };
    }
}