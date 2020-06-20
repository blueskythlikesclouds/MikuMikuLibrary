using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.IO;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Archives
{
    public class FarcArchiveNode : BinaryFileNode<FarcArchive>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Remove | NodeFlags.Move | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Archive.png" );

        [Category( "General" )]
        public int Alignment
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [Category( "General" )]
        [DisplayName( "Enable compression" )]
        public bool IsCompressed
        {
            get => GetProperty<bool>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
            AddImportHandler<Stream>( filePath => Data.Add( Path.GetFileName( filePath ), filePath ) );
            AddExportHandler<FarcArchive>( filePath => Data.Save( filePath ) );
            AddReplaceHandler<FarcArchive>( BinaryFile.Load<FarcArchive> );
            AddCustomHandler( "Export All", () =>
            {
                using ( var folderBrowseDialog = new VistaFolderBrowserDialog() )
                {
                    folderBrowseDialog.Description = "Select a folder to export entries to.";
                    folderBrowseDialog.UseDescriptionForTitle = true;

                    if ( folderBrowseDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( string handle in Data.Entries )
                    {
                        using ( var source = Data.Open( handle, EntryStreamMode.OriginalStream ) )
                        using ( var destination = File.Create( Path.Combine( folderBrowseDialog.SelectedPath, handle ) ) )
                            source.CopyTo( destination );
                    }
                }
            }, Keys.Control | Keys.Shift | Keys.E );
        }

        protected override void PopulateCore()
        {
            foreach ( string handle in Data )
            {
                var module = ModuleImportUtilities.GetModule( handle, () => Data.Open( handle, EntryStreamMode.OriginalStream ) );

                INode node;
                
                if ( module != null && typeof( IBinaryFile ).IsAssignableFrom( module.ModelType ) && NodeFactory.NodeTypes.ContainsKey( module.ModelType ) )
                    node = NodeFactory.Create( module.ModelType, handle, new Func<Stream>( () => Data.Open( handle, EntryStreamMode.MemoryStream ) ) );

                else
                    node = new StreamNode( handle, Data.Open( handle, EntryStreamMode.OriginalStream ) );

                Nodes.Add( node );
            }
        }

        protected override void SynchronizeCore()
        {
            foreach ( var node in Nodes )
            {
                switch ( node )
                {
                    case IDirtyNode dirtyNode when dirtyNode.IsDirty:
                        Data.Add( dirtyNode.Name, dirtyNode.GetStream(), false, ConflictPolicy.Replace );
                        break;
                    case StreamNode streamNode:
                        Data.Add( streamNode.Name, streamNode.Data, true, ConflictPolicy.Replace );
                        break;
                }
            }

            foreach ( string entryName in Data.Entries.Except( Nodes.Select( x => x.Name ), StringComparer.InvariantCultureIgnoreCase ).ToList() )
                Data.Remove( entryName );
        }

        protected override void OnExport( string filePath )
        {
            foreach ( var node in Nodes )
            {
                if ( !( node.Data is Stream stream ) )
                    continue;

                stream.Close();

                node.Replace( Data.Open( node.Name, EntryStreamMode.OriginalStream ) );
            }

            base.OnExport( filePath );
        }

        public FarcArchiveNode( string name, FarcArchive data ) : base( name, data )
        {
        }

        public FarcArchiveNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}