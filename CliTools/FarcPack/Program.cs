using System;
using System.IO;
using System.Linq;
using FarcPack.Properties;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.IO;

namespace FarcPack
{
    internal class Program
    {
        private static void Main( string[] args )
        {
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

                bool EqualsAny( params string[] strings )
                {
                    return strings.Contains( arg, StringComparer.OrdinalIgnoreCase );
                }
            }

            if ( sourceFileName == null )
            {
                Console.WriteLine( Resources.HelpText );
                Console.ReadLine();
                return;
            }

            if ( destinationFileName == null )
                destinationFileName = sourceFileName;

            if ( sourceFileName.EndsWith( ".farc", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, null );

                using ( var stream = File.OpenRead( sourceFileName ) )
                {
                    var farcArchive = BinaryFile.Load<FarcArchive>( stream );

                    Directory.CreateDirectory( destinationFileName );

                    foreach ( string fileName in farcArchive )
                    {
                        using ( var destination = File.Create( Path.Combine( destinationFileName, fileName ) ) )
                        using ( var source = farcArchive.Open( fileName, EntryStreamMode.OriginalStream ) )
                            source.CopyTo( destination );
                    }
                }
            }
            else
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, "farc" );

                var farcArchive = new FarcArchive { IsCompressed = compress, Alignment = alignment };

                if ( File.GetAttributes( sourceFileName ).HasFlag( FileAttributes.Directory ) )
                {
                    foreach ( string filePath in Directory.EnumerateFiles( sourceFileName ) )
                        farcArchive.Add( Path.GetFileName( filePath ), filePath );
                }

                else
                {
                    farcArchive.Add( Path.GetFileName( sourceFileName ), sourceFileName );
                }

                farcArchive.Save( destinationFileName );
            }
        }
    }
}