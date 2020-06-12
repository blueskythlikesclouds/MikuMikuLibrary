using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using MikuMikuLibrary.Materials;
using MikuMikuModel.Nodes.TypeConverters;
using MikuMikuModel.Resources;

namespace MikuMikuModel.Nodes.Materials
{
    public class MaterialTextureNode : Node<MaterialTexture>
    {
        private static readonly XmlSerializer sSerializer = new XmlSerializer( typeof( MaterialTexture ) );

        public override NodeFlags Flags => NodeFlags.None;
        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/MaterialTexture.png" );

        [DisplayName( "Texture id" )]
        [TypeConverter( typeof( IdTypeConverter ) )]
        public uint TextureId
        {
            get => GetProperty<uint>();
            set => SetProperty( value );
        }

        [DisplayName( "Is active" )] public bool IsActive => GetProperty<bool>();

        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int Field00
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int Field01
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [TypeConverter( typeof( Int32HexTypeConverter ) )]
        public int Field02
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public float Field03
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field04
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field05
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field06
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field07
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field08
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field09
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field10
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field11
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field12
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field13
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field14
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field15
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field16
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field17
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field18
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field19
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field20
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field21
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field22
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field23
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field24
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field25
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field26
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field27
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        public float Field28
        {
            get => GetProperty<float>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
            RegisterCustomHandler( "Copy values", () =>
                {
                    using ( var stringWriter = new StringWriter( CultureInfo.InvariantCulture ) )
                    {
                        sSerializer.Serialize( stringWriter, Data );
                        Clipboard.SetText( stringWriter.ToString() );
                    }
                }, Keys.Control | Keys.C );
            RegisterCustomHandler( "Paste values", () =>
                {
                    try
                    {
                        using ( var stringReader = new StringReader( Clipboard.GetText() ) )
                        {
                            var materialTexture = ( MaterialTexture ) sSerializer.Deserialize( stringReader );

                            // holy hell this looks BAD
                            SetProperty( materialTexture.Field00, nameof( materialTexture.Field00 ) );
                            SetProperty( materialTexture.Field01, nameof( materialTexture.Field01 ) );
                            SetProperty( materialTexture.Field02, nameof( materialTexture.Field02 ) );
                            SetProperty( materialTexture.Field03, nameof( materialTexture.Field03 ) );
                            SetProperty( materialTexture.Field04, nameof( materialTexture.Field04 ) );
                            SetProperty( materialTexture.Field05, nameof( materialTexture.Field05 ) );
                            SetProperty( materialTexture.Field06, nameof( materialTexture.Field06 ) );
                            SetProperty( materialTexture.Field07, nameof( materialTexture.Field07 ) );
                            SetProperty( materialTexture.Field08, nameof( materialTexture.Field08 ) );
                            SetProperty( materialTexture.Field09, nameof( materialTexture.Field09 ) );
                            SetProperty( materialTexture.Field10, nameof( materialTexture.Field10 ) );
                            SetProperty( materialTexture.Field11, nameof( materialTexture.Field11 ) );
                            SetProperty( materialTexture.Field12, nameof( materialTexture.Field12 ) );
                            SetProperty( materialTexture.Field13, nameof( materialTexture.Field13 ) );
                            SetProperty( materialTexture.Field14, nameof( materialTexture.Field14 ) );
                            SetProperty( materialTexture.Field15, nameof( materialTexture.Field15 ) );
                            SetProperty( materialTexture.Field16, nameof( materialTexture.Field16 ) );
                            SetProperty( materialTexture.Field17, nameof( materialTexture.Field17 ) );
                            SetProperty( materialTexture.Field18, nameof( materialTexture.Field18 ) );
                            SetProperty( materialTexture.Field19, nameof( materialTexture.Field19 ) );
                            SetProperty( materialTexture.Field20, nameof( materialTexture.Field20 ) );
                            SetProperty( materialTexture.Field21, nameof( materialTexture.Field21 ) );
                            SetProperty( materialTexture.Field22, nameof( materialTexture.Field22 ) );
                            SetProperty( materialTexture.Field23, nameof( materialTexture.Field23 ) );
                            SetProperty( materialTexture.Field24, nameof( materialTexture.Field24 ) );
                            SetProperty( materialTexture.Field25, nameof( materialTexture.Field25 ) );
                            SetProperty( materialTexture.Field26, nameof( materialTexture.Field26 ) );
                            SetProperty( materialTexture.Field27, nameof( materialTexture.Field27 ) );
                            SetProperty( materialTexture.Field28, nameof( materialTexture.Field28 ) );
                        }
                    }
                    catch
                    {
                        MessageBox.Show( "There is no valid data to paste.", Program.Name, MessageBoxButtons.OK,
                            MessageBoxIcon.Error );
                    }
                }, Keys.Control | Keys.V );
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public MaterialTextureNode( string name, MaterialTexture data ) : base( name, data )
        {
        }
    }
}