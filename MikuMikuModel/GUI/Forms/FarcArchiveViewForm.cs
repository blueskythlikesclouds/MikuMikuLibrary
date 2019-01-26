using System;
using System.IO;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Archives.Farc;
using MikuMikuLibrary.IO;
using MikuMikuModel.Resources;

namespace MikuMikuModel.GUI.Forms
{
    public partial class FarcArchiveViewForm : Form
    {
        private readonly FarcArchive mFarcArchive;

        public string SelectedEntryName => mTreeView.SelectedNode?.Name;

        public Stream OpenSelectedEntry( EntryStreamMode mode ) =>
            SelectedEntryName != null ? mFarcArchive.Open( SelectedEntryName, mode ) : null;

        private void OnLoad( object sender, EventArgs e )
        {
            var fileIcon = ResourceStore.LoadBitmap( "Icons/File.png" );
            mTreeView.ImageList.Images.Add( fileIcon );

            foreach ( string entryName in mFarcArchive )
                mTreeView.Nodes.Add( entryName );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                mComponents?.Dispose();
                mFarcArchive?.Dispose();
            }

            base.Dispose( disposing );
        }

        public FarcArchiveViewForm( FarcArchive farcArchive )
        {
            InitializeComponent();
            mFarcArchive = farcArchive;
        }

        public FarcArchiveViewForm( string filePath )
        {
            InitializeComponent();
            mFarcArchive = BinaryFile.Load<FarcArchive>( filePath );
        }
    }
}
