using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
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
            TypeDescriptor.AddAttributes( typeof( ICollection ), new EditorAttribute( typeof( DirtyCollectionEditor ), typeof( UITypeEditor ) ) );
        }
    }
}