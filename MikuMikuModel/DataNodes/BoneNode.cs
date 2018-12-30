using MikuMikuLibrary.Models;
using System.ComponentModel;
using System.Numerics;

namespace MikuMikuModel.DataNodes
{
    public class BoneNode : DataNode<Bone>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename; }
        }

        [DisplayName( "Parent ID" )]
        public int ParentID
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }

        public int ID
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }

        public Matrix4x4 Matrix
        {
            get { return GetProperty<Matrix4x4>(); }
            set { SetProperty( value ); }
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        public BoneNode( string name, Bone data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
