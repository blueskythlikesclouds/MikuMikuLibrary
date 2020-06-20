using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections.IO
{
    public abstract class BinaryFileSection<T> : Section<T> where T : IBinaryFile, new()
    {
        protected override void Read( T data, EndianBinaryReader reader, long length )
        {
            data.Format = Format;
            data.Endianness = Endianness;
            {
                data.Read( reader, this );
            }
        }

        protected override void Write( T data, EndianBinaryWriter writer ) => 
            data.Write( writer, this );

        protected BinaryFileSection( SectionMode mode, T data = default ) : base( mode, data )
        {
            if ( mode != SectionMode.Write )
                return;

            Endianness = data.Endianness;
            AddressSpace = data.Format.GetAddressSpace();
        }
    }
}