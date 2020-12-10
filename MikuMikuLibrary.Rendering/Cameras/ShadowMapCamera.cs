using System;
using System.Numerics;
using MikuMikuLibrary.Geometry;
using OpenTK.Input;

namespace MikuMikuLibrary.Rendering.Cameras
{
    public class ShadowMapCamera : Camera
    {
        private Vector3 mPosition;

        private Matrix4x4 mProjection;
        private Matrix4x4 mView;

        public override Vector3 Position => mPosition;

        public override void Update( MouseState mouseState, KeyboardState keyboardState )
        {
        }

        public void CalculateMatrices( BoundingSphere boundingSphere, Vector3 lightDirection )
        {
            float radius = ( float ) Math.Sqrt( boundingSphere.Radius * boundingSphere.Radius + boundingSphere.Radius * boundingSphere.Radius );

            mPosition = boundingSphere.Center + lightDirection * boundingSphere.Radius;
            mProjection = Matrix4x4.CreateOrthographic( radius * 2, radius * 2, -radius * 2, radius * 2 );
            mView = Matrix4x4.CreateLookAt( mPosition, boundingSphere.Center, Vector3.UnitY );
        }

        public override Matrix4x4 GetProjection()
        {
            return mProjection;
        }

        public override Matrix4x4 GetView()
        {
            return mView;
        }
    }
}