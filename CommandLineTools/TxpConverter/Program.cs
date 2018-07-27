using MikuMikuLibrary.Processing.Textures;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;

namespace TxpConverter
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
                destinationFileName = Path.ChangeExtension( destinationFileName, "bin" );

                var textureSet = new TextureSet();
                var textures = new SortedList<int, Texture>();
                foreach ( var textureFileName in Directory.EnumerateFiles( sourceFileName, "*.dds" ) )
                {
                    var cleanFileName = Path.GetFileNameWithoutExtension( textureFileName );
                    if ( int.TryParse( cleanFileName, out int index ) )
                    {
                        textures.Add( index, TextureEncoder.Encode( textureFileName ) );
                    }

                    else
                        Console.WriteLine( "WARNING: Skipped '{0}' because it didn't match the expected name format", Path.GetFileName( textureFileName ) );
                }

                textureSet.Textures.Capacity = textures.Count;
                foreach ( var texture in textures.Values )
                    textureSet.Textures.Add( texture );

                textureSet.Save( destinationFileName );
                textureSet.Dispose();
            }

            else if ( sourceFileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, null );

                var textureSet = TextureSet.Load<TextureSet>( sourceFileName );

                Directory.CreateDirectory( destinationFileName );
                for ( int i = 0; i < textureSet.Textures.Count; i++ )
                {
                    string outputFileName = Path.Combine( destinationFileName, $"{i}.dds" );
                    TextureDecoder.DecodeToDDS( textureSet.Textures[ i ], outputFileName );
                }

                textureSet.Dispose();
            }
        }
    }
}
