using System.ComponentModel;
using System.Numerics;
using MikuMikuLibrary.Misc;

namespace MikuMikuModel.Nodes.TypeConverters
{
    public static class TypeConverterUtilities
    {
        public static void RegisterTypeConverters()
        {
            TypeDescriptor.AddAttributes( typeof( Color ), new TypeConverterAttribute( typeof( ColorTypeConverter ) ) );
            TypeDescriptor.AddAttributes( typeof( Vector2 ), new TypeConverterAttribute( typeof( Vector2TypeConverter ) ) );
            TypeDescriptor.AddAttributes( typeof( Vector3 ), new TypeConverterAttribute( typeof( Vector3TypeConverter ) ) );
            TypeDescriptor.AddAttributes( typeof( Vector4 ), new TypeConverterAttribute( typeof( Vector4TypeConverter ) ) );
        }
    }
}