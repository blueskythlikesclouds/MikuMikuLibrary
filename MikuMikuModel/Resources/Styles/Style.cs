using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MikuMikuModel.Resources.Styles
{
    public class Style
    {
        private StyleColorTable mColorTable;
        private StyleToolStripRenderer mToolStripRenderer;

        public string Name { get; set; }

        public SerializableColor Text { get; set; }
        public SerializableColor SelectedText { get; set; }

        public SerializableColor Foreground { get; set; }
        public SerializableColor Background { get; set; }

        public SerializableColor Border { get; set; }

        public SerializableColor ToolStripDropDownBackground { get; set; }

        public SerializableColor MenuItemSelected { get; set; }
        public SerializableColor MenuBorder { get; set; }
        public SerializableColor MenuItemBorder { get; set; }

        public SerializableColor MenuStripGradientBegin { get; set; }
        public SerializableColor MenuStripGradientEnd { get; set; }

        public SerializableColor MenuItemSelectedGradientBegin { get; set; }
        public SerializableColor MenuItemSelectedGradientEnd { get; set; }
        public SerializableColor MenuItemPressedGradientBegin { get; set; }
        public SerializableColor MenuItemPressedGradientEnd { get; set; }        
        
        public SerializableColor ImageMarginGradientBegin { get; set; }
        public SerializableColor ImageMarginGradientMiddle { get; set; }
        public SerializableColor ImageMarginGradientEnd { get; set; }

        public SerializableColor SeparatorDark { get; set; }
        public SerializableColor SeparatorLight { get; set; }

        public SerializableColor ButtonColor { get; set; }
        public SerializableColor ButtonMouseOverColor { get; set; }
        public SerializableColor ButtonMouseDownColor { get; set; }
        public SerializableColor ButtonBorderColor { get; set; }

        public SerializableColor ViewportBackground { get; set; }

        public SerializableColor GridInnerColor { get; set; }
        public SerializableColor GridOuterColor { get; set; }
        public SerializableColor GridXColor { get; set; }
        public SerializableColor GridZColor { get; set; }

        public SerializableColor MenuStripForeground { get; set; }
        public SerializableColor MenuStripBackground { get; set; }

        public StyleColorTable ColorTable => mColorTable ?? ( mColorTable = new StyleColorTable( this ) );

        public StyleToolStripRenderer ToolStripRenderer =>
            mToolStripRenderer ?? ( mToolStripRenderer = new StyleToolStripRenderer( this ) );
    }

    public struct SerializableColor : IXmlSerializable
    {
        public Color Color;

        public byte R => Color.R;
        public byte G => Color.G;
        public byte B => Color.B;
        public byte A => Color.A;

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml( XmlReader reader )
        {
            int r = int.Parse( reader.GetAttribute( "R" ), CultureInfo.InvariantCulture );
            int g = int.Parse( reader.GetAttribute( "G" ), CultureInfo.InvariantCulture );
            int b = int.Parse( reader.GetAttribute( "B" ), CultureInfo.InvariantCulture );
            int a = int.Parse( reader.GetAttribute( "A" ), CultureInfo.InvariantCulture );

            Color = Color.FromArgb( a, r, g, b );
        }

        public void WriteXml( XmlWriter writer )
        {
            writer.WriteAttributeString( "R", Color.R.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteAttributeString( "G", Color.G.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteAttributeString( "B", Color.B.ToString( CultureInfo.InvariantCulture ) );
            writer.WriteAttributeString( "A", Color.A.ToString( CultureInfo.InvariantCulture ) );
        }

        public static implicit operator SerializableColor( Color color ) => 
            new SerializableColor( color );

        public static implicit operator Color( SerializableColor serializableColor ) => 
            serializableColor.Color;

        public SerializableColor( Color color )
        {
            Color = color;
        }
    }
}