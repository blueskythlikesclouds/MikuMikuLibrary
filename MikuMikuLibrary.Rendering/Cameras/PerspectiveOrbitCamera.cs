using System;
using System.Numerics;
using MikuMikuLibrary.Geometry;
using OpenTK;
using OpenTK.Input;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace MikuMikuLibrary.Rendering.Cameras
{
    public sealed class PerspectiveOrbitCamera : Camera
    {
        private const Key cSpeedUpKey = Key.ShiftLeft;
        private const Key cSlowDownKey = Key.AltLeft;

        private const float cSpeed = 0.1f;
        private const float cSpeedFast = 0.8f;
        private const float cSpeedSlow = 0.025f;

        private const float cWheelSpeed = 0.05f;
        private const float cWheelSpeedFast = 10.4f;
        private const float cWheelSpeedSlow = 0.0125f;

        private ButtonState mPreviousLeftButton;
        private int mMousePreviousX;
        private int mMousePreviousY;
        private int mMousePreviousWheel;

        private Vector3 mViewPoint = Vector3.Zero;
        private Vector3 mInterest = Vector3.Zero;

        private Vector2 mOrbitRotation = Vector2.Zero;
        private float mOrbitDistance = 0.0f;

        private float mOrbitDistanceMin = 0.1f;
        private float mOrbitDistanceMax = 100000.0f;

        public override Vector3 Position => mViewPoint;

        public float FieldOfView { get; set; } = MathHelper.DegreesToRadians( 65.0f );
        public float AspectRatio { get; set; } = 16.0f / 9.0f;

        public float ZNear { get; set; } = 0.5f;
        public float ZFar { get; set; } = 10000.0f;

        public void Reset( BoundingSphere boundingSphere )
        {
            float distance = ( float ) ( boundingSphere.Radius * 2f / Math.Tan( MathHelper.DegreesToRadians( 65.0f ) ) ) + 0.75f;

            mInterest = boundingSphere.Center;
            mOrbitRotation = Vector2.Zero;
            mOrbitDistance = distance;
            mViewPoint = mInterest + CalculateCameraOrbitPosition();
        }

        public override void Update( MouseState mouseState, KeyboardState keyboardState )
        {
            int deltaX = mouseState.X - mMousePreviousX;
            int deltaY = mouseState.Y - mMousePreviousY;
            int deltaWheel = mouseState.Wheel - mMousePreviousWheel;

            if ( mouseState.LeftButton == ButtonState.Pressed && mPreviousLeftButton == ButtonState.Pressed )
            {
                mOrbitRotation.X -= deltaX * 0.18f;
                mOrbitRotation.Y -= deltaY * 0.18f;
                mOrbitRotation.Y = MathHelper.Clamp( mOrbitRotation.Y, -89.9f, +89.9f );
            }

            float wheelSpeed =
                keyboardState.IsKeyDown( cSpeedUpKey ) ? cWheelSpeedFast :
                keyboardState.IsKeyDown( cSlowDownKey ) ? cWheelSpeedSlow : cWheelSpeed;

            mOrbitDistance = MathHelper.Clamp( mOrbitDistance - ( wheelSpeed * deltaWheel ), mOrbitDistanceMin,
                mOrbitDistanceMax );

            var frontDirection = mInterest - mViewPoint;
            frontDirection.Y = 0.0f;
            frontDirection = Vector3.Normalize( frontDirection );

            float speed =
                keyboardState.IsKeyDown( cSpeedUpKey ) ? cSpeedFast :
                keyboardState.IsKeyDown( cSlowDownKey ) ? cSpeedSlow : cSpeed;

            bool front = keyboardState.IsKeyDown( Key.W );
            bool back = keyboardState.IsKeyDown( Key.S );
            bool left = keyboardState.IsKeyDown( Key.A );
            bool right = keyboardState.IsKeyDown( Key.D );
            bool up = keyboardState.IsKeyDown( Key.Space );
            bool down = keyboardState.IsKeyDown( Key.ControlLeft );

            if ( front && !back )
                mInterest += frontDirection * speed;

            else if ( back && !front )
                mInterest -= frontDirection * speed;

            if ( left && !right )
                mInterest -= Vector3.Normalize( Vector3.Cross( frontDirection, Vector3.UnitY ) ) * speed;

            else if ( right && !left )
                mInterest += Vector3.Normalize( Vector3.Cross( frontDirection, Vector3.UnitY ) ) * speed;

            if ( up && !down )
                mInterest += Vector3.UnitY * speed / 2;

            else if ( !up && down )
                mInterest -= Vector3.UnitY * speed / 2;

            mViewPoint = mInterest + CalculateCameraOrbitPosition();

            mPreviousLeftButton = mouseState.LeftButton;
            mMousePreviousX = mouseState.X;
            mMousePreviousY = mouseState.Y;
            mMousePreviousWheel = mouseState.Wheel;
        }

        public override Matrix4x4 GetProjection()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView( FieldOfView, AspectRatio, ZNear, ZFar );
        }

        public override Matrix4x4 GetView()
        {
            return Matrix4x4.CreateLookAt( mViewPoint, mInterest, Vector3.UnitY );
        }

        private Vector3 CalculateCameraOrbitPosition()
        {
            return Vector3.Transform( new Vector3( 0.0f, 0.0f, mOrbitDistance ),
                Matrix4x4.CreateRotationX( MathHelper.DegreesToRadians( mOrbitRotation.Y ) ) *
                Matrix4x4.CreateRotationY( MathHelper.DegreesToRadians( mOrbitRotation.X ) ) );
        }
    }
}