using MikuMikuLibrary.Misc;
using System;
using System.ComponentModel;
using System.Globalization;

namespace MikuMikuModel.DataNodes.TypeConverters
{
    public class ColorTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) =>
            sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );

        public override bool CanConvertTo( ITypeDescriptorContext context, Type sourceType ) =>
             sourceType == typeof( string ) || base.CanConvertTo( context, sourceType );

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( value is string input )
            {
                var floatValues = input.Trim( '<', '>' )
                    .Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );

                var r = float.Parse( floatValues[ 0 ], CultureInfo.InvariantCulture );
                var g = float.Parse( floatValues[ 1 ], CultureInfo.InvariantCulture );
                var b = float.Parse( floatValues[ 2 ], CultureInfo.InvariantCulture );
                var a = float.Parse( floatValues[ 3 ], CultureInfo.InvariantCulture );

                return new Color( r, g, b, a );
            }

            return base.ConvertFrom( context, culture, value );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            if ( value is Color input && destinationType == typeof( string ) )
            {
                var r = input.R.ToString( CultureInfo.InvariantCulture );
                var g = input.G.ToString( CultureInfo.InvariantCulture );
                var b = input.B.ToString( CultureInfo.InvariantCulture );
                var a = input.A.ToString( CultureInfo.InvariantCulture );

                return $"<{r}, {g}, {b}, {a}>";
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }
}
