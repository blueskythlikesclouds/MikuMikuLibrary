using System;
using System.ComponentModel;
using System.Globalization;

namespace MikuMikuModel.Nodes.TypeConverters
{
    public class IdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) => 
            sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );

        public override bool CanConvertTo( ITypeDescriptorContext context, Type sourceType ) => 
            sourceType == typeof( string ) || base.CanConvertTo( context, sourceType );

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( value is string input )
                return uint.TryParse( input, out uint id ) ? id : 0xFFFFFFFF;

            return base.ConvertFrom( context, culture, value );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType )
        {
            if ( value is uint input && destinationType == typeof( string ) )
                return input == 0xFFFFFFFF ? "Invalid" : input.ToString();

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }
}