using MikuMikuLibrary.IO;
using System.Drawing;

namespace MikuMikuModel.DataNodes
{
    public abstract class BinaryFileNode<T> : DataNode<T> where T : IBinaryFile
    {
        public override Bitmap Icon
        {
            get { return Properties.Resources.File; }
        }

        public BinaryFormat Format
        {
            get { return GetProperty<BinaryFormat>(); }
            set { SetProperty( value ); }
        }

        public Endianness Endianness
        {
            get { return GetProperty<Endianness>(); }
            set { SetProperty( value ); }
        }

        public BinaryFileNode( string name, T data ) : base( name, data )
        {
        }
    }
}
