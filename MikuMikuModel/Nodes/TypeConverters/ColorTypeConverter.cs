using System;
using System.ComponentModel;
using System.Globalization;
using MikuMikuLibrary.Misc;

namespace MikuMikuModel.Nodes.TypeConverters
{
    public class ColorTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) => 
            sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );

        public override bool CanConvertTo( ITypeDescriptorContext context, Type sourceType ) => 
            sourceType == typeof( string ) || base.CanConvertTo( context, sourceType );

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( !( value is string input ) ) 
                return base.ConvertFrom( context, culture, value );

            var floatValues = input.Trim( '<', '>' ).Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );

            float r = float.Parse( floatValues[ 0 ], CultureInfo.InvariantCulture );
            float g = float.Parse( floatValues[ 1 ], CultureInfo.InvariantCulture );
            float b = float.Parse( floatValues[ 2 ], CultureInfo.InvariantCulture );
            float a = float.Parse( floatValues[ 3 ], CultureInfo.InvariantCulture );

            return new Color( r, g, b, a );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            if ( !( value is Color input ) || destinationType != typeof( string ) ) 
                return base.ConvertTo( context, culture, value, destinationType );

            string r = input.R.ToString( CultureInfo.InvariantCulture );
            string g = input.G.ToString( CultureInfo.InvariantCulture );
            string b = input.B.ToString( CultureInfo.InvariantCulture );
            string a = input.A.ToString( CultureInfo.InvariantCulture );

            return $"<{r}, {g}, {b}, {a}>";
        }
    }
}