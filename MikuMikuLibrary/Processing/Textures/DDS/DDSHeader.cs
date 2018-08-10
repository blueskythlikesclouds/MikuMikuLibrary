//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System.IO;
using System.Text;

namespace MikuMikuLibrary.Processing.Textures.DDS
{
    public class DDSHeader
    {
        public const int Magic = 0x20534444; // 'DDS '

        public int Size { get; set; }

        public DDSHeaderFlags Flags { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int PitchOrLinearSize { get; set; }

        public int Depth { get; set; }

        public int MipMapCount { get; set; }

        public int[] Reserved { get; } = new int[ 11 ];

        public DDSPixelFormat PixelFormat { get; } = new DDSPixelFormat();

        public DDSHeaderCaps Caps { get; set; }

        public DDSHeaderCaps2 Caps2 { get; set; }

        public int Caps3 { get; set; }

        public int Caps4 { get; set; }

        public int Reserved2 { get; set; }

        public DDSHeader()
        {
            Size = 0x7C;
            Flags = DDSHeaderFlags.Caps | DDSHeaderFlags.Height | DDSHeaderFlags.Width | DDSHeaderFlags.PixelFormat;
            Caps = DDSHeaderCaps.Texture;
        }

        public DDSHeader( int width, int height, DDSPixelFormatFourCC format ) : this()
        {
            Height = height;
            Width = width;
            PitchOrLinearSize = DDSFormatDetails.CalculatePitchOrLinearSize( width, height, format, out var additionalFlags );
            Flags |= additionalFlags;
            PixelFormat.FourCC = format;
        }

        public DDSHeader( byte[] data ) : this( new MemoryStream( data ), false ) { }

        public DDSHeader( Stream stream, bool leaveOpen = true )
        {
            using ( var reader = new BinaryReader( stream, Encoding.Default, leaveOpen ) )
                Read( reader );
        }

        public DDSHeader( string file ) : this( File.OpenRead( file ), false ) { }

        public void Save( string file )
        {
            Save( File.OpenWrite( file ), false );
        }

        public void Save( Stream stream, bool leaveOpen = true )
        {
            using ( var writer = new BinaryWriter( stream, Encoding.Default, leaveOpen ) )
                Write( writer );
        }

        public MemoryStream Save()
        {
            var stream = new MemoryStream();
            Save( stream );
            return stream;
        }

        internal void Read( BinaryReader reader )
        {
            var magic = reader.ReadInt32();
            if ( magic != Magic )
                throw new InvalidDataException( "Header magic value did not match the expected value" );

            Size = reader.ReadInt32();
            Flags = ( DDSHeaderFlags )reader.ReadInt32();
            Height = reader.ReadInt32();
            Width = reader.ReadInt32();
            PitchOrLinearSize = reader.ReadInt32();
            Depth = reader.ReadInt32();
            MipMapCount = reader.ReadInt32();

            for ( var i = 0; i < Reserved.Length; i++ )
            {
                Reserved[ i ] = reader.ReadInt32();
            }

            PixelFormat.Read( reader );
            Caps = ( DDSHeaderCaps )reader.ReadInt32();
            Caps2 = ( DDSHeaderCaps2 )reader.ReadInt32();
            Caps3 = reader.ReadInt32();
            Caps4 = reader.ReadInt32();
            Reserved2 = reader.ReadInt32();
        }

        internal void Write( BinaryWriter writer )
        {
            writer.Write( Magic );
            writer.Write( Size );
            writer.Write( ( int )Flags );
            writer.Write( Height );
            writer.Write( Width );
            writer.Write( PitchOrLinearSize );
            writer.Write( Depth );
            writer.Write( MipMapCount );

            for ( var i = 0; i < Reserved.Length; i++ )
            {
                writer.Write( Reserved[ i ] );
            }

            PixelFormat.Write( writer );
            writer.Write( ( int )Caps );
            writer.Write( ( int )Caps2 );
            writer.Write( Caps3 );
            writer.Write( Caps4 );
            writer.Write( Reserved2 );
        }
    }
}