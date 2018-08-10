using MikuMikuLibrary.IO.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MikuMikuLibrary.IO.Sections
{
    public static partial class SectionManager
    {
        private static readonly Dictionary<Type, SectionInfo> sectionInfos = new Dictionary<Type, SectionInfo>();
        private static readonly Dictionary<string, SectionInfo> sectionInfosBySignature = new Dictionary<string, SectionInfo>();
        private static readonly Dictionary<Type, SectionInfo> singleSectionInfosByDataType = new Dictionary<Type, SectionInfo>();

        public static IReadOnlyDictionary<Type, SectionInfo> SectionInfos
        {
            get { return sectionInfos; }
        }

        public static IReadOnlyDictionary<string, SectionInfo> SectionInfosBySignature
        {
            get { return sectionInfosBySignature; }
        }

        public static IReadOnlyDictionary<Type, SectionInfo> SingleSectionInfosByDataType
        {
            get { return singleSectionInfosByDataType; }
        }

        public static SectionInfo Register( Type sectionType )
        {
            if ( sectionInfos.ContainsKey( sectionType ) )
                throw new ArgumentException( "Section type is already registered", nameof( sectionType ) );

            if ( !SectionInfo.IsSection( sectionType ) )
                throw new ArgumentException( "Type is not a section type", nameof( sectionType ) );

            var sectionInfo = new SectionInfo( sectionType );

            sectionInfos.Add( sectionType, sectionInfo );
            sectionInfosBySignature.Add( sectionInfo.Signature, sectionInfo );

            if ( !singleSectionInfosByDataType.ContainsKey( sectionInfo.DataType ) )
                singleSectionInfosByDataType.Add( sectionInfo.DataType, sectionInfo );

            return sectionInfo;
        }

        public static bool TryRegister( Type sectionType, out SectionInfo sectionInfo )
        {
            if ( sectionInfos.ContainsKey( sectionType ) )
            {
                sectionInfo = null;
                return false;
            }

            if ( !SectionInfo.IsSection( sectionType ) )
            {
                sectionInfo = null;
                return false;
            }

            sectionInfo = new SectionInfo( sectionType );

            sectionInfos.Add( sectionType, sectionInfo );
            sectionInfosBySignature.Add( sectionInfo.Signature, sectionInfo );

            if ( !singleSectionInfosByDataType.ContainsKey( sectionInfo.DataType ) )
                singleSectionInfosByDataType.Add( sectionInfo.DataType, sectionInfo );

            return true;
        }

        public static SectionInfo GetOrRegister( Type sectionType )
        {
            if ( sectionInfos.TryGetValue( sectionType, out SectionInfo sectionInfo ) )
                return sectionInfo;

            return Register( sectionType );
        }

        public static Section CreateSection( Type sectionType, Stream source, object dataToRead = null )
        {
            return sectionInfos[ sectionType ].Create( source, dataToRead );
        }

        public static Section CreateSection( Type sectionType, object dataToWrite, Endianness endianness )
        {
            return sectionInfos[ sectionType ].Create( dataToWrite, endianness );
        }

#if DEBUG
        public static void PrintSection( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                reader.PushBaseOffset();

                var signature = reader.ReadString( StringBinaryFormat.FixedLength, 4 );
                uint sectionSize = reader.ReadUInt32();
                uint dataOffset = reader.ReadUInt32();
                int endiannessFlag = reader.ReadInt32();
                var endianness = endiannessFlag == 0x18000000 ? Endianness.BigEndian : Endianness.LittleEndian;
                int depth = reader.ReadInt32();
                uint dataSize = reader.ReadUInt32();

                Console.WriteLine( string.Join( "", Enumerable.Repeat( "  ", depth ).Append( signature ) ) );

                reader.ReadAtOffset( dataOffset, () =>
                {
                    var data = reader.ReadBytes( ( int )dataSize );
                    while ( reader.Position < ( reader.PeekBaseOffset() + dataOffset + sectionSize ) )
                        PrintSection( source );
                } );

                reader.SeekBegin( reader.PeekBaseOffset() + dataOffset + sectionSize );
            }
        }
#endif

        static SectionManager()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(
                x => typeof( Section ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );

            foreach ( var type in types )
                TryRegister( type, out _ );
        }
    }
}
