using MikuMikuLibrary.Archives;
using System;
using System.IO;
using System.Linq;

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
            
            bool compress = false;
            int alignment = 16;

            for ( int i = 0; i < args.Length; i++ )
            {
                string arg = args[ i ];
            
                if ( EqualsAny( "-c", "--compress" ) )
                    compress = true;
                    
                else if ( EqualsAny( "-a", "--alignment" ) )
                    alignment = int.Parse( args[ ++i ] );
            
                else if ( sourceFileName == null )
                    sourceFileName = arg;

                else if ( destinationFileName == null )
                    destinationFileName = arg;
                    
                bool EqualsAny( params string[] strings ) =>
                    strings.Contains( arg, StringComparer.OrdinalIgnoreCase );
            }

            if ( destinationFileName == null )
                destinationFileName = sourceFileName;

            if ( File.GetAttributes( sourceFileName ).HasFlag( FileAttributes.Directory ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, "farc" );

                var farcArchive = new FarcArchive();
                farcArchive.IsCompressed = compress;
                farcArchive.Alignment = alignment;
                
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
