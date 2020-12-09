using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class LayerVideo3D
    {
        public FCurve AnchorZ { get; }
        public FCurve PositionZ { get; }
        public FCurve DirectionX { get; }
        public FCurve DirectionY { get; }
        public FCurve DirectionZ { get; }
        public FCurve RotationX { get; }
        public FCurve RotationY { get; }
        public FCurve ScaleZ { get; }

        internal void Read( EndianBinaryReader reader )
        {
            AnchorZ.Read( reader );
            PositionZ.Read( reader );
            DirectionX.Read( reader );
            DirectionY.Read( reader );
            DirectionZ.Read( reader );
            RotationX.Read( reader );
            RotationY.Read( reader );
            ScaleZ.Read( reader );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            AnchorZ.Write( writer );
            PositionZ.Write( writer );
            DirectionX.Write( writer );
            DirectionY.Write( writer );
            DirectionZ.Write( writer );
            RotationX.Write( writer );
            RotationY.Write( writer );
            ScaleZ.Write( writer );
        }

        public LayerVideo3D()
        {
            AnchorZ = new FCurve();
            PositionZ = new FCurve();
            DirectionX = new FCurve();
            DirectionY = new FCurve();
            DirectionZ = new FCurve();
            RotationX = new FCurve();
            RotationY = new FCurve();
            ScaleZ = new FCurve();
        }
    }
}