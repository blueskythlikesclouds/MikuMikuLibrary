using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class Marker
    {
        public float Frame { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Frame = reader.ReadSingle();
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Frame );
            writer.AddStringToStringTable( Name );
        }
    }
}