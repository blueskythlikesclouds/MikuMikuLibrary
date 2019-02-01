using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Archives.Farc;
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

        public int Alignment
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
            RegisterImportHandler<Stream>( NodeFactory.Create );
            RegisterExportHandler<FarcArchive>( filePath => Data.Save( filePath ) );
            RegisterReplaceHandler<FarcArchive>( BinaryFile.Load<FarcArchive> );
            RegisterCustomHandler( "Export All", () =>
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
                var module = ModuleImportUtilities.GetModule( handle,
                    () => Data.Open( handle, EntryStreamMode.OriginalStream ) );

                Nodes.Add(
                    module != null && typeof( IBinaryFile ).IsAssignableFrom( module.ModelType ) &&
                    NodeFactory.NodeTypes.ContainsKey( module.ModelType )
                        ? NodeFactory.Create( module.ModelType, handle,
                            new Func<Stream>( () => Data.Open( handle, EntryStreamMode.MemoryStream ) ) )
                        : new StreamNode( handle, Data.Open( handle, EntryStreamMode.OriginalStream ) ) );
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

            foreach ( string entryName in Data.Entries.Except( Nodes.Select( x => x.Name ),
                StringComparer.InvariantCultureIgnoreCase ).ToList() )
            {
                Data.Remove( entryName );
            }
        }

        public FarcArchiveNode( string name, FarcArchive data ) : base( name, data )
        {
        }

        public FarcArchiveNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}