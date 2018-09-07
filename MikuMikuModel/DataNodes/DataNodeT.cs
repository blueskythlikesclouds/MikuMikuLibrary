using System;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    public abstract class DataNode<T> : DataNode
    {
        [Browsable( false )]
        public virtual new T Data
        {
            get { return ( T )base.Data; }
            protected set { base.Data = value; }
        }

        public override Type DataType
        {
            get { return typeof( T ); }
        }

        public DataNode( string name, T data ) : base( name, data )
        {
        }
    }
}
