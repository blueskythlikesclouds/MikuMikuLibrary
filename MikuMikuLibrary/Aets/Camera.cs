using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class Camera
    {
        public FCurve EyeX { get; }
        public FCurve EyeY { get; }
        public FCurve EyeZ { get; }
        public FCurve PositionX { get; }
        public FCurve PositionY { get; }
        public FCurve PositionZ { get; }
        public FCurve DirectionX { get; }
        public FCurve DirectionY { get; }
        public FCurve DirectionZ { get; }
        public FCurve RotationX { get; }
        public FCurve RotationY { get; }
        public FCurve RotationZ { get; }
        public FCurve Zoom { get; }

        internal void Read( EndianBinaryReader reader )
        {
            EyeX.Read( reader );
            EyeY.Read( reader );
            EyeZ.Read( reader );
            PositionX.Read( reader );
            PositionY.Read( reader );
            PositionZ.Read( reader );
            DirectionX.Read( reader );
            DirectionY.Read( reader );
            DirectionZ.Read( reader );
            RotationX.Read( reader );
            RotationY.Read( reader );
            RotationZ.Read( reader );
            Zoom.Read( reader );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            EyeX.Write( writer );
            EyeY.Write( writer );
            EyeZ.Write( writer );
            PositionX.Write( writer );
            PositionY.Write( writer );
            PositionZ.Write( writer );
            DirectionX.Write( writer );
            DirectionY.Write( writer );
            DirectionZ.Write( writer );
            RotationX.Write( writer );
            RotationY.Write( writer );
            RotationZ.Write( writer );
            Zoom.Write( writer );
        }

        public Camera()
        {
            EyeX = new FCurve();
            EyeY = new FCurve();
            EyeZ = new FCurve();
            PositionX = new FCurve();
            PositionY = new FCurve();
            PositionZ = new FCurve();
            DirectionX = new FCurve();
            DirectionY = new FCurve();
            DirectionZ = new FCurve();
            RotationX = new FCurve();
            RotationY = new FCurve();
            RotationZ = new FCurve();
            Zoom = new FCurve();
        }
    }
}