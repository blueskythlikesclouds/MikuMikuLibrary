using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Archives.Farc;
using System;
using System.IO;

namespace FarcPack
{
    class Program
    {
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

            if ( File.GetAttributes( sourceFileName ).HasFlag( FileAttributes.Directory ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, "farc" );

                var farcArchive = new FarcArchive();
                foreach ( var filePath in Directory.EnumerateFiles( sourceFileName ) )
                    farcArchive.Add( Path.GetFileName( filePath ), filePath );

                farcArchive.Save( destinationFileName );
            }

            else if ( args[ 0 ].EndsWith( ".farc", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, null );

                using ( var stream = File.OpenRead( sourceFileName ) )
                {
                    var farcArchive = FarcArchive.Load<FarcArchive>( stream );

                    Directory.CreateDirectory( destinationFileName );
                    foreach ( var fileName in farcArchive )
                    {
                        using ( var destination = File.Create( Path.Combine( destinationFileName, fileName ) ) )
                        using ( var source = farcArchive.Open( fileName, EntryStreamMode.OriginalStream ) )
                            source.CopyTo( destination );
                    }
                }
            }
        }
    }
}
