using MikuMikuLibrary.IO;
using System.Drawing;

namespace MikuMikuModel.DataNodes
{
    public abstract class BinaryFileNode<T> : DataNode<T> where T : IBinaryFile
    {
        public override Bitmap Icon => Properties.Resources.File;

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
