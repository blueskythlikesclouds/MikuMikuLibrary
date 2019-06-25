﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace MikuMikuModel.Nodes.TypeConverters
{
    public class Int32HexTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) =>
            sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );

        public override bool CanConvertTo( ITypeDescriptorContext context, Type sourceType ) =>
            sourceType == typeof( string ) || base.CanConvertTo( context, sourceType );

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( value is string input )
            {
                if ( input.StartsWith( "0x" ) )
                    input = input.Substring( 2 );

                return int.Parse( input, NumberStyles.HexNumber, culture );
            }

            return base.ConvertFrom( context, culture, value );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType )
        {
            if ( value is int && destinationType == typeof( string ) )
                return string.Format( "0x{0:X8}", value );

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }

    public class UInt32HexTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) =>
            sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );

        public override bool CanConvertTo( ITypeDescriptorContext context, Type sourceType ) =>
            sourceType == typeof( string ) || base.CanConvertTo( context, sourceType );

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( value is string input )
            {
                if ( input.StartsWith( "0x" ) )
                    input = input.Substring( 2 );

                return uint.Parse( input, NumberStyles.HexNumber, culture );
            }

            return base.ConvertFrom( context, culture, value );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destinationType )
        {
            if ( value is uint && destinationType == typeof( string ) )
                return string.Format( "0x{0:X8}", value );

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }
}
