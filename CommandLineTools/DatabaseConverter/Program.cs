using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DatabaseConverter
{
    class Program
    {
        static BinaryFile GetDatabaseInstance( string fileName )
        {
            fileName = Path.GetFileNameWithoutExtension( fileName ).ToLowerInvariant().Replace( "_", "" );

            if ( fileName.StartsWith( "mdata" ) )
                fileName = fileName.Substring( 5 );

            switch ( fileName )
            {
                case "aetdb":
                    return new AetDatabase();
                case "bonedata":
                    return new BoneDatabase();
                //case "editdb":
                //    return new EditDatabase()
                case "motdb":
                    return new MotionDatabase();
                case "objdb":
                    return new ObjectDatabase();
                case "sprdb":
                    return new SpriteDatabase();
                case "stagedata":
                    return new StageDatabase();
                case "strarray":
                case "stringarray":
                    return new StringArray();
                case "texdb":
                    return new TextureDatabase();
                default:
                    throw new ArgumentException( "Database type could not be detected", nameof( fileName ) );
            }
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

            if ( sourceFileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, "xml" );

                var database = GetDatabaseInstance( sourceFileName );
                database.Load( sourceFileName );

                var serializer = new XmlSerializer( database.GetType() );
                using ( var destination = File.CreateText( destinationFileName ) )
                    serializer.Serialize( destination, database );

                database.Dispose();
            }
            else if ( sourceFileName.EndsWith( ".xml", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, "bin" );

                var database = GetDatabaseInstance( sourceFileName );

                var serializer = new XmlSerializer( database.GetType() );
                using ( var source = File.OpenText( sourceFileName ) )
                    database = ( BinaryFile )serializer.Deserialize( source );

                database.Save( destinationFileName );
                database.Dispose();
            }
        }
    }
}
