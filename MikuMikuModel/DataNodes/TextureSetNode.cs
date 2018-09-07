using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MikuMikuModel.DataNodes
{
    [DataNodeSpecialName( "Texture Set" )]
    public class TextureSetNode : BinaryFileNode<TextureSet>
    {
        public override DataNodeFlags Flags
        {
            get { return DataNodeFlags.Branch; }
        }

        public override DataNodeActionFlags ActionFlags
        {
            get
            {
                return
                  DataNodeActionFlags.Import | DataNodeActionFlags.Export |
                  DataNodeActionFlags.Move | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;
            }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.TextureSet; }
        }

        [Browsable( false )]
        public ListNode<Texture> Textures { get; set; }

        protected override void InitializeCore()
        {
            RegisterReplaceHandler<TextureSet>( BinaryFile.Load<TextureSet> );
            RegisterExportHandler<TextureSet>( ( path ) =>
            {
                // Assume it's being exported for F2nd PS3
                if ( BinaryFormatUtilities.IsClassic( Data.Format ) && path.EndsWith( ".txd", StringComparison.OrdinalIgnoreCase ) )
                {
                    Data.Format = BinaryFormat.F2nd;
                    Data.Endianness = Endianness.LittleEndian;
                }

                // Or reverse
                else if ( BinaryFormatUtilities.IsModern( Data.Format ) && path.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
                {
                    Data.Format = BinaryFormat.DT;
                    Data.Endianness = Endianness.LittleEndian;
                }

                // Save a TXI if we are modern
                if ( BinaryFormatUtilities.IsModern( Data.Format ) )
                {
                    var textureDatabase = new TextureDatabase();
                    textureDatabase.Format = Data.Format;
                    textureDatabase.Endianness = Endianness.BigEndian;

                    foreach ( var texture in Data.Textures )
                        textureDatabase.Textures.Add( new TextureEntry
                        {
                            ID = texture.ID,
                            Name = texture.Name,
                        } );

                    textureDatabase.Save( Path.ChangeExtension( path, "txi" ) );
                }

                Data.Save( path );
            } );
            RegisterImportHandler<Texture>( ( path ) =>
            {
                var texture = TextureEncoder.Encode( path );
                var node = DataNodeFactory.Create<Texture>( Path.GetFileNameWithoutExtension( path ), texture );
                Textures.Add( node );
            } );
            RegisterImportHandler<Bitmap>( ( path ) =>
            {
                var texture = TextureEncoder.Encode( path );
                var node = DataNodeFactory.Create<Texture>( Path.GetFileNameWithoutExtension( path ), texture );
                Textures.Add( node );
            } );
            RegisterCustomHandler( "Export All", () =>
            {
                using ( var saveFileDialog = new SaveFileDialog() )
                {
                    saveFileDialog.AutoUpgradeEnabled = true;
                    saveFileDialog.CheckPathExists = true;
                    saveFileDialog.Title = "Select a folder to export textures to.";
                    saveFileDialog.FileName = "Enter into a directory and press Save";

                    if ( saveFileDialog.ShowDialog() == DialogResult.OK )
                    {
                        var outputDirectory = Path.GetDirectoryName( saveFileDialog.FileName );
                        foreach ( var texture in Data.Textures )
                        {
                            if ( !TextureFormatUtilities.IsCompressed( texture.Format ) || texture.IsYCbCr )
                                TextureDecoder.DecodeToPNG( texture, Path.Combine( outputDirectory, texture.Name + ".png" ) );
                            else
                                TextureDecoder.DecodeToDDS( texture, Path.Combine( outputDirectory, texture.Name + ".dds" ) );
                        }
                    }
                }
            }, Keys.Control | Keys.Shift | Keys.E );
            RegisterDataUpdateHandler( () =>
            {
                var data = new TextureSet();
                data.Format = Format;
                data.Endianness = Endianness;
                data.Textures.AddRange( Textures.Data );
                return data;
            } );
        }

        protected override void InitializeViewCore()
        {
            if ( Parent != null && BinaryFormatUtilities.IsModern( Format ) )
            {
                var fileName = Path.ChangeExtension( Name, "txi" );
                var textureDatabaseNode = Parent.FindNode<TextureDatabase>( fileName, StringComparison.OrdinalIgnoreCase );

                if ( textureDatabaseNode != null )
                {
                    var textureDatabase = textureDatabaseNode.Data;

                    // Pass the IDs and the names to the set
                    for ( int i = 0; i < Math.Min( textureDatabase.Textures.Count, Data.Textures.Count ); i++ )
                    {
                        Data.Textures[ i ].ID = textureDatabase.Textures[ i ].ID;
                        Data.Textures[ i ].Name = textureDatabase.Textures[ i ].Name;
                    }
                }
            }

            Add( Textures = new ListNode<Texture>( nameof( Data.Textures ), Data.Textures ) );
        }

        protected override void OnReplace( object oldData )
        {
            TextureSet oldDataT = ( TextureSet )oldData;

            // Pass the format/endianness
            Data.Format = oldDataT.Format;
            Data.Endianness = oldDataT.Endianness;

            base.OnReplace( oldData );
        }

        public TextureSetNode( string name, TextureSet data ) : base( name, data )
        {
        }
    }
}
