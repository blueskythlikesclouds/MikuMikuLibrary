using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MikuMikuLibrary.Archives.Farc;

namespace FarcPack
{
    class Program
    {
        static void WriteLine( string text, ConsoleColor color )
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine( text );
            Console.ForegroundColor = oldColor;
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

            if ( File.GetAttributes( sourceFileName ).HasFlag( FileAttributes.Directory ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, "farc" );

                var farcArchive = new FarcArchive();
                foreach ( var filePath in Directory.EnumerateFiles( sourceFileName ) )
                    farcArchive.Add( Path.GetFileName( filePath ), filePath );

                farcArchive.Save( destinationFileName );
                farcArchive.Dispose();
            }

            else if ( args[ 0 ].EndsWith( ".farc", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, null );

                var farcArchive = FarcArchive.Load<FarcArchive>( sourceFileName );

                Directory.CreateDirectory( destinationFileName );
                foreach ( var fileName in farcArchive )
                {
                    using ( var destination = File.Create( Path.Combine( destinationFileName, fileName ) ) )
                    using ( var source = farcArchive.Open( fileName ) )
                        source.CopyTo( destination );
                }

                farcArchive.Dispose();
            }
        }
    }
}
