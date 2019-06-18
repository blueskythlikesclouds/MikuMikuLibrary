using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DatabaseConverter
{
    class Program
    {
        class DatabaseInfo
        {
            public Type Type;
            public string ClassicFileName;
            public string ModernFileExtension;

            public DatabaseInfo( Type type, string classicFileName, string modernFileExtension )
            {
                Type = type;
                ClassicFileName = classicFileName;
                ModernFileExtension = modernFileExtension;
            }
        }

        private static readonly DatabaseInfo[] sDatabaseInfos = new DatabaseInfo[]
        {
            new DatabaseInfo( typeof( AetDatabase ), "aetdb", "aei" ),
            new DatabaseInfo( typeof( BoneDatabase ), "bonedata", "bon" ),
            new DatabaseInfo( typeof( MotionDatabase ), "motdb", null ),
            new DatabaseInfo( typeof( ObjectDatabase ), "objdb", "osi" ),
            new DatabaseInfo( typeof( SpriteDatabase ), "sprdb", "spi" ),
            new DatabaseInfo( typeof( StageDatabase ), "stagedata", null ),
            new DatabaseInfo( typeof( StringArray ), "strarray", "str" ),
            new DatabaseInfo( typeof( StringArray ), "stringarray", "str" )
        };

        static DatabaseInfo GetDatabaseInfo( string fileName )
        {
            if ( fileName.EndsWith( ".xml" ) )
                fileName = Path.ChangeExtension( fileName, null );

            string extension = Path.GetExtension( fileName ).Trim( '.' );

            fileName = Path.GetFileNameWithoutExtension( fileName ).Replace( "_", "" );
            if ( fileName.StartsWith( "mdata", StringComparison.OrdinalIgnoreCase ) )
                fileName = fileName.Substring( 6 );

            foreach ( var databaseInfo in sDatabaseInfos )
                if ( ( !string.IsNullOrEmpty( databaseInfo.ModernFileExtension ) &&
                       databaseInfo.ModernFileExtension.Equals( extension, StringComparison.OrdinalIgnoreCase ) ) ||
                     databaseInfo.ClassicFileName.Equals( fileName, StringComparison.OrdinalIgnoreCase ) )
                    return databaseInfo;

            return null;
        }

        static void Main( string[] args )
        {
            if ( args.Length < 1 )
            {
                Console.WriteLine( Properties.Resources.HelpText );
                Console.ReadLine();
                return;
            }

            string sourceFileName = null;
            string destinationFileName = null;

            foreach ( var arg in args )
            {
                if ( sourceFileName == null )
                    sourceFileName = arg;

                else if ( destinationFileName == null )
                    destinationFileName = arg;
            }

            if ( destinationFileName == null )
                destinationFileName = sourceFileName;

            var databaseInfo = GetDatabaseInfo( sourceFileName );
            if ( databaseInfo != null )
            {
                if ( sourceFileName.EndsWith( "xml", StringComparison.OrdinalIgnoreCase ) )
                {
                    var serializer = new XmlSerializer( databaseInfo.Type );

                    IBinaryFile database;
                    using ( var source = File.OpenText( sourceFileName ) )
                        database = ( IBinaryFile ) serializer.Deserialize( source );

                    if ( BinaryFormatUtilities.IsModern( database.Format ) &&
                         !string.IsNullOrEmpty( databaseInfo.ModernFileExtension ) )
                        destinationFileName =
                            Path.ChangeExtension( destinationFileName, null );
                    else
                        destinationFileName = Path.ChangeExtension( destinationFileName, "bin" );

                    database.Save( destinationFileName );
                }
                else
                {
                    var database = ( IBinaryFile ) Activator.CreateInstance( databaseInfo.Type );
                    database.Load( sourceFileName );

                    if ( BinaryFormatUtilities.IsModern( database.Format ) )
                        destinationFileName =
                            Path.ChangeExtension( destinationFileName, databaseInfo.ModernFileExtension ) + ".xml";
                    else
                        destinationFileName = Path.ChangeExtension( destinationFileName, "xml" );

                    var serializer = new XmlSerializer( databaseInfo.Type );

                    using ( var destination = File.CreateText( destinationFileName ) )
                        serializer.Serialize( destination, database );
                }
            }
            else
                throw new InvalidDataException( "Database type could not be detected" );
        }
    }
}
