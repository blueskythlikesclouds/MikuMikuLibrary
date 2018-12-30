using MikuMikuLibrary.Models;
using MikuMikuLibrary.Textures;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using MikuMikuModel.GUI.Forms;

namespace MikuMikuModel.DataNodes
{
    public class ModelNode : BinaryFileNode<Model>
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
                  DataNodeActionFlags.Import | DataNodeActionFlags.Export | DataNodeActionFlags.Move |
                  DataNodeActionFlags.Remove | DataNodeActionFlags.Rename | DataNodeActionFlags.Replace;
            }
        }

        public override Control Control
        {
            get
            {
                ModelViewControl.Instance.SetModel( Data, TextureNode.GlobalTextureSet );
                return ModelViewControl.Instance;
            }
        }

        public override Bitmap Icon
        {
            get { return Properties.Resources.Model; }
        }

        [Browsable( false )]
        public ListNode<Mesh> Meshes { get; set; }

        [Browsable( false )]
        public DataNode Textures { get; set; }

        protected override void InitializeCore()
        {
            RegisterExportHandler<Model>( ( path ) =>
            {
                var configuration = ConfigurationList.Instance.FindConfiguration( path );

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;
                var boneDatabase = configuration?.BoneDatabase;

                Data.Save( path, objectDatabase, textureDatabase, boneDatabase );
            } );
            RegisterExportHandler<Assimp.Scene>( ( path ) => ModelExporter.ConvertAiSceneFromModel( Data, path ) );
            RegisterReplaceHandler<Model>( ( path ) =>
            {
                var configuration = ConfigurationList.Instance.FindConfiguration( path );

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;

                var model = new Model();
                model.Load( path, objectDatabase, textureDatabase );
                return model;
            } );
            RegisterReplaceHandler<Assimp.Scene>( ( path ) =>
            {
                if ( Data.Meshes.Count > 1 )
                    return ModelImporter.ConvertModelFromAiScene( path );

                return ModelImporter.ConvertModelFromAiSceneWithSingleMesh( path );
            } );
            RegisterDataUpdateHandler( () =>
            {
                var data = new Model();
                data.Format = Format;
                data.Endianness = Endianness;
                data.Meshes.AddRange( Meshes.Data );
                data.TextureIDs.AddRange( Data.TextureIDs );
                data.TextureSet = Textures?.Data as TextureSet;
                return data;
            } );
            RegisterCustomHandler( "Rename all shaders to...", () =>
            {
                using ( var renameForm = new RenameForm( "BLINN" ) )
                {
                    if ( renameForm.ShowDialog() == DialogResult.OK )
                    {
                        foreach ( var material in Data.Meshes.SelectMany( x => x.Materials ) )
                            material.Shader = renameForm.TextBoxText;
                    }
                }

                HasPendingChanges = true;
            } );
            RegisterCustomHandler( "Convert triangles to triangle strips", () =>
            {
                foreach ( var indexTable in Data.Meshes.SelectMany( x => x.SubMeshes ).SelectMany( x => x.IndexTables ) )
                {
                    if ( indexTable.PrimitiveType == IndexTablePrimitiveType.Triangles )
                    {
                        ushort[] triangleStrip = TriangleStripUtilities.GenerateStrips( indexTable.Indices );
                        if ( triangleStrip != null )
                        {
                            indexTable.PrimitiveType = IndexTablePrimitiveType.TriangleStrip;
                            indexTable.Indices = triangleStrip;
                        }
                    }
                }

                HasPendingChanges = true;
            } );
        }

        protected override void InitializeViewCore()
        {
            Add( Meshes = new ListNode<Mesh>( nameof( Data.Meshes ), Data.Meshes ) );

            if ( Data.TextureSet == null && Parent != null )
            {
                bool found = false;

                var objectDatabase = ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase;
                if ( objectDatabase != null )
                {
                    // Try finding an object that uses us, models don't seem to get reused
                    var objectEntry = objectDatabase.GetObjectByFileName( Name );
                    if ( objectEntry != null )
                    {
                        var textureSetNode = Parent.FindNode<TextureSet>( objectEntry.TextureFileName, StringComparison.OrdinalIgnoreCase );
                        if ( textureSetNode != null )
                        {
                            // If the texture count matches, we most likely own this texture
                            if ( textureSetNode.Data.Textures.Count == Data.TextureIDs.Count )
                            {
                                found = true;

                                // Pass the IDs, and rename the textures
                                var textureDatabase = ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase;
                                for ( int i = 0; i < Data.TextureIDs.Count; i++ )
                                {
                                    textureSetNode.Data.Textures[ i ].ID = Data.TextureIDs[ i ];
                                    textureSetNode.Data.Textures[ i ].Name = textureDatabase?.GetTexture( Data.TextureIDs[ i ] )?.Name;
                                }

                                // Attach the texture set
                                Data.TextureSet = textureSetNode.Data;
                            }
                        }

                        Textures = new ReferenceNode( "Texture Set", textureSetNode );
                    }
                }

                // If we couldn't find the textures, try finding the texture by default
                if ( !found && Name.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
                {
                    var textureName = Name.Substring( 0, Name.Length - 8 ) + "_tex.bin";
                    var textureSetNode = Parent.FindNode<TextureSet>( textureName, StringComparison.OrdinalIgnoreCase );

                    if ( textureSetNode != null )
                    {
                        // If the texture count matches, we probably own this texture
                        if ( textureSetNode.Data.Textures.Count == Data.TextureIDs.Count )
                        {
                            // Pass the IDs and rename them
                            var textureDatabase = ConfigurationList.Instance.CurrentConfiguration?.TextureDatabase;
                            for ( int i = 0; i < Data.TextureIDs.Count; i++ )
                            {
                                textureSetNode.Data.Textures[ i ].ID = Data.TextureIDs[ i ];
                                textureSetNode.Data.Textures[ i ].Name = textureDatabase?.GetTexture( Data.TextureIDs[ i ] )?.Name;
                            }

                            // Attach the texture set
                            Data.TextureSet = textureSetNode.Data;
                        }
                    }

                    Textures = new ReferenceNode( "Texture Set", textureSetNode );
                }

                else if ( !found && Name.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                {
                    var textureName = Path.ChangeExtension( Name, "txd" );
                    var textureSetNode = Parent.FindNode<TextureSet>( textureName, StringComparison.OrdinalIgnoreCase );

                    if ( textureSetNode != null )
                    {
                        textureSetNode.InitializeView();

                        // If the texture count matches, we probably own this texture
                        if ( textureSetNode.Data.Textures.Count == Data.TextureIDs.Count )
                        {
                            // Pass the IDs
                            for ( int i = 0; i < Data.TextureIDs.Count; i++ )
                                textureSetNode.Data.Textures[ i ].ID = Data.TextureIDs[ i ];

                            // Attach the texture set
                            Data.TextureSet = textureSetNode.Data;
                        }
                    }

                    Textures = new ReferenceNode( "Texture Set", textureSetNode );
                }
            }

            if ( Data.TextureSet != null )
            {
                // Pass the textures to the global texture set
                TextureNode.GlobalTextureSet.Textures.AddRange( Data.TextureSet.Textures );

                if ( Textures == null )
                    Textures = new TextureSetNode( "Texture Set", Data.TextureSet );

                Add( Textures );
            }
        }

        protected override void OnReplace( object oldData )
        {
            Model oldDataT = ( Model )oldData;

            // Replace the mesh name with the one we replaced.
            if ( oldDataT.Meshes.Count == Data.Meshes.Count )
            {
                Data.Meshes[ 0 ].Name = oldDataT.Meshes[ 0 ].Name;
                Data.Meshes[ 0 ].ID = oldDataT.Meshes[ 0 ].ID;
            }

            // Pass the ex data to meshes if they don't have them.
            // Game crashes without the ex data if it tries to make use of them.
            // SMH sega, can't even do a null check!!
            foreach ( var mesh in Data.Meshes )
            {
                var oldMesh = oldDataT.Meshes.FirstOrDefault( x => x.Name.Equals( mesh.Name, StringComparison.OrdinalIgnoreCase ) );
                if ( oldMesh != null )
                {
                    if ( mesh.Skin != null && mesh.Skin.ExData == null )
                        mesh.Skin.ExData = oldMesh.Skin?.ExData;

                    mesh.Name = oldMesh.Name;
                    mesh.ID = oldMesh.ID;
                }
            }

            // Replace the texture set
            Textures?.Replace( Data.TextureSet );

            // Pass the format/endianness
            Data.Format = oldDataT.Format;
            Data.Endianness = oldDataT.Endianness;

            base.OnReplace( oldData );
        }

        public ModelNode( string name, Model data ) : base( name, data )
        {
        }
    }
}
