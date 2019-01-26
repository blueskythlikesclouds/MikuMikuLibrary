using System.Numerics;

namespace MikuMikuLibrary.Motions
{
    public class KeySetVector
    {
        public KeySet X { get; set; }
        public KeySet Y { get; set; }
        public KeySet Z { get; set; }

        public Vector3 Interpolate( float frame )
        {
            float x = X?.Interpolate( frame ) ?? 0;
            float y = Y?.Interpolate( frame ) ?? 0;
            float z = Z?.Interpolate( frame ) ?? 0;

            return new Vector3( x, y, z );
        }
    }
}