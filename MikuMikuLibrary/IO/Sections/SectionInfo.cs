using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MikuMikuLibrary.IO.Sections
{
    public class SectionInfo
    {
        public Type SectionType { get; }
        public Type DataType { get; }
        public string Signature { get; }

        /// <summary>
        /// Sub sections by section signature
        /// </summary>
        public Dictionary<string, SubSectionInfo> SubSectionInfos { get; }

        public Section Create( Stream source, object dataToRead = null )
        {
            return ( Section )Activator.CreateInstance( SectionType, source, dataToRead ?? Activator.CreateInstance( DataType ) );
        }

        public Section Create( object dataToWrite, Endianness endianness )
        {
            return ( Section )Activator.CreateInstance( SectionType, dataToWrite, endianness );
        }

        public static bool IsSection( Type type )
        {
            return typeof( Section ).IsAssignableFrom( type ) && type.GetCustomAttribute<SectionAttribute>() != null;
        }

        public SectionInfo( Type sectionType )
        {
            var sectionAttribute = sectionType.GetCustomAttribute<SectionAttribute>();
            if ( sectionAttribute == null )
                throw new ArgumentException( "Section type must contain a SectionAttribute attribute", nameof( sectionType ) );

            SectionType = sectionType;
            DataType = sectionAttribute.DataType;
            Signature = sectionAttribute.Signature;

            SubSectionInfos = new Dictionary<string, SubSectionInfo>();

            foreach ( var propertyInfo in sectionType.GetProperties() )
            {
                var subSectionAttribute = propertyInfo.GetCustomAttribute<SubSectionAttribute>();
                if ( subSectionAttribute == null )
                    continue;

                var subSectionInfo = new SubSectionInfo( propertyInfo );
                SubSectionInfos.Add( subSectionInfo.SectionInfo.Signature, subSectionInfo );
            }
        }
    }

    public class SubSectionInfo
    {
        public PropertyInfo PropertyInfo { get; }
        public SectionInfo SectionInfo { get; }
        public int Order { get; }
        public bool IsList { get; }
        public bool IsSectionByDefault { get; }

        public Section SetFromSection( object obj, Stream source )
        {
            Section section;
            var value = PropertyInfo.GetValue( obj );
            bool init = value != null;

            if ( IsList )
            {
                if ( !init )
                    value = Activator.CreateInstance( PropertyInfo.PropertyType );

                if ( IsSectionByDefault )
                    ( value as IList ).Add( ( section = SectionInfo.Create( source ) ) );
                else
                    ( value as IList ).Add( ( section = SectionInfo.Create( source ) ).Data );
            }

            else
            {
                if ( IsSectionByDefault )
                {
                    init = false;
                    value = ( section = SectionInfo.Create( source ) );
                }

                else
                    value = ( section = SectionInfo.Create( source, value ) ).Data;
            }

            if ( !init )
                PropertyInfo.SetValue( obj, value );

            return section;
        }

        public Section GetSection( object obj, Endianness endianness )
        {
            if ( IsList )
                throw new InvalidOperationException( "Attempted to call GetSection on a list subsection" );

            if ( IsSectionByDefault )
                return PropertyInfo.GetValue( obj ) as Section;
            else
                return SectionInfo.Create( PropertyInfo.GetValue( obj ), endianness );
        }

        public IEnumerable<Section> GetSections( object obj, Endianness endianness )
        {
            if ( !IsList )
                throw new InvalidOperationException( "Attempted to call GetSections on a non-list subsection" );

            var value = PropertyInfo.GetValue( obj );
            if ( value == null || !( value is IList list ) )
                yield break;

            if ( IsSectionByDefault )
            {
                foreach ( var item in list )
                {
                    if ( item is Section section )
                        yield return section;
                }
            }

            else
            {
                foreach ( var item in list )
                    yield return SectionInfo.Create( item, endianness );
            }
        }

        public SubSectionInfo( PropertyInfo propertyInfo )
        {
            var subSectionAttribute = propertyInfo.GetCustomAttribute<SubSectionAttribute>();
            if ( subSectionAttribute == null )
                throw new ArgumentException( "Property needs SubSectionAttribute", nameof( propertyInfo ) );

            // Assume the type is already a section type
            if ( subSectionAttribute.SectionType == null )
            {
                var sectionType = propertyInfo.PropertyType;

                // Is it a list of sub sections?
                if ( typeof( IList ).IsAssignableFrom( sectionType ) )
                {
                    // If that's the case, try getting the generic argument
                    var genericArguments = sectionType.GetGenericArguments();

                    if ( genericArguments.Length != 1 )
                        throw new InvalidDataException( "Generic argument length doesn't match 1 for property with SubSectionAttribute(int order)" );

                    sectionType = genericArguments[ 0 ];

                    // See if the generic argument is not a section type
                    if ( !SectionInfo.IsSection( sectionType ) )
                        throw new ArgumentException( "First generic argument for the IList<> property with SubSectionAttribute(int order) is not a section type", nameof( propertyInfo ) );

                    IsList = true;

                    SectionInfo = SectionManager.GetOrRegister( sectionType );
                }
                else
                {
                    if ( !SectionInfo.IsSection( sectionType ) )
                        throw new ArgumentException( "Property with SubSectionAttribute(int order) is not a section type", nameof( propertyInfo ) );

                    SectionInfo = SectionManager.GetOrRegister( sectionType );
                }

                IsSectionByDefault = true;
            }

            else
            {
                var dataType = propertyInfo.PropertyType;
                var sectionInfo = SectionManager.GetOrRegister( subSectionAttribute.SectionType );

                // Is it a list?
                if ( typeof( IList ).IsAssignableFrom( dataType ) )
                {
                    // If that's the case, try getting the generic argument
                    var genericArguments = dataType.GetGenericArguments();

                    if ( genericArguments.Length != 1 )
                        throw new InvalidDataException( "Generic argument length doesn't match 1 for property with SubSectionAttribute(Type sectionType, int order)" );

                    dataType = genericArguments[ 0 ];
                    if ( !dataType.Equals( sectionInfo.DataType ) )
                        throw new ArgumentException( "First generic argument of IList<> doesn't match the section's data type for property with SubSectionAttribute(Type sectionType, int order)" );

                    IsList = true;
                }
                else if ( !dataType.Equals( sectionInfo.DataType ) )
                    throw new ArgumentException( "Section's data type does not match type of property with SubSectionAttribute(Type sectionType, int order)" );

                SectionInfo = sectionInfo;
            }

            PropertyInfo = propertyInfo;
            Order = subSectionAttribute.Order;
        }
    }

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
    public class SectionAttribute : Attribute
    {
        public string Signature { get; }
        public Type DataType { get; }

        public SectionAttribute( string signature, Type dataType )
        {
            Signature = signature ?? throw new ArgumentNullException( nameof( signature ) );
            DataType = dataType ?? throw new ArgumentNullException( nameof( dataType ) );

            if ( Encoding.ASCII.GetByteCount( signature ) != 4 )
                throw new ArgumentException( "Section signature is supposed to be 4 bytes", nameof( signature ) );
        }
    }

    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false )]
    public class SubSectionAttribute : Attribute
    {
        public Type SectionType { get; }
        public int Order { get; }

        public SubSectionAttribute( [CallerLineNumber]int order = int.MaxValue )
        {
            Order = order;
        }

        public SubSectionAttribute( Type sectionType, [CallerLineNumber]int order = int.MaxValue )
        {
            if ( sectionType == null || !typeof( Section ).IsAssignableFrom( sectionType ) )
                throw new ArgumentException( "Section type is either null or doesn't inherit from Section", nameof( sectionType ) );

            SectionType = sectionType;
            Order = order;
        }
    }
}
