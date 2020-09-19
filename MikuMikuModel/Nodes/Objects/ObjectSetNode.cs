using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Assimp;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Objects.Extra;
using MikuMikuLibrary.Objects.Extra.Blocks;
using MikuMikuLibrary.Objects.Processing;
using MikuMikuLibrary.Objects.Processing.Assimp;
using MikuMikuLibrary.Objects.Processing.Fbx;
using MikuMikuLibrary.Textures;
using MikuMikuModel.Configurations;
using MikuMikuModel.GUI.Controls;
using MikuMikuModel.GUI.Forms;
using MikuMikuModel.Mementos;
using MikuMikuModel.Nodes.Archives;
using MikuMikuModel.Nodes.Collections;
using MikuMikuModel.Nodes.IO;
using MikuMikuModel.Nodes.Textures;
using MikuMikuModel.Resources;
using Ookii.Dialogs.WinForms;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Object = MikuMikuLibrary.Objects.Object;
using Quaternion = System.Numerics.Quaternion;
using Material = MikuMikuLibrary.Materials.Material;
using PrimitiveType = MikuMikuLibrary.Objects.PrimitiveType;

namespace MikuMikuModel.Nodes.Objects
{
    public class ObjectSetNode : BinaryFileNode<ObjectSet>
    {
        private static readonly XmlSerializer sObjectSetInfoSerializer = new XmlSerializer( typeof( ObjectSetInfo ) );

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

        [Category( "General" )]
        [DisplayName( "Texture ids" )] 
        public List<uint> TextureIds => GetProperty<List<uint>>();

        protected override void Initialize()
        {
            AddExportHandler<ObjectSet>( filePath =>
            {
                if ( filePath.EndsWith( ".fbx", StringComparison.OrdinalIgnoreCase ) )
                {
                    Data.TryFixParentBoneInfos( SourceConfiguration?.BoneDatabase );
                    FbxExporter.ExportToFile( Data, filePath );
                }

                else
                {
                    var configuration = ConfigurationList.Instance.CurrentConfiguration;

                    var objectDatabase = configuration?.ObjectDatabase;
                    var textureDatabase = configuration?.TextureDatabase;
                    var boneDatabase = configuration?.BoneDatabase;

                    Data.Save( filePath, objectDatabase, textureDatabase, boneDatabase );
                }
            } );
            AddExportHandler<Scene>( filePath =>
            {
                Data.TryFixParentBoneInfos( SourceConfiguration?.BoneDatabase );
                AssimpExporter.ExportToFile( Data, filePath );
            } );
            AddReplaceHandler<ObjectSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;

                var objectDatabase = configuration?.ObjectDatabase;
                var textureDatabase = configuration?.TextureDatabase;

                var objectSet = new ObjectSet();
                objectSet.Load( filePath, objectDatabase, textureDatabase );
                return objectSet;
            } );
            AddReplaceHandler<Scene>( filePath =>
            {
                if ( Data.Objects.Count > 1 )
                    return AssimpImporter.ImportFromFile( filePath );

                return AssimpImporter.ImportFromFileWithSingleObject( filePath );
            } );

