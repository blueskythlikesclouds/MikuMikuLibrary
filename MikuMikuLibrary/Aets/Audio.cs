using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Aets
{
    public class Audio
    {
        internal long ReferenceOffset { get; private set; }
        
        public uint SoundId { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            ReferenceOffset = reader.Offset;

            SoundId = reader.ReadUInt32();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            ReferenceOffset = writer.Offset;

            writer.Write( SoundId );
        }
    }
}