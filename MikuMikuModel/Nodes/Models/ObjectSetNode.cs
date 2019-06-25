using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Assimp;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Objects.Processing;
using MikuMikuLibrary.Objects.Processing.Assimp;
using MikuMikuLibrary.Textures;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Misc;
using MikuMikuModel.Nodes.Textures;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;
using Object = MikuMikuLibrary.Objects.Object;
using PrimitiveType = MikuMikuLibrary.Objects.PrimitiveType;

namespace MikuMikuModel.Nodes.Models
{
    public class ObjectSetNode : BinaryFileNode<ObjectSet>
    {
        private INode mTextureSetNode;

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image =>
            ResourceStore.LoadBitmap( "Icons/ObjectSet.png" );

        public override Control Control
        {
            get
            {
                ModelViewControl.Instance.SetModel( Data, Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        [DisplayName( "Texture ids" )] public List<int> TextureIds => GetProperty<List<int>>();

        protected override void Initialize()
        {
            RegisterExportHandler<ObjectSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;
                var boneDatabase = configuration?.BoneDatabase;

                Data.Save( filePath, objectDatabase, textureDatabase, boneDatabase );
            } );
            RegisterExportHandler<Scene>( filePath => Exporter.ConvertAiSceneFromObjectSet( Data, filePath ) );
            RegisterReplaceHandler<ObjectSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;

                var objectSet = new ObjectSet();
                objectSet.Load( filePath, objectDatabase, textureDatabase );
                return objectSet;
            } );
            RegisterReplaceHandler<Scene>( filePath =>
            {
                if ( Data.Objects.Count > 1 )
                    return Importer.ConvertObjectSetFromAiScene( filePath );

                return Importer.ConvertObjectSetFromAiSceneWithSingleObject( filePath );
            } );
            RegisterCustomHandler( "Rename all shaders to...", () =>
            {
                using ( var inputDialog = new InputDialog { WindowTitle = "Rename all shaders", Input = "BLINN" } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        return;

                    foreach ( var material in Data.Objects.SelectMany( x => x.Materials ) )
                        material.Shader = inputDialog.Input;
                }

                IsDirty = true;
            } );
            RegisterCustomHandler( "Convert triangles to triangle strips", () =>
            {
                foreach ( var subMesh in Data.Objects.SelectMany( x => x.Meshes ).SelectMany( x => x.SubMeshes ) )
                {
                    if ( subMesh.PrimitiveType == PrimitiveType.Triangles )
                    {
                        ushort[] triangleStrip = Stripifier.Stripify( subMesh.Indices );
                        if ( triangleStrip != null )
                        {
                            subMesh.PrimitiveType = PrimitiveType.TriangleStrip;
                            subMesh.Indices = triangleStrip;
                        }
                    }
                }

                IsDirty = true;
            } );
            RegisterCustomHandler( "Combine all objects into one", () =>
            {
                if ( Data.Objects.Count <= 1 )
                    return;

                var combinedObject = new Object { Name = "Combined object" };
                var indexMap = new Dictionary<int, int>();

                foreach ( var obj in Data.Objects )
                {
                    if ( obj.Skin != null )
                    {
                        if ( combinedObject.Skin == null )
                        {
                            combinedObject.Skin = new Skin();
                            combinedObject.Skin.Bones.AddRange( obj.Skin.Bones );
                        }
                        else
                        {
                            for ( int i = 0; i < obj.Skin.Bones.Count; i++ )
                            {
                                var bone = obj.Skin.Bones[ i ];
                                var boneIndex = combinedObject.Skin.Bones
                                    .FindIndex( x => x.Name.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) );

                                if ( boneIndex == -1 )
                                {
                                    indexMap[ i ] = combinedObject.Skin.Bones.Count;
                                    combinedObject.Skin.Bones.Add( bone );
                                }
                                else
                                {
                                    indexMap[ i ] = boneIndex;
                                }
                            }

                            foreach ( var subMesh in obj.Meshes.SelectMany( x => x.SubMeshes ) )
                            {
                                if ( subMesh.BoneIndices?.Length >= 1 )
                                {
                                    for ( int i = 0; i < subMesh.BoneIndices.Length; i++ )
                                        subMesh.BoneIndices[ i ] =
                                            ( ushort ) indexMap[ subMesh.BoneIndices[ i ] ];
                                }
                            }
                        }
                    }

                    foreach ( var subMesh in obj.Meshes.SelectMany( x => x.SubMeshes ) )
                        subMesh.MaterialIndex += combinedObject.Materials.Count;

                    combinedObject.Meshes.AddRange( obj.Meshes );
                    combinedObject.Materials.AddRange( obj.Materials );
                }

                Data.Objects.Clear();
                Data.Objects.Add( combinedObject );

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
            Nodes.Add( new ListNode<Object>( "Objects", Data.Objects, x => x.Name ) );

            if ( Parent != null && mTextureSetNode == null )
            {
                string textureSetName = null;

                var objectDatabase = SourceConfiguration?.ObjectDatabase;
                var objectSetInfo = objectDatabase?.GetObjectSetInfoByFileName( Name );
                if ( objectSetInfo != null )
                    textureSetName = objectSetInfo.TextureFileName;

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
                        var textureInfo = textureDatabase?.GetTextureInfo( Data.TextureIds[ i ] );

                        texture.Id = Data.TextureIds[ i ];
                        texture.Name = textureInfo?.Name ?? texture.Name;
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
                Data.TextureSet = ( TextureSet ) mTextureSetNode.Data;
        }

        protected override void OnReplace( ObjectSet previousData )
        {
            if ( Data.Objects.Count == 1 && previousData.Objects.Count == 1 )
                Data.Objects[ 0 ].Name = previousData.Objects[ 0 ].Name;

            foreach ( var newObject in Data.Objects )
            {
                var oldObject = previousData.Objects.FirstOrDefault( x =>
                    x.Name.Equals( newObject.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( oldObject == null )
                    continue;

                if ( newObject.Skin != null && newObject.Skin.ExData == null )
                    newObject.Skin.ExData = oldObject.Skin?.ExData;

                newObject.Name = oldObject.Name;
                newObject.Id = oldObject.Id;
            }

            if ( mTextureSetNode != null && Data.TextureSet != null )
                mTextureSetNode.Replace( Data.TextureSet );

            base.OnReplace( previousData );
        }

        public ObjectSetNode( string name, ObjectSet data ) : base( name, data )
        {
        }

        public ObjectSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}