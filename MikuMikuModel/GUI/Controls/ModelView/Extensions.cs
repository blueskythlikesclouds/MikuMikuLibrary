using MikuMikuLibrary.Misc;
using OpenTK;
using OpenTK.Graphics;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public static unsafe class Extensions
    {
        public static Vector2 ToGL( this System.Numerics.Vector2 value )
        {
            return *( Vector2* ) &value;
        }

        public static Vector3 ToGL( this System.Numerics.Vector3 value )
        {
            return *( Vector3* ) &value;
        }

        public static Vector4 ToGL( this System.Numerics.Vector4 value )
        {
            return *( Vector4* ) &value;
        }

        public static Color4 ToGL( this Color value )
        {
            return *( Color4* ) &value;
        }
    }
}