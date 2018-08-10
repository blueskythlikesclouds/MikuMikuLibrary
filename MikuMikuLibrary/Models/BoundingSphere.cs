using MikuMikuLibrary.IO.Common;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public struct BoundingSphere
    {
        public Vector3 Center;
        public float Radius;

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( 0 );
            writer.Write( Center );
            writer.Write( Radius );
        }

        public override string ToString()
        {
            return $"<{Center}, {Radius}>";
        }

        internal static BoundingSphere FromReader( EndianBinaryReader reader )
        {
            var boundingSphere = new BoundingSphere();
            reader.SeekCurrent( 4 );
            boundingSphere.Center = reader.ReadVector3();
            boundingSphere.Radius = reader.ReadSingle();
            return boundingSphere;
        }

        public static BoundingSphere FromBoundingBox( BoundingBox boundingBox )
        {
            var boundingSphere = new BoundingSphere();
            boundingSphere.Center = boundingBox.Center;
            boundingSphere.Radius = boundingBox.SizeMax / 2f;
            return boundingSphere;
        }
    }
}
