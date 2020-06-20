using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

namespace MikuMikuModel.Nodes.TypeConverters
{
    public class Vector2TypeConverter : TypeConverter
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

            float x = float.Parse( floatValues[ 0 ], CultureInfo.InvariantCulture );
            float y = float.Parse( floatValues[ 1 ], CultureInfo.InvariantCulture );

            return new Vector2( x, y );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            if ( !( value is Vector2 input ) || destinationType != typeof( string ) ) 
                return base.ConvertTo( context, culture, value, destinationType );

            string x = input.X.ToString( CultureInfo.InvariantCulture );
            string y = input.Y.ToString( CultureInfo.InvariantCulture );

            return $"<{x}, {y}>";
        }
    }

    public class Vector3TypeConverter : TypeConverter
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

            float x = float.Parse( floatValues[ 0 ], CultureInfo.InvariantCulture );
            float y = float.Parse( floatValues[ 1 ], CultureInfo.InvariantCulture );
            float z = float.Parse( floatValues[ 2 ], CultureInfo.InvariantCulture );

            return new Vector3( x, y, z );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            if ( !( value is Vector3 input ) || destinationType != typeof( string ) )
                return base.ConvertTo( context, culture, value, destinationType );

            string x = input.X.ToString( CultureInfo.InvariantCulture );
            string y = input.Y.ToString( CultureInfo.InvariantCulture );
            string z = input.Z.ToString( CultureInfo.InvariantCulture );

            return $"<{x}, {y}, {z}>";
        }
    }

    public class Vector4TypeConverter : TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) => 
            sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );

        public override bool CanConvertTo( ITypeDescriptorContext context, Type sourceType ) => 
            sourceType == typeof( string ) || base.CanConvertTo( context, sourceType );

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if ( !( value is string input ) )
                return base.ConvertFrom( context, culture, value );

            var floatValues = input.Trim( '<', '>' )
                .Split( new[] { "," }, StringSplitOptions.RemoveEmptyEntries );

            float x = float.Parse( floatValues[ 0 ], CultureInfo.InvariantCulture );
            float y = float.Parse( floatValues[ 1 ], CultureInfo.InvariantCulture );
            float z = float.Parse( floatValues[ 2 ], CultureInfo.InvariantCulture );
            float w = float.Parse( floatValues[ 3 ], CultureInfo.InvariantCulture );

            return new Vector4( x, y, z, w );
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            if ( !( value is Vector4 input ) || destinationType != typeof( string ) ) 
                return base.ConvertTo( context, culture, value, destinationType );

            string x = input.X.ToString( CultureInfo.InvariantCulture );
            string y = input.Y.ToString( CultureInfo.InvariantCulture );
            string z = input.Z.ToString( CultureInfo.InvariantCulture );
            string w = input.W.ToString( CultureInfo.InvariantCulture );

            return $"<{x}, {y}, {z}, {w}>";
        }
    }
}