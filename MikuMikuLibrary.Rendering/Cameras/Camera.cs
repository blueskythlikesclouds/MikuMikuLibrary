using System.Numerics;
using OpenTK.Input;

namespace MikuMikuLibrary.Rendering.Cameras
{
    public abstract class Camera
    {
        public static readonly Camera Null = new NullCamera();

        public abstract Vector3 Position { get; }

        public abstract void Update( MouseState mouseState, KeyboardState keyboardState );

        public abstract Matrix4x4 GetProjection();
        public abstract Matrix4x4 GetView();

        private sealed class NullCamera : Camera
        {
            public override Vector3 Position { get; } = Vector3.Zero;

            public override void Update( MouseState mouseState, KeyboardState keyboardState )
            {
            }

            public override Matrix4x4 GetProjection() => Matrix4x4.Identity;
            public override Matrix4x4 GetView() => Matrix4x4.Identity;
        }
    }
}
