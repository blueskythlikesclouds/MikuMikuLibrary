using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MikuMikuLibrary.Archives;
using MikuMikuLibrary.Archives.CriMw;
using MikuMikuLibrary.IO;
using MikuMikuModel.Modules;
using MikuMikuModel.Nodes.IO;
using Ookii.Dialogs.WinForms;

namespace MikuMikuModel.Nodes.Archives.CriMw
{
    public class CpkArchiveNode : BinaryFileNode<CpkArchive>
    {
        private class CpkDirectory
        {
            public readonly string Name;
            public readonly Dictionary<string, CpkDirectory> Directories;
            public readonly List<INode> Nodes;

            public CpkDirectoryNode GetDirectoryNode() =>
                new CpkDirectoryNode( Name,
                    Directories.Values.OrderBy( x => x.Name ).Select( x => x.GetDirectoryNode() )
                        .Concat( Nodes.OrderBy( x => x.Name ) ).ToList() );

            public CpkDirectory( string name )
            {
                Name = name;
                Directories = new Dictionary<string, CpkDirectory>();
                Nodes = new List<INode>();
            }
        }

        public CpkArchiveNode( string name, CpkArchive data ) : base( name, data )
        {
        }

        public CpkArchiveNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }

        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
            AddCustomHandler( "Export All", () =>
            {
                using ( var folderBrowseDialog = new VistaFolderBrowserDialog() )
                {
                    folderBrowseDialog.Description = $"Select a folder to export files to. {Name}";
                    folderBrowseDialog.UseDescriptionForTitle = true;

                    if ( folderBrowseDialog.ShowDialog() != DialogResult.OK )
                        return;

                    Data.Extract( folderBrowseDialog.SelectedPath );
                }
            } );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            var root = new CpkDirectory( string.Empty );

            foreach ( var path in Data )
            {
                var split = path.Split( '\\', '/' );

                var parent = root;

                for ( int i = 0; i < split.Length - 1; i++ )
                {
                    string dirName = split[ i ];

                    if ( !parent.Directories.TryGetValue( dirName, out var child ) )
                        parent.Directories[ dirName ] = child = new CpkDirectory( dirName );

                    parent = child;
                }

                string name = split[ split.Length - 1 ];

                var module = ModuleImportUtilities.GetModule( name,
                    () => Data.Open( path, EntryStreamMode.OriginalStream ) );

                INode node;

                if ( module != null && typeof( IBinaryFile ).IsAssignableFrom( module.ModelType ) && NodeFactory.NodeTypes.ContainsKey( module.ModelType ) )
                    node = NodeFactory.Create( module.ModelType, name, new Func<Stream>( () => Data.Open( path, EntryStreamMode.MemoryStream ) ) );

                else
                    node = new StreamNode( name, Data.Open( path, EntryStreamMode.OriginalStream ) );

                parent.Nodes.Add( node );
            }

            foreach ( var dir in root.Directories.Values.OrderBy( x => x.Name ) )
                Nodes.Add( dir.GetDirectoryNode() );

            foreach ( var node in root.Nodes.OrderBy( x => x.Name ) )
                Nodes.Add( node );
        }

        protected override void SynchronizeCore()
        {
        }
    }
}