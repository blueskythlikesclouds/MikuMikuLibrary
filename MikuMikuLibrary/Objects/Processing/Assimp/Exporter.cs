using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Ai = Assimp;

namespace MikuMikuLibrary.Objects.Processing.Assimp
{
    public static class Exporter
    {
        public static void ConvertAiSceneFromObjectSet( ObjectSet objectSet, string outputFileName,
            TextureDatabase textureDatabase = null, bool appendTags = false )
        {
            SceneUtilities.Export( ConvertAiSceneFromObjectSet( objectSet, textureDatabase, appendTags ), outputFileName );

            if ( objectSet.TextureSet != null )
            {
                var texturesOutputDirectory = Path.GetDirectoryName( outputFileName );
                TextureUtilities.SaveTextures( objectSet.TextureSet, texturesOutputDirectory );
            }
        }

        public static Ai.Scene ConvertAiSceneFromObjectSet( ObjectSet objectSet, TextureDatabase textureDatabase = null,
            bool appendTags = false )
        {
            var aiScene = new Ai.Scene();
            aiScene.RootNode = new Ai.Node( "RootNode" );

            if ( objectSet.TextureSet != null )
                TextureUtilities.RenameTextures( objectSet.TextureSet, textureDatabase );

            foreach ( var obj in objectSet.Objects )
                ConvertAiMaterialsFromMaterials( aiScene, obj.Materials, objectSet.TextureSet );

            var bones = new List<BoneInfo>();
            var boneParentMap = new Dictionary<string, string>();
            foreach ( var obj in objectSet.Objects.Where( x => x.Skin != null ) )
            {
                foreach ( var boneInfo in obj.Skin.Bones )
                {
                    if ( !boneParentMap.ContainsKey( boneInfo.Name ) )
                    {
                        var parentBone =
                            obj.Skin.Bones.FirstOrDefault( x => x.Id == boneInfo.ParentId );

                        bones.Add( boneInfo );
                        boneParentMap.Add( boneInfo.Name, parentBone?.Name );
                    }
                }
            }

            ConvertAiNodesFromBones( aiScene, bones, boneParentMap, appendTags );
            ConvertAiNodesFromObjects( aiScene, objectSet.Objects, appendTags );

            return aiScene;
        }

        public static void ConvertAiSceneFromObjectSets( List<ObjectSet> objectSets, string outputFileName,
            TextureDatabase textureDatabase = null )
        {
            SceneUtilities.Export( ConvertAiSceneFromObjectSets( objectSets, textureDatabase ), outputFileName );

            var texturesOutputDirectory = Path.GetDirectoryName( outputFileName );
            foreach ( var objectSet in objectSets.Where( x => x.TextureSet != null ) )
                TextureUtilities.SaveTextures( objectSet.TextureSet, texturesOutputDirectory );
        }

        public static Ai.Scene ConvertAiSceneFromObjectSets( List<ObjectSet> objectSets,
            TextureDatabase textureDatabase = null )
        {
            var aiScene = new Ai.Scene();
            aiScene.RootNode = new Ai.Node( "RootNode" );

            foreach ( var objectSet in objectSets.Where( x => x.TextureSet != null ) )
                TextureUtilities.RenameTextures( objectSet.TextureSet, textureDatabase );

            foreach ( var objectSet in objectSets )
            {
                foreach ( var obj in objectSet.Objects )
                    ConvertAiMaterialsFromMaterials( aiScene, obj.Materials, objectSet.TextureSet );
            }

            var bones = new List<BoneInfo>();
            var boneParentMap = new Dictionary<string, string>();
            foreach ( var objectSet in objectSets )
            {
                foreach ( var obj in objectSet.Objects.Where( x => x.Skin != null ) )
                {
                    foreach ( var boneInfo in obj.Skin.Bones )
                    {
                        if ( !boneParentMap.ContainsKey( boneInfo.Name ) )
                        {
                            var parentBone =
                                obj.Skin.Bones.FirstOrDefault( x => x.Id == boneInfo.ParentId );

                            bones.Add( boneInfo );
                            boneParentMap.Add( boneInfo.Name, parentBone?.Name );
                        }
                    }
                }
            }

            ConvertAiNodesFromBones( aiScene, bones, boneParentMap );

            foreach ( var objectSet in objectSets )
                ConvertAiNodesFromObjects( aiScene, objectSet.Objects );

            return aiScene;
        }

