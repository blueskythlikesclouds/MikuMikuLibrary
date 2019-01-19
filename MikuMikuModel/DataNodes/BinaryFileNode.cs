using MikuMikuLibrary.IO;
using MikuMikuModel.Resources;
using System.Drawing;

namespace MikuMikuModel.DataNodes
{
    public abstract class BinaryFileNode<T> : DataNode<T> where T : IBinaryFile
    {
        public override Bitmap Icon => ResourceStore.LoadBitmap( "Icons/File.png" );

        public BinaryFormat Format
        {
            get => GetProperty<BinaryFormat>();
            set => SetProperty( value );
        }

        public Endianness Endianness
        {
            get => GetProperty<Endianness>();
            set => SetProperty( value );
        }

        public BinaryFileNode( string name, T data ) : base( name, data )
        {
        }
    }
}
