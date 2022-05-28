using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Archives.CriMw
{
    public class CpkDirectoryNode : Node<List<INode>>
    {
        public CpkDirectoryNode( string name, List<INode> data ) : base( name, data )
        {
        }

        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Folder.png" );
        public override NodeFlags Flags => NodeFlags.Add;
        
        protected override void Initialize()
        {
            AddCustomHandler( "Export All", () =>
            {
                using ( var folderBrowseDialog = new VistaFolderBrowserDialog() )
                {
                    folderBrowseDialog.Description = $"Select a folder to export files to. ({Name})";
                    folderBrowseDialog.UseDescriptionForTitle = true;

                    if ( folderBrowseDialog.ShowDialog() != DialogResult.OK )
                        return;

                    string dirPath = Name;
                    var parent = Parent;

                    while ( parent != null && !( parent is CpkArchiveNode ) )
                    {
                        dirPath = parent.Name + "/" + dirPath;
                        parent = parent.Parent;
                    }

                    if ( !( parent is CpkArchiveNode cpkNode ) )
                        throw new Exception( "Invalid behavior, root parent is not CpkArchiveNode" );

                    cpkNode.Data.Extract( folderBrowseDialog.SelectedPath, dirPath );
                }
            } );
        }

        protected override void PopulateCore()
        {
            foreach ( var node in Data )
                Nodes.Add( node );
        }

        protected override void SynchronizeCore()
        {
        }
    }
}