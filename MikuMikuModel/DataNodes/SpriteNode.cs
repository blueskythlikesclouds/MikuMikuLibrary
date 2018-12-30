using MikuMikuLibrary.Sprites;
using System.ComponentModel;

namespace MikuMikuModel.DataNodes
{
    public class SpriteNode : DataNode<Sprite>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Leaf; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get { return DataNodeActionFlags.Move | DataNodeActionFlags.Remove | DataNodeActionFlags.Rename; }
        }

        public int Field00
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public int Field01
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        [DisplayName( "Texture index" )]
        public int TextureIndex
        {
            get { return GetProperty<int>(); }
            set { SetProperty( value ); }
        }
        public float Field02
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field03
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field04
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field05
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Field06
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float X
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Y
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Width
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }
        public float Height
        {
            get { return GetProperty<float>(); }
            set { SetProperty( value ); }
        }

        protected override void InitializeCore()
        {
        }

        protected override void InitializeViewCore()
        {
        }

        protected override void OnRename( string oldName )
        {
            SetProperty( Name, nameof( Data.Name ) );
            base.OnRename( oldName );
        }

        public SpriteNode( string name, Sprite data ) :
            base( string.IsNullOrEmpty( data.Name ) ? name : data.Name, data )
        {
        }
    }
}
