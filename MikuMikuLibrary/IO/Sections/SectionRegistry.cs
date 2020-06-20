using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MikuMikuLibrary.IO.Sections
{
    public static class SectionRegistry
    {
        private static readonly Dictionary<Type, SectionInfo> sSectionInfosBySectionType =
            new Dictionary<Type, SectionInfo>();

        private static readonly Dictionary<string, SectionInfo> sSectionInfosBySignature =
            new Dictionary<string, SectionInfo>();

        private static readonly Dictionary<Type, SectionInfo> sSingleSectionInfosByDataType =
            new Dictionary<Type, SectionInfo>();

        public static IEnumerable<SectionInfo> SectionInfos => sSectionInfosBySectionType.Values;
        public static IReadOnlyDictionary<Type, SectionInfo> SectionInfosBySectionType => sSectionInfosBySectionType;
        public static IReadOnlyDictionary<string, SectionInfo> SectionInfosBySignature => sSectionInfosBySignature;

        public static IReadOnlyDictionary<Type, SectionInfo> SingleSectionInfosByDataType =>
            sSingleSectionInfosByDataType;

        public static SectionInfo GetOrRegisterSectionInfo( Type sectionType )
        {
            if ( SectionInfosBySectionType.TryGetValue( sectionType, out var sectionInfo ) ) 
                return sectionInfo;

            sectionInfo = new SectionInfo( sectionType );
            sSectionInfosBySectionType[ sectionType ] = sectionInfo;
            sSectionInfosBySignature[ sectionInfo.Signature ] = sectionInfo;
            sSingleSectionInfosByDataType[ sectionInfo.DataType ] = sectionInfo;

            return sectionInfo;
        }

        public static SectionInfo Register<T>() where T : ISection => 
            GetOrRegisterSectionInfo( typeof( T ) );

        static SectionRegistry()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(
                x => typeof( ISection ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );

            foreach ( var type in types )
                GetOrRegisterSectionInfo( type );
        }
    }
}