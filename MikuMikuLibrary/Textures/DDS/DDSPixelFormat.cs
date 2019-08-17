//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System.IO;

namespace MikuMikuLibrary.Textures.DDS
{
    public class DDSPixelFormat
    {
        public int Size { get; set; }

        public DDSPixelFormatFlags Flags { get; set; }

        public DDSPixelFormatFourCC FourCC { get; set; }

        public int RGBBitCount { get; set; }

        public int RBitMask { get; set; }

        public int GBitMask { get; set; }

        public int BBitMask { get; set; }

        public int ABitMask { get; set; }

        internal void Read( BinaryReader reader )
        {
            Size = reader.ReadInt32();
            Flags = ( DDSPixelFormatFlags ) reader.ReadInt32();
            FourCC = ( DDSPixelFormatFourCC ) reader.ReadInt32();
            RGBBitCount = reader.ReadInt32();
            RBitMask = reader.ReadInt32();
            GBitMask = reader.ReadInt32();
            BBitMask = reader.ReadInt32();
            ABitMask = reader.ReadInt32();
        }

        internal void Write( BinaryWriter writer )
        {
            writer.Write( Size );
            writer.Write( ( int ) Flags );
            writer.Write( ( int ) FourCC );
            writer.Write( RGBBitCount );
            writer.Write( RBitMask );
            writer.Write( GBitMask );
            writer.Write( BBitMask );
            writer.Write( ABitMask );
        }

        public DDSPixelFormat()
        {
            Size = 0x20;
            Flags = DDSPixelFormatFlags.FourCC;
        }

        public DDSPixelFormat( DDSPixelFormatFourCC format ) : this()
        {
            FourCC = format;
        }
    }
}