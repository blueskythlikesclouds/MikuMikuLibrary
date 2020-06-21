using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.DDS;
using TxpConverter.Properties;

namespace TxpConverter
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            if ( args.Length < 1 )
            {
                Console.WriteLine( Resources.HelpText );
                Console.ReadLine();
                return;
            }

            string sourceFileName = null;
            string destinationFileName = null;

            foreach ( string arg in args )
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

                foreach ( string textureFileName in Directory.EnumerateFiles( sourceFileName ) )
                {
                    if ( textureFileName.EndsWith( ".dds", StringComparison.OrdinalIgnoreCase ) ||
                         textureFileName.EndsWith( ".png", StringComparison.OrdinalIgnoreCase ) )
                    {
                        string cleanFileName = Path.GetFileNameWithoutExtension( textureFileName );

                        if ( int.TryParse( cleanFileName, out int index ) )
                        {
                            Texture texture;

                            if ( textureFileName.EndsWith( ".png", StringComparison.OrdinalIgnoreCase ) )
                            {
                                var bitmap = new Bitmap( textureFileName );
                                var format = TextureFormat.RGB8;

                                if ( DDSCodec.HasTransparency( bitmap ) )
                                    format = TextureFormat.RGBA8;

                                texture = TextureEncoder.Encode( new Bitmap( textureFileName ), format, false );
                            }

                            else
                            {
                                texture = TextureEncoder.Encode( textureFileName );
                            }

                            textures.Add( index, texture );
                        }

                        else
                        {
                            Console.WriteLine( "WARNING: Skipped '{0}' because it didn't match the expected name format", 
                                Path.GetFileName( textureFileName ) );
                        }
                    }
                }

                textureSet.Textures.Capacity = textures.Count;

                foreach ( var texture in textures.Values )
                    textureSet.Textures.Add( texture );

                textureSet.Save( destinationFileName );
            }

            else if ( sourceFileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) ||
                      sourceFileName.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) )
            {
                destinationFileName = Path.ChangeExtension( destinationFileName, null );

                var textureSet = BinaryFile.Load<TextureSet>( sourceFileName );

                Directory.CreateDirectory( destinationFileName );

                for ( int i = 0; i < textureSet.Textures.Count; i++ )
                {
                    var texture = textureSet.Textures[ i ];
                    string name = string.IsNullOrEmpty( texture.Name ) ? $"{i}" : texture.Name;

                    if ( TextureFormatUtilities.IsCompressed( texture.Format ) )
                        TextureDecoder.DecodeToDDS( texture, Path.Combine( destinationFileName, $"{name}.dds" ) );

                    else
                        TextureDecoder.DecodeToPNG( texture, Path.Combine( destinationFileName, $"{name}.png" ) );
                }
            }
        }
    }
}