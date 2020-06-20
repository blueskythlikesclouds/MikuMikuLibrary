using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MikuMikuLibrary.IO.Sections
{
    public class SectionInfo
    {
        private readonly Dictionary<SectionInfo, SubSectionInfo> mSubSectionInfos =
            new Dictionary<SectionInfo, SubSectionInfo>();

        public Type SectionType { get; }
        public string Signature { get; }
        public Type DataType { get; }
        public bool IsBinaryFile { get; }

        public IReadOnlyDictionary<SectionInfo, SubSectionInfo> SubSectionInfos => mSubSectionInfos;

        public ISection Create( SectionMode mode, object obj = null ) => 
            ( ISection ) Activator.CreateInstance( SectionType, mode, obj );

        internal SectionInfo( Type sectionType )
        {
            SectionType = sectionType;

            var attribute = sectionType.GetCustomAttribute<SectionAttribute>();

            if ( attribute == null )
                throw new ArgumentException( "Section type is missing section attribute", nameof( sectionType ) );

            Signature = attribute.Signature;

            for ( var type = sectionType.BaseType; type != null; type = type.BaseType )
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Section<> ) )
                {
                    DataType = type.GetGenericArguments()[ 0 ];
                    break;
                }
            }

            if ( DataType == null )
                DataType = attribute.DataType;

            IsBinaryFile = typeof( IBinaryFile ).IsAssignableFrom( DataType );

            foreach ( var propertyInfo in sectionType.GetProperties() )
            {
                var subSectionAttribute = propertyInfo.GetCustomAttribute<SubSectionAttribute>();

                if ( subSectionAttribute == null )
                    continue;

                var subSectionInfo = new SubSectionInfo( propertyInfo );
                mSubSectionInfos.Add( subSectionInfo.SectionInfo, subSectionInfo );
            }
        }
    }

    [AttributeUsage( AttributeTargets.Class )]
    public class SectionAttribute : Attribute
    {
        public string Signature { get; }
        public Type DataType { get; }

        public SectionAttribute( string signature )
        {
            Signature = signature;
        }

        public SectionAttribute( string signature, Type dataType ) : this( signature )
        {
            DataType = dataType;
        }
    }

    public class SubSectionInfo
    {
        public PropertyInfo PropertyInfo { get; }
        public SectionInfo SectionInfo { get; }
        public int Priority { get; }
        public bool IsListType { get; }
        public bool IsSectionType { get; }

        public void ProcessPropertyForReading( ISection section, object obj )
        {
            if ( IsListType )
            {
                var list = PropertyInfo.GetValue( obj ) as IList;
                list.Add( IsSectionType ? section : section.Data );
            }
            else
            {
                PropertyInfo.SetValue( obj, IsSectionType ? section : section.Data );
            }
        }

        public IEnumerable<ISection> ProcessPropertyForWriting( object obj )
        {
            var value = PropertyInfo.GetValue( obj );
            if ( value != null )
            {
                if ( IsListType )
                {
                    var list = value as IList;
                    if ( IsSectionType )
                        foreach ( var item in list )
                            yield return ( ISection ) item;
                    else
                        foreach ( var item in list )
                            yield return SectionInfo.Create( SectionMode.Write, item );
                }
                else
                {
                    if ( IsSectionType )
                        yield return ( ISection ) value;
                    else
                        yield return SectionInfo.Create( SectionMode.Write, value );
                }
            }
        }

        internal SubSectionInfo( PropertyInfo propertyInfo )
        {
            var subSectionAttribute = propertyInfo.GetCustomAttribute<SubSectionAttribute>();

            Type genericType = null;
            for ( var type = propertyInfo.PropertyType; type != null; type = type.BaseType )
            {
                if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( List<> ) )
                {
                    genericType = type.GetGenericArguments()[ 0 ];
                    break;
                }
            }

            if ( genericType != null )
            {
                IsListType = true;
                IsSectionType = typeof( ISection ).IsAssignableFrom( genericType );

                if ( IsSectionType )
                    SectionInfo = SectionRegistry.GetOrRegisterSectionInfo( genericType );
                else
                    SectionInfo = SectionRegistry.GetOrRegisterSectionInfo( subSectionAttribute.SectionType );
            }
            else
            {
                IsSectionType = typeof( ISection ).IsAssignableFrom( propertyInfo.PropertyType );

                if ( IsSectionType )
                    SectionInfo = SectionRegistry.GetOrRegisterSectionInfo( propertyInfo.PropertyType );
                else
                    SectionInfo = SectionRegistry.GetOrRegisterSectionInfo( subSectionAttribute.SectionType );
            }

            PropertyInfo = propertyInfo;
            Priority = subSectionAttribute.Priority;
        }
    }

    [AttributeUsage( AttributeTargets.Property )]
    public class SubSectionAttribute : Attribute
    {
        public int Priority { get; }
        public Type SectionType { get; }

        public SubSectionAttribute( [CallerLineNumber] int priority = int.MaxValue )
        {
            Priority = priority;
        }

        public SubSectionAttribute( Type sectionType, [CallerLineNumber] int priority = int.MaxValue ) 
            : this( priority )
        {
            SectionType = sectionType;
        }
    }
}