        private static Ai.Node ConvertAiNodeFromBone( Ai.Node parent, Matrix4x4 inverseParentTransform,
            BoneInfo boneInfo, bool appendTags = false )
        {
            Matrix4x4.Invert( boneInfo.InverseBindPoseMatrix, out Matrix4x4 inverse );
            var transform = Matrix4x4.Multiply( inverseParentTransform, inverse );

            var aiNode = new Ai.Node( boneInfo.Name, parent );
            aiNode.Transform = transform.ToAssimp();

            if ( appendTags )
                aiNode.Name = $"{aiNode.Name}{Tag.Create( "ID", boneInfo.Id )}";

            parent.Children.Add( aiNode );
            return aiNode;
        }

        private static void ConvertAiNodesFromBonesRecursively( Ai.Node parent, Matrix4x4 inverseParentTransform,
            string parentName, List<BoneInfo> bones, Dictionary<string, string> boneParentMap, bool appendTags = false )
        {
            foreach ( var bone in bones )
            {
                if ( boneParentMap[ bone.Name ] == parentName )
                {
                    var boneNode = ConvertAiNodeFromBone( parent, inverseParentTransform, bone, appendTags );
                    ConvertAiNodesFromBonesRecursively( boneNode, bone.InverseBindPoseMatrix, bone.Name, bones,
                        boneParentMap, appendTags );
                }
            }
        }

        private static void ConvertAiNodesFromBones( Ai.Scene aiScene, List<BoneInfo> bones,
            Dictionary<string, string> boneParentMap, bool appendTags = false )
        {
            ConvertAiNodesFromBonesRecursively( aiScene.RootNode, Matrix4x4.Identity, null, bones, boneParentMap,
                appendTags );
        }

        private static Ai.Node ConvertAiNodeFromObject( Object obj, Ai.Scene aiScene, bool appendTags = false )
        {
            var aiNode = new Ai.Node( obj.Name );

            foreach ( var mesh in obj.Meshes )
            {
                foreach ( var subMesh in mesh.SubMeshes )
                {
                    var aiSubMeshNode = new Ai.Node( mesh.Name );
                    var aiMesh = new Ai.Mesh( mesh.Name, Ai.PrimitiveType.Triangle );

                    var material = obj.Materials[ subMesh.MaterialIndex ];
                    aiMesh.MaterialIndex = aiScene.Materials.FindIndex( x => x.Name.Equals( material.Name ) );

                    if ( mesh.Vertices != null )
                        aiMesh.Vertices.AddRange( mesh.Vertices.Select(
                            x => new Ai.Vector3D( x.X, x.Y, x.Z ) ) );

                    if ( mesh.Normals != null )
                        aiMesh.Normals.AddRange( mesh.Normals.Select(
                            x => new Ai.Vector3D( x.X, x.Y, x.Z ) ) );

                    if ( mesh.UVChannel1 != null )
                        aiMesh.TextureCoordinateChannels[ 0 ].AddRange( mesh.UVChannel1.Select(
                            x => new Ai.Vector3D( x.X, 1.0f - x.Y, 0.0f ) ) );

                    if ( mesh.UVChannel2 != null )
                        aiMesh.TextureCoordinateChannels[ 1 ].AddRange( mesh.UVChannel2.Select(
                            x => new Ai.Vector3D( x.X, 1.0f - x.Y, 0.0f ) ) );

                    if ( mesh.Colors != null )
                        aiMesh.VertexColorChannels[ 0 ].AddRange( mesh.Colors.Select(
                            x => new Ai.Color4D( x.R, x.G, x.B, x.A ) ) );

                    foreach ( var triangle in subMesh.GetTriangles() )
                    {
                        var aiFace = new Ai.Face();
                        aiFace.Indices.Add( triangle.A );
                        aiFace.Indices.Add( triangle.B );
                        aiFace.Indices.Add( triangle.C );
                        aiMesh.Faces.Add( aiFace );
                    }

                    if ( subMesh.BoneIndices != null )
                    {
                        for ( int i = 0; i < subMesh.BoneIndices.Length; i++ )
                        {
                            int boneIndex = subMesh.BoneIndices[ i ];
                            var bone = obj.Skin.Bones[ boneIndex ];
                            var aiBone = new Ai.Bone();
                            aiBone.Name = bone.Name;
                            aiBone.OffsetMatrix = bone.InverseBindPoseMatrix.ToAssimp();

                            if ( appendTags )
                                aiBone.Name = $"{aiBone.Name}{Tag.Create( "ID", bone.Id )}";

                            for ( int j = 0; j < mesh.BoneWeights.Length; j++ )
                            {
                                var weight = mesh.BoneWeights[ j ];
                                if ( weight.Index1 == i )
                                    aiBone.VertexWeights.Add( new Ai.VertexWeight
                                    {
                                        VertexID = j,
                                        Weight = weight.Weight1
                                    } );

                                if ( weight.Index2 == i )
                                    aiBone.VertexWeights.Add( new Ai.VertexWeight
                                    {
                                        VertexID = j,
                                        Weight = weight.Weight2
                                    } );

                                if ( weight.Index3 == i )
                                    aiBone.VertexWeights.Add( new Ai.VertexWeight
                                    {
                                        VertexID = j,
                                        Weight = weight.Weight3
                                    } );

                                if ( weight.Index4 == i )
                                    aiBone.VertexWeights.Add( new Ai.VertexWeight
                                    {
                                        VertexID = j,
                                        Weight = weight.Weight4
                                    } );
                            }

                            aiMesh.Bones.Add( aiBone );
                        }
                    }

                    aiNode.Children.Add( aiSubMeshNode );
                    aiSubMeshNode.MeshIndices.Add( aiScene.MeshCount );
                    aiScene.Meshes.Add( aiMesh );
                }
            }

            if ( appendTags )
                aiNode.Name = $"{aiNode.Name}{Tag.Create( "ID", obj.Id )}";

            return aiNode;
        }

        private static void ConvertAiNodesFromObjects( Ai.Scene aiScene, List<Object> objects, bool appendTags = false )
        {
            foreach ( var obj in objects )
                aiScene.RootNode.Children.Add( ConvertAiNodeFromObject( obj, aiScene, appendTags ) );
        }

        private static Ai.TextureSlot ConvertTextureSlotFromTextureId( int textureId, Ai.TextureType type,
            TextureSet textureList )
        {
            if ( textureId == -1 || textureList == null )
                return default( Ai.TextureSlot );

            var texture = textureList.Textures.FirstOrDefault( x => x.Id == textureId );
            if ( texture != null )
                return new Ai.TextureSlot( TextureUtilities.GetFileName( texture ), type, 0, Ai.TextureMapping.FromUV,
                    0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );

            return default( Ai.TextureSlot );
        }

        private static Ai.Material ConvertAiMaterialFromMaterial( Material material, TextureSet textures )
        {
            var aiMaterial = new Ai.Material
            {
                Name = material.Name,
                ColorDiffuse = material.DiffuseColor.ToAssimp(),
                ColorAmbient = material.AmbientColor.ToAssimp(),
                ColorSpecular = material.SpecularColor.ToAssimp(),
                ColorEmissive = material.EmissionColor.ToAssimp(),
                Shininess = material.Shininess,
                ShadingMode = Ai.ShadingMode.Phong,
            };

            ConvertMaterialTexture( material.Diffuse, Ai.TextureType.Diffuse );
            ConvertMaterialTexture( material.Ambient, Ai.TextureType.Ambient );
            ConvertMaterialTexture( material.Normal, Ai.TextureType.Normals );
            ConvertMaterialTexture( material.Specular, Ai.TextureType.Specular );
            ConvertMaterialTexture( material.Reflection, Ai.TextureType.Reflection );
            ConvertMaterialTexture( material.Tangent, Ai.TextureType.Shininess );

            void ConvertMaterialTexture( MaterialTexture materialTexture, Ai.TextureType textureType )
            {
                if ( materialTexture.IsActive )
                {
                    var texture = ConvertTextureSlotFromTextureId( materialTexture.TextureId, textureType, textures );
                    aiMaterial.AddMaterialTexture( ref texture );
                }
            }

            return aiMaterial;
        }

        private static void ConvertAiMaterialsFromMaterials( Ai.Scene aiScene, List<Material> materials,
            TextureSet textures )
        {
            foreach ( var material in materials )
            {
                if ( !aiScene.Materials.Any( x => x.Name.Equals( material.Name ) ) )
                    aiScene.Materials.Add( ConvertAiMaterialFromMaterial( material, textures ) );
            }
        }
    }
}