            AddCustomHandler( "Copy object set info to clipboard", () =>
            {
                uint objectSetId = 39;
                uint objectId = 0xFFFFFFFF;

                var objectDatabase = ConfigurationList.Instance.CurrentConfiguration?.ObjectDatabase;

                if ( objectDatabase != null && objectDatabase.ObjectSets.Count > 0 )
                {
                    objectSetId = objectDatabase.ObjectSets.Max( x => x.Id ) + 1;
                    objectId = objectDatabase.ObjectSets.SelectMany( x => x.Objects ).Max( x => x.Id ) + 1;
                }

                else
                {
                    using ( var inputDialog = new InputDialog
                    {
                        WindowTitle = "Enter base id for objects",
                        Input = Math.Max( 0, Data.Objects.Max( x => x.Id ) + 1 ).ToString()
                    } )
                    {
                        while ( inputDialog.ShowDialog() == DialogResult.OK )
                        {
                            bool result = uint.TryParse( inputDialog.Input, out objectId );

                            if ( !result || objectId == 0xFFFFFFFF )
                                MessageBox.Show( "Please enter a correct id number.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error );

                            else
                                break;
                        }
                    }
                }

                if ( objectId == 0xFFFFFFFF )
                    return;

                string baseName = Path.ChangeExtension( Name, null );
                if ( Data.Format.IsClassic() && baseName.EndsWith( "_obj", StringComparison.OrdinalIgnoreCase ) )
                    baseName = baseName.Substring( 0, baseName.Length - 4 );

                var objectSetInfo = new ObjectSetInfo
                {
                    Name = baseName.ToUpperInvariant(),
                    Id = objectSetId,
                    FileName = Name,
                    TextureFileName = baseName + ( Data.Format.IsClassic() ? "_tex.bin" : ".txd" ),
                    ArchiveFileName = Parent is FarcArchiveNode ? Parent.Name : baseName + ".farc"
                };

                foreach ( var obj in Data.Objects )
                {
                    objectSetInfo.Objects.Add( new ObjectInfo
                    {
                        Id = objectId++,
                        Name = obj.Name.ToUpperInvariant()
                    } );
                }

                using ( var stringWriter = new StringWriter() )
                using ( var xmlWriter = XmlWriter.Create( stringWriter,
                    new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true } ) )
                {
                    sObjectSetInfoSerializer.Serialize( xmlWriter, objectSetInfo,
                        new XmlSerializerNamespaces( new[] { XmlQualifiedName.Empty } ) );

                    Clipboard.SetText( stringWriter.ToString() );
                }
            } );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Combine all objects into one", () =>
            {
                if ( Data.Objects.Count <= 1 )
                    return false;

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
                                int boneIndex = combinedObject.Skin.Bones.FindIndex(
                                    x => x.Name.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) );

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
                                        subMesh.BoneIndices[ i ] = ( ushort ) indexMap[ subMesh.BoneIndices[ i ] ];
                                }
                            }
                        }

                        combinedObject.Skin.Blocks.AddRange( obj.Skin.Blocks );
                    }

                    foreach ( var subMesh in obj.Meshes.SelectMany( x => x.SubMeshes ) )
                        subMesh.MaterialIndex += ( uint ) combinedObject.Materials.Count;

                    combinedObject.Meshes.AddRange( obj.Meshes );
                    combinedObject.Materials.AddRange( obj.Materials );
                }

                Data.Objects.Clear();
                Data.Objects.Add( combinedObject );

                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Convert all bones to osage", () =>
            {
                foreach ( var obj in Data.Objects )
                {
                    var movingBone = obj.Skin?.Bones.FirstOrDefault( x => x.Name == "kl_mune_b_wj" );
                    if ( movingBone == null )
                        continue;

                    var nameMap = new Dictionary<string, string>();
                    var stringBuilder = new StringBuilder();

                    foreach ( var bone in obj.Skin.Bones )
                    {
                        // Ignore bones if they are already OSG or EXP.
                        // Also ignore the moving bone and its parents.

                        // Ignore j_kao_wj for now because it fucks miku's headphones up
                        if ( bone.Name == "j_kao_wj" )
                            continue;

                        var boneToCompare = movingBone;

                        do
                        {
                            if ( boneToCompare == bone )
                                break;

                            boneToCompare = boneToCompare.Parent;
                        } while ( boneToCompare != null );

                        if ( boneToCompare == bone )
                            continue;

                        if ( obj.Skin.Blocks.Any( x =>
                        {
                            switch ( x )
                            {
                                case OsageBlock osgBlock:
                                    return osgBlock.Bones.Any( y => y.Name == bone.Name );
                                case ExpressionBlock expBlock:
                                    return expBlock.BoneName == bone.Name;
                                default:
                                    return false;
                            }
                        } ) )
                            continue;

                        if ( bone.Parent == null )
                            bone.Parent = movingBone;

                        Matrix4x4.Invert( bone.InverseBindPoseMatrix, out var bindPoseMatrix );
                        var matrix = Matrix4x4.Multiply( bindPoseMatrix, bone.Parent.InverseBindPoseMatrix );

                        Matrix4x4.Decompose( matrix, out var scale, out var rotation, out var translation );

                        rotation = Quaternion.Normalize( rotation );

                        string newName = bone.Name;

                        if ( newName.EndsWith( "_wj", StringComparison.OrdinalIgnoreCase ) )
                            newName = newName.Remove( newName.Length - 3 );

                        newName += "_ragdoll";

                        nameMap.Add( bone.Name, newName );

                        bone.Name = newName;

                        string baseName = newName;

                        var osageBlock = new OsageBlock
                        {
                            ExternalName = $"c_{baseName}_osg",
                            InternalName = $"e_{baseName}",
                            ParentName = bone.Parent.Name,
                            Position = translation,
                            Rotation = rotation.ToEulerAngles(),
                            Scale = scale
                        };

                        osageBlock.Bones.Add( new OsageBone { Name = bone.Name, Stiffness = 0.08f } );
                        obj.Skin.Blocks.Add( osageBlock );

                        stringBuilder.AppendFormat(
                            "{0}.node.0.coli_r=0.030000\r\n" +
                            "{0}.node.0.hinge_ymax=179.000000\r\n" +
                            "{0}.node.0.hinge_ymin=-179.000000\r\n" +
                            "{0}.node.0.hinge_zmax=179.000000\r\n" +
                            "{0}.node.0.hinge_zmin=-179.000000\r\n" +
                            "{0}.node.0.inertial_cancel=1.000000\r\n" +
                            "{0}.node.0.weight=3.000000\r\n" +
                            "{0}.node.length=1\r\n" +
                            "{0}.root.force=0.010000\r\n" +
                            "{0}.root.force_gain=0.300000\r\n" +
                            "{0}.root.friction=1.000000\r\n" +
                            "{0}.root.init_rot_y=0.000000\r\n" +
                            "{0}.root.init_rot_z=0.000000\r\n" +
                            "{0}.root.rot_y=0.000000\r\n" +
                            "{0}.root.rot_z=0.000000\r\n" +
                            "{0}.root.stiffness=0.100000\r\n" +
                            "{0}.root.wind_afc=0.500000\r\n",
                            osageBlock.ExternalName );
                    }

                    Clipboard.SetText( stringBuilder.ToString() );

                    foreach ( var block in obj.Skin.Blocks.OfType<NodeBlock>() )
                    {
                        if ( nameMap.TryGetValue( block.ParentName, out string newName ) )
                            block.ParentName = newName;

                        if ( !( block is ExpressionBlock expBlock ) )
                            continue;

                        // Change the bone names in the expressions if necessary.
                        for ( int j = 0; j < expBlock.Expressions.Count; j++ )
                        {
                            string expression = expBlock.Expressions[ j ];

                            foreach ( var kvp in nameMap )
                                expression = expression.Replace( kvp.Key, kvp.Value );

                            expBlock.Expressions[ j ] = expression;
                        }
                    }
                }

                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos );

            AddDirtyCustomHandler( "Convert motion bones to osage", () =>
            {
                foreach ( var obj in Data.Objects )
                {
                    if ( obj.Skin == null )
                        continue;

                    var stringBuilder = new StringBuilder();

                    foreach ( var block in obj.Skin.Blocks.ToList() )
                    {
                        if ( !( block is MotionBlock motionBlock ) )
                            continue;

                        foreach ( var motionBone in motionBlock.Bones )
                        {
                            var bone = obj.Skin.Bones.First( x => x.Name.Equals( motionBone.Name ) );

                            if ( bone?.Parent == null )
                                continue;

                            var matrix = Matrix4x4.Multiply( motionBone.Transformation,
                                bone.Parent.InverseBindPoseMatrix );

                            Matrix4x4.Decompose( matrix, out var scale, out var rotation, out var translation );

                            rotation = Quaternion.Normalize( rotation );

                            string baseName = bone.Name;

                            if ( baseName.StartsWith( "j_", StringComparison.OrdinalIgnoreCase ) )
                                baseName = baseName.Remove( 0, 2 );

                            if ( baseName.EndsWith( "_wj", StringComparison.OrdinalIgnoreCase ) )
                                baseName = baseName.Remove( baseName.Length - 3 );

                            var osageBlock = new OsageBlock();
                            osageBlock.ParentName = bone.Parent.Name;
                            osageBlock.ExternalName = "c_" + baseName + "_osg";
                            osageBlock.InternalName = "e_" + baseName;
                            osageBlock.Position = matrix.Translation;
                            osageBlock.Rotation = rotation.ToEulerAngles();
                            osageBlock.Scale = scale;
                            osageBlock.Bones.Add( new OsageBone { Name = bone.Name, Stiffness = 0.17f } );
                            obj.Skin.Blocks.Add( osageBlock );

                            stringBuilder.AppendFormat(
                                "{0}.node.0.coli_r=0.030000\r\n" +
                                "{0}.node.0.hinge_ymax=179.000000\r\n" +
                                "{0}.node.0.hinge_ymin=-179.000000\r\n" +
                                "{0}.node.0.hinge_zmax=179.000000\r\n" +
                                "{0}.node.0.hinge_zmin=-179.000000\r\n" +
                                "{0}.node.0.inertial_cancel=1.000000\r\n" +
                                "{0}.node.0.weight=3.000000\r\n" +
                                "{0}.node.length=1\r\n" +
                                "{0}.root.force=0.010000\r\n" +
                                "{0}.root.force_gain=0.300000\r\n" +
                                "{0}.root.friction=1.000000\r\n" +
                                "{0}.root.init_rot_y=0.000000\r\n" +
                                "{0}.root.init_rot_z=0.000000\r\n" +
                                "{0}.root.rot_y=0.000000\r\n" +
                                "{0}.root.rot_z=0.000000\r\n" +
                                "{0}.root.stiffness=0.100000\r\n" +
                                "{0}.root.wind_afc=0.500000\r\n",
                                osageBlock.ExternalName );
                        }

                        obj.Skin.Blocks.Remove( block );
                    }

                    Clipboard.SetText( stringBuilder.ToString() );
                }

                return true;
            }, Keys.None, CustomHandlerFlags.Repopulate | CustomHandlerFlags.ClearMementos );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Convert triangles to triangle strips", () =>
            {
                foreach ( var subMesh in Data.Objects.SelectMany( x => x.Meshes ).SelectMany( x => x.SubMeshes ) )
                {
                    if ( subMesh.PrimitiveType != PrimitiveType.Triangles )
                        continue;

                    var triangleStrip = Stripifier.Stripify( subMesh.Indices );
                    if ( triangleStrip == null )
                        continue;

                    subMesh.PrimitiveType = PrimitiveType.TriangleStrip;
                    subMesh.Indices = triangleStrip;
                }

                return true;
            } );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Enable shadows", () =>
            {
                foreach ( var subMesh in Data.Objects.SelectMany( x => x.Meshes ).SelectMany( x => x.SubMeshes ) )
                    subMesh.Flags |= SubMeshFlags.ReceiveShadow;

                return true;
            }, Keys.None, CustomHandlerFlags.ClearMementos );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Generate tangents", () =>
            {
                foreach ( var mesh in Data.Objects.SelectMany( x => x.Meshes ) )
                    mesh.GenerateTangents();

                return true;
            } );

            AddCustomHandlerSeparator();

            AddDirtyCustomHandler( "Rename all shaders to...", () =>
            {
                using ( var inputDialog = new InputDialog { WindowTitle = "Rename all shaders", Input = "BLINN" } )
                {
                    if ( inputDialog.ShowDialog() != DialogResult.OK )
                        return false;

                    foreach ( var material in Data.Objects.SelectMany( x => x.Materials ) )
                        material.ShaderName = inputDialog.Input;
                }

                return true;
            }, Keys.None, CustomHandlerFlags.ClearMementos );

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

                if ( textureSetNode != null )
                {
                    var textureDatabase = SourceConfiguration?.TextureDatabase;

                    textureSetNode.Populate();

                    for ( int i = 0; i < Data.TextureIds.Count; i++ )
                    {
                        if ( i >= textureSetNode.Data.Textures.Count )
                            break;

                        var texture = textureSetNode.Data.Textures[ i ];
                        var textureInfo = textureDatabase?.GetTextureInfo( Data.TextureIds[ i ] );

                        texture.Id = Data.TextureIds[ i ];
                        texture.Name = textureInfo?.Name ?? texture.Name;
                    }

                    Data.TextureSet = textureSetNode.Data;

                    mTextureSetNode = new ReferenceNode( "Texture Set", textureSetNode );
                }
                else
                {
                    mTextureSetNode = null;
                }
            }

            if ( mTextureSetNode == null && Data.TextureSet != null )
                mTextureSetNode = new TextureSetNode( "Texture Set", Data.TextureSet );

            if ( mTextureSetNode != null )
                Nodes.Add( mTextureSetNode );

            if ( mTextureSetNode is ReferenceNode referenceNode )
                SetSubscription( referenceNode.Node );
        }

        protected override void SynchronizeCore()
        {
            if ( mTextureSetNode == null ) 
                return;

            Data.TextureSet = ( TextureSet ) mTextureSetNode.Data;

            Data.TextureIds.Clear();
            Data.TextureIds.AddRange( Data.TextureSet.Textures.Select( x => x.Id ) );
        }

        protected override void OnReplace( ObjectSet previousData )
        {
            if ( Data.Objects.Count == 1 && previousData.Objects.Count == 1 )
                Data.Objects[ 0 ].Name = previousData.Objects[ 0 ].Name;

            var materialOverrideMap = new Dictionary<Material, (Object SourceObject, Material MatchingMaterial)>();

            foreach ( var newObject in Data.Objects )
            {
                var oldObject = previousData.Objects.FirstOrDefault( x =>
                    x.Name.Equals( newObject.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( oldObject == null )
                    continue;

                if ( newObject.Skin != null && oldObject.Skin != null )
                {
                    foreach ( var newBone in newObject.Skin.Bones )
                    {
                        if ( newBone.Id != 0xFFFFFFFF )
                            continue;

                        var oldBone = oldObject.Skin.Bones.FirstOrDefault( x => 
                            x.Name.Equals( newBone.Name, StringComparison.OrdinalIgnoreCase ) );

                        if ( oldBone != null )
                            newBone.Id = oldBone.Id;
                    }

                    if ( newObject.Skin.Blocks.Count == 0 )
                        newObject.Skin.Blocks.AddRange( oldObject.Skin.Blocks );
                }

                newObject.Name = oldObject.Name;
                newObject.Id = oldObject.Id;

                foreach ( var material in newObject.Materials )
                {
                    var matchingMaterial = oldObject.Materials.FirstOrDefault( x => 
                        x.Name.Equals( material.Name, StringComparison.OrdinalIgnoreCase ) );

                    if ( matchingMaterial == null )
                        continue;

                    materialOverrideMap.Add( material, ( newObject, matchingMaterial ) );
                }
            }

            if ( materialOverrideMap.Count > 0 )
            {
                using ( var itemSelectForm = new ItemSelectForm<Material>( materialOverrideMap.OrderBy( x => x.Key.Name ).Select( x =>
                {
                    string name = x.Key.Name;
                    if ( Data.Objects.Count > 1 )
                        name += " (" + x.Value.SourceObject.Name + ")";

                    return ( x.Key, name );
                } ) )
                {
                    Text = "Please select the materials you want to override.",
                    GroupBoxText = "Materials",
                    CheckBoxText = "Override only textures"
                })
                {
                    if ( itemSelectForm.ShowDialog() == DialogResult.OK )
                    {
                        var selectedMaterials = itemSelectForm.CheckedItems.ToHashSet();

                        var texturesToAdd = new HashSet<uint>();
                        var texturesToRemove = new HashSet<uint>();

                        foreach ( var obj in Data.Objects )
                        {
                            for ( int i = 0; i < obj.Materials.Count; i++ )
                            {
                                var material = obj.Materials[ i ];

                                if ( !materialOverrideMap.TryGetValue( material, out var entry ) )
                                    continue;

                                bool @override = selectedMaterials.Contains( material );

                                // I made all the if statements separate to make everything clear.
                                if ( itemSelectForm.CheckBoxChecked )
                                {
                                    for ( int j = 0; j < 8; j++ )
                                    {
                                        var newMaterialTexture = material.MaterialTextures[ j ];
                                        var oldMaterialTexture = entry.MatchingMaterial.MaterialTextures[ j ];

                                        if ( newMaterialTexture.TextureId == oldMaterialTexture.TextureId )
                                            continue;

                                        if ( @override )
                                        {
                                            if ( oldMaterialTexture.Type != MaterialTextureType.None )
                                            {
                                                if ( newMaterialTexture.Type != MaterialTextureType.None )
                                                    oldMaterialTexture.TextureId = newMaterialTexture.TextureId;

                                                else
                                                    texturesToAdd.Add( oldMaterialTexture.TextureId );
                                            }
                                        }
                                        else
                                        {
                                            if ( newMaterialTexture.Type != MaterialTextureType.None )
                                                texturesToRemove.Add( newMaterialTexture.TextureId );

                                            if ( oldMaterialTexture.Type != MaterialTextureType.None )
                                                texturesToAdd.Add( oldMaterialTexture.TextureId );
                                        }
                                    }
                                }
                                else
                                {
                                    if ( @override )
                                        continue;

                                    for ( int j = 0; j < 8; j++ )
                                    {
                                        var newMaterialTexture = material.MaterialTextures[ j ];
                                        var oldMaterialTexture = entry.MatchingMaterial.MaterialTextures[ j ];

                                        if ( newMaterialTexture.TextureId == oldMaterialTexture.TextureId )
                                            continue;

                                        if ( newMaterialTexture.Type != MaterialTextureType.None )
                                            texturesToRemove.Add( newMaterialTexture.TextureId );

                                        if ( oldMaterialTexture.Type != MaterialTextureType.None )
                                            texturesToAdd.Add( oldMaterialTexture.TextureId );
                                    }
                                }

                                obj.Materials[ i ] = entry.MatchingMaterial;
                            }
                        }

                        var allIds = Data.Objects.SelectMany( x => x.Materials ).SelectMany( x => x.MaterialTextures )
                            .Where( x => x.Type != MaterialTextureType.None ).Select( x => x.TextureId ).ToHashSet();

                        foreach ( uint textureToRemove in texturesToRemove.Except( allIds ) )
                            Data.TextureSet.Textures.RemoveAll( x => x.Id == textureToRemove );

                        foreach ( uint textureToAdd in texturesToAdd )
                        {
                            var oldTexture = previousData.TextureSet.Textures
                                .FirstOrDefault( x => x.Id == textureToAdd );

                            if ( oldTexture == null )
                                continue;

                            var newTexture = Data.TextureSet.Textures
                                .FirstOrDefault( x => x.Id == textureToAdd );

                            if ( newTexture != null )
                                continue;

                            Data.TextureSet.Textures.Add( oldTexture );
                        }

                        Data.TextureSet.Textures.Sort( ( x, y ) => string.Compare( x.Name, y.Name, StringComparison.Ordinal ) );

                        Data.TextureIds.Clear();
                        Data.TextureIds.AddRange( Data.TextureSet.Textures.Select( x => x.Id ) );
                    }
                }
            }

            if ( previousData.Format.IsModern() && Data.TextureSet != null )
            {
                Dictionary<uint, uint> idDictionary = null;

                foreach ( var texture in Data.TextureSet.Textures )
                {
                    if ( string.IsNullOrEmpty( texture.Name ) )
                        texture.Name = Guid.NewGuid().ToString();

                    uint id = MurmurHash.Calculate( texture.Name );

                    if ( id == texture.Id )
                        continue;

                    if ( idDictionary == null )
                        idDictionary = new Dictionary<uint, uint>( Data.TextureSet.Textures.Count );

                    idDictionary[ texture.Id ] = id;
                    texture.Id = id;
                }

                if ( idDictionary != null )
                {
                    foreach ( var materialTexture in Data.Objects.SelectMany( x => x.Materials )
                        .SelectMany( x => x.MaterialTextures ) )
                    {
                        if ( !idDictionary.TryGetValue( materialTexture.TextureId, out uint id ) )
                            continue;

                        materialTexture.TextureId = id;
                    }

                    Data.TextureIds.Clear();
                    Data.TextureIds.AddRange( Data.TextureSet.Textures.Select( x => x.Id ) );
                }
            }

            if ( mTextureSetNode != null && Data.TextureSet != null )
                mTextureSetNode.Replace( Data.TextureSet );

            base.OnReplace( previousData );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && mTextureSetNode is ReferenceNode referenceNode )
                SetSubscription( referenceNode.Node, true );

            base.Dispose( disposing );
        }

        public ObjectSetNode( string name, ObjectSet data ) : base( name, data )
        {
        }

        public ObjectSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}