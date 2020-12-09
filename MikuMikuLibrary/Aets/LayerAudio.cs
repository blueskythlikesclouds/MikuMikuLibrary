using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class LayerAudio
    {
        public FCurve VolumeL { get; set; }
        public FCurve VolumeR { get; set; }
        public FCurve PanL { get; set; }
        public FCurve PanR { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            VolumeL.Read( reader );
            VolumeR.Read( reader );
            PanL.Read( reader );
            PanR.Read( reader );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            VolumeL.Write( writer );
            VolumeR.Write( writer );
            PanL.Write( writer );
            PanR.Write( writer );
        }

        public LayerAudio()
        {
            VolumeL = new FCurve();
            VolumeR = new FCurve();
            PanL = new FCurve();
            PanR = new FCurve();
        }
    }
}