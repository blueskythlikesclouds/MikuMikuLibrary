using System;
using System.Numerics;

namespace MikuMikuLibrary.Extensions
{
    public static class QuaternionEx
    {
        public static float GetPitch( this Quaternion q )
        {
            float y = 2 * ( q.Y * q.Z + q.W * q.X );
            float x = q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z;

            if ( Math.Abs( x ) < 0.0001 && Math.Abs( y ) < 0.0001 )
                return ( float ) ( 2 * Math.Atan2( q.X, q.W ) );

            return ( float ) Math.Atan2( y, x );
        }

        public static float GetRoll( this Quaternion q ) => 
            ( float ) Math.Atan2( 2 * ( q.X * q.Y + q.W * q.Z ), q.W * q.W + q.X * q.X - q.Y * q.Y - q.Z * q.Z );

        public static float GetYaw( this Quaternion q ) => 
            ( float ) Math.Asin( Math.Max( -1, Math.Min( 1, -2 * ( q.X * q.Z - q.W * q.Y ) ) ) );

        public static Vector3 ToEulerAngles( this Quaternion q ) => 
            new Vector3( q.GetPitch(), q.GetYaw(), q.GetRoll() );
    }
}