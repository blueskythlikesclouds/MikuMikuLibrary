using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class LayerVideo
    {
        public TransferMode TransferMode { get; }
        public FCurve AnchorX { get; }
        public FCurve AnchorY { get; }
        public FCurve PositionX { get; }
        public FCurve PositionY { get; }
        public FCurve Rotation { get; }
        public FCurve ScaleX { get; }
        public FCurve ScaleY { get; }
        public FCurve Opacity { get; }
        public LayerVideo3D Video3D { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            TransferMode.Read( reader );

            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SeekCurrent( 4 );

            AnchorX.Read( reader );
            AnchorY.Read( reader );
            PositionX.Read( reader );
            PositionY.Read( reader );
            Rotation.Read( reader );
            ScaleX.Read( reader );
            ScaleY.Read( reader );
            Opacity.Read( reader );

            reader.ReadOffset( () =>
            {
                Video3D = new LayerVideo3D();
                Video3D.Read( reader );
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            TransferMode.Write( writer );

            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 4 );

            AnchorX.Write( writer );
            AnchorY.Write( writer );
            PositionX.Write( writer );
            PositionY.Write( writer );
            Rotation.Write( writer );
            ScaleX.Write( writer );
            ScaleY.Write( writer );
            Opacity.Write( writer );

            writer.ScheduleWriteOffsetIf( Video3D != null, 8, AlignmentMode.Left, () =>
            {
                Video3D.Write( writer );
            } );
        }

        public LayerVideo()
        {
            TransferMode = new TransferMode();
            AnchorX = new FCurve();
            AnchorY = new FCurve();
            PositionX = new FCurve();
            PositionY = new FCurve();
            Rotation = new FCurve();
            ScaleX = new FCurve();
            ScaleY = new FCurve();
            Opacity = new FCurve();
        }
    }
}