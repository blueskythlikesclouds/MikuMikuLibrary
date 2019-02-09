using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Assimp;
using MikuMikuLibrary.Models;
using MikuMikuLibrary.Models.Processing;
using MikuMikuLibrary.Models.Processing.Assimp;
using MikuMikuLibrary.Textures;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Nodes.Textures;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;
using Mesh = MikuMikuLibrary.Models.Mesh;
using PrimitiveType = MikuMikuLibrary.Models.PrimitiveType;

namespace MikuMikuModel.Nodes.Models
{
    public class ModelNode : BinaryFileNode<Model>
    {
        private INode mTextureSetNode;

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/Model.png" );

        public override Control Control
        {
            get
            {
                ModelViewControl.Instance.SetModel( Data, Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        [DisplayName( "Texture ids" )]
        public List<int> TextureIds => GetProperty<List<int>>();

        protected override void Initialize()
        {
            RegisterExportHandler<Model>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;
                var boneDatabase = configuration?.BoneDatabase;

                Data.Save( filePath, objectDatabase, textureDatabase, boneDatabase );
            } );
            RegisterExportHandler<Scene>( filePath => Exporter.ConvertAiSceneFromModel( Data, filePath ) );
            RegisterReplaceHandler<Model>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;

                var model = new Model();
                model.Load( filePath, objectDatabase, textureDatabase );
                return model;
            } );
            RegisterReplaceHandler<Scene>( filePath =>
            {
                if ( Data.Meshes.Count > 1 )
                    return Importer.ConvertModelFromAiScene( filePath );

                return Importer.ConvertModelFromAiSceneWithSingleMesh( filePath );
            } );
            RegisterCustomHandler( "Rename all shaders to...", () =>
            {
                using ( var inputDialog = new InputDialog { WindowTitle = "Rename all shaders", Input = "BLINN" } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( var material in Data.Meshes.SelectMany( x => x.Materials ) )
                        material.Shader = inputDialog.Input;
                }

                IsDirty = true;
            } );
            RegisterCustomHandler( "Convert triangles to triangle strips", () =>
            {
                foreach ( var indexTable in Data.Meshes.SelectMany( x => x.SubMeshes ).SelectMany( x => x.IndexTables ) )
                {
                    if ( indexTable.PrimitiveType == PrimitiveType.Triangles )
                    {
                        ushort[] triangleStrip = Stripifier.Stripify( indexTable.Indices );
                        if ( triangleStrip != null )
                        {
                            indexTable.PrimitiveType = PrimitiveType.TriangleStrip;
                            indexTable.Indices = triangleStrip;
                        }
                    }
                }

                IsDirty = true;
            } );
            RegisterCustomHandler( "Combine all meshes into one", () =>
            {
                if ( Data.Meshes.Count <= 1 )
                    return;
                    
                var combinedMesh = new Mesh { Name = "Combined mesh" };
                var indexMap = new Dictionary<int, int>();
                
                foreach ( var mesh in Data.Meshes )
                {
                    if ( mesh.Skin != null )
                    {
                        if ( combinedMesh.Skin == null )
                        {
                            combinedMesh.Skin = new Skin();
                            combinedMesh.Skin.Bones.AddRange( mesh.Skin.Bones );
                        }
                        else
                        {
                            for ( int i = 0; i < mesh.Skin.Bones.Count; i++ )
                            {
                                var bone = mesh.Skin.Bones[ i ];
                                var boneIndex = combinedMesh.Skin.Bones
                                    .FindIndex( x => x.Name.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) );
                                    
                                if ( boneIndex == -1 )
                                {
                                    indexMap[ i ] = combinedMesh.Skin.Bones.Count;
                                    combinedMesh.Skin.Bones.Add( bone );
                                }
                                else
                                {
                                    indexMap[ i ] = boneIndex;
                                }
                            }
                            
                            foreach ( var indexTable in mesh.SubMeshes.SelectMany( x => x.IndexTables ) )
                            {
                                if ( indexTable.BoneIndices?.Length >= 1 )
                                {
                                    for ( int i = 0; i < indexTable.BoneIndices.Length; i++ )
                                        indexTable.BoneIndices[ i ] = ( ushort ) indexMap[ indexTable.BoneIndices[ i ] ];
                                }
                            }
                        }
                    }
                    
                    foreach ( var indexTable in mesh.SubMeshes.SelectMany( x => x.IndexTables ) )
                        indexTable.MaterialIndex += combinedMesh.Materials.Count;
                    
                    combinedMesh.SubMeshes.AddRange( mesh.SubMeshes );
                    combinedMesh.Materials.AddRange( mesh.Materials );
                }
                
                Data.Meshes.Clear();
                Data.Meshes.Add( combinedMesh );
                
                if ( IsPopulated )
                {
                    IsPopulated = false;
                    Populate();
                }
                
                IsDirty = true;
            } );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<Mesh>( "Meshes", Data.Meshes, x => x.Name ) );

            if ( Parent != null && mTextureSetNode == null )
            {
                string textureSetName = null;

                var objectDatabase = SourceConfiguration?.ObjectDatabase;
                var objectEntry = objectDatabase?.GetObjectByFileName( Name );
                if ( objectEntry != null )
                    textureSetName = objectEntry.TextureFileName;

                var textureSetNode = Parent.FindNode<TextureSetNode>( textureSetName );
                if ( textureSetNode == null )
                {
                    if ( Name.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
                        textureSetName = $"{Name.Substring( 0, Name.Length - 8 )}_tex.bin";
                    else if ( Name.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                        textureSetName = Path.ChangeExtension( Name, "txd" );

                    textureSetNode = Parent.FindNode<TextureSetNode>( textureSetName );
                }

                if ( textureSetNode != null && textureSetNode.Data.Textures.Count == Data.TextureIds.Count )
                {
                    var textureDatabase = SourceConfiguration?.TextureDatabase;

                    textureSetNode.Populate();
                    for ( int i = 0; i < Data.TextureIds.Count; i++ )
                    {
                        var texture = textureSetNode.Data.Textures[ i ];
                        var textureEntry = textureDatabase?.GetTexture( Data.TextureIds[ i ] );

                        texture.Id = Data.TextureIds[ i ];
                        texture.Name = textureEntry?.Name ?? texture.Name;
                    }

                    Data.TextureSet = textureSetNode.Data;

                    mTextureSetNode = new ReferenceNode( "Texture Set", textureSetNode );
                }
                else
                    mTextureSetNode = null;
            }

            if ( mTextureSetNode == null && Data.TextureSet != null )
                mTextureSetNode = new TextureSetNode( "Texture Set", Data.TextureSet );

            if ( mTextureSetNode != null )
                Nodes.Add( mTextureSetNode );
        }

        protected override void SynchronizeCore()
        {
            if ( mTextureSetNode != null )
                Data.TextureSet = ( TextureSet )mTextureSetNode.Data;
        }

        protected override void OnReplace( Model previousData )
        {
            if ( Data.Meshes.Count == 1 && previousData.Meshes.Count == 1 )
                Data.Meshes[ 0 ].Name = previousData.Meshes[ 0 ].Name;

            foreach ( var newMesh in Data.Meshes )
            {
                var oldMesh = previousData.Meshes.FirstOrDefault( x =>
                    x.Name.Equals( newMesh.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( oldMesh == null )
                    continue;

                if ( newMesh.Skin != null && newMesh.Skin.ExData == null )
                    newMesh.Skin.ExData = oldMesh.Skin?.ExData;

                newMesh.Name = oldMesh.Name;
                newMesh.Id = oldMesh.Id;
            }

            if ( mTextureSetNode != null && Data.TextureSet != null )
                mTextureSetNode.Replace( Data.TextureSet );

            base.OnReplace( previousData );
        }

        public ModelNode( string name, Model data ) : base( name, data )
        {
        }

        public ModelNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}