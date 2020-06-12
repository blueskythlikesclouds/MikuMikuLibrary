using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MikuMikuLibrary.Motions
{
    public class KeyBinding
    {
        public KeySet X { get; set; }
        public KeySet Y { get; set; }
        public KeySet Z { get; set; }

        public IEnumerable<ushort> Frames
        {
            get
            {
                var enumerable = Enumerable.Empty<ushort>();
                if ( X != null )
                    enumerable = enumerable.Concat( X.Keys.Select( x => x.Frame ) );
                if ( Y != null )
                    enumerable = enumerable.Concat( Y.Keys.Select( x => x.Frame ) );
                if ( Z != null )
                    enumerable = enumerable.Concat( Z.Keys.Select( x => x.Frame ) );

                return enumerable.Distinct().OrderBy( x => x );
            }
        }

        public void Merge( KeyBinding other )
        {
            if ( X == null )
                X = other.X;
            else if ( other.X != null )
                X.Merge( other.X );

            if ( Y == null )
                Y = other.Y;
            else if ( other.Y != null )
                Y.Merge( other.Y );

            if ( Z == null )
                Z = other.Z;
            else if ( other.Z != null )
                Z.Merge( other.Z );
        }

        public void Sort()
        {
            X?.Sort();
            Y?.Sort();
            Z?.Sort();
        }

        public Vector3 Interpolate( float frame )
        {
            float x = X?.Interpolate( frame ) ?? 0;
            float y = Y?.Interpolate( frame ) ?? 0;
            float z = Z?.Interpolate( frame ) ?? 0;

            return new Vector3( x, y, z );
        }
    }
}