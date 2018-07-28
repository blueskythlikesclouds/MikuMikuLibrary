using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using MikuMikuLibrary.Processing.Textures;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Ai = Assimp;

namespace MikuMikuLibrary.Processing.Models
{
    public static class AssimpModelExporter
    {
        private const string TexturesDirectoryName = "Textures";

        public static void ConvertAiSceneFromModel( Model model, string outputFileName, TextureDatabase textureDatabase = null, bool appendTags = false )
        {
            AssimpSceneUtilities.Export( ConvertAiSceneFromModel( model, textureDatabase, appendTags ), outputFileName );

            var texturesOutputDirectory = Path.Combine( Path.GetDirectoryName( outputFileName ), TexturesDirectoryName );
            TextureUtilities.SaveTextures( model.TextureSet, texturesOutputDirectory );
        }

        public static Ai.Scene ConvertAiSceneFromModel( Model model, TextureDatabase textureDatabase = null, bool appendTags = false )
        {
            var aiScene = new Ai.Scene();
            aiScene.RootNode = new Ai.Node( "RootNode" );

            TextureUtilities.RenameTextures( model.TextureSet, textureDatabase );

            foreach ( var mesh in model.Meshes )
                ConvertAiMaterialsFromMaterials( aiScene, mesh.Materials, model.TextureSet );

            var bones = new List<Bone>();
            var boneParentMap = new Dictionary<string, string>();
            foreach ( var mesh in model.Meshes )
            {
                foreach ( var bone in mesh.Bones )
                {
                    if ( !boneParentMap.ContainsKey( bone.Name ) )
                    {
                        var parentBone =
                            mesh.Bones.FirstOrDefault( x => x.ID == bone.ParentID );

                        bones.Add( bone );
                        boneParentMap.Add( bone.Name, parentBone?.Name );
                    }
                }
            }
            ConvertAiNodesFromBones( aiScene, bones, boneParentMap, appendTags );
            ConvertAiNodesFromMeshes( aiScene, model.Meshes, appendTags );

            return aiScene;
        }

        public static void ConvertAiSceneFromModels( List<Model> models, string outputFileName, TextureDatabase textureDatabase = null )
        {
            AssimpSceneUtilities.Export( ConvertAiSceneFromModels( models, textureDatabase ), outputFileName );

            var texturesOutputDirectory = Path.Combine( Path.GetDirectoryName( outputFileName ), TexturesDirectoryName );
            foreach ( var model in models )
                TextureUtilities.SaveTextures( model.TextureSet, texturesOutputDirectory );
        }

        public static Ai.Scene ConvertAiSceneFromModels( List<Model> models, TextureDatabase textureDatabase = null )
        {
            var aiScene = new Ai.Scene();
            aiScene.RootNode = new Ai.Node( "RootNode" );

            foreach ( var model in models )
                TextureUtilities.RenameTextures( model.TextureSet, textureDatabase );

            foreach ( var model in models )
            {
                foreach ( var mesh in model.Meshes )
                    ConvertAiMaterialsFromMaterials( aiScene, mesh.Materials, model.TextureSet );
            }

            var bones = new List<Bone>();
            var boneParentMap = new Dictionary<string, string>();
            foreach ( var model in models )
            {
                foreach ( var mesh in model.Meshes )
                {
                    foreach ( var bone in mesh.Bones )
                    {
                        if ( !boneParentMap.ContainsKey( bone.Name ) )
                        {
                            var parentBone =
                                mesh.Bones.FirstOrDefault( x => x.ID == bone.ParentID );

                            bones.Add( bone );
                            boneParentMap.Add( bone.Name, parentBone?.Name );
                        }
                    }
                }
            }

            ConvertAiNodesFromBones( aiScene, bones, boneParentMap );

            foreach ( var model in models )
                ConvertAiNodesFromMeshes( aiScene, model.Meshes );

            return aiScene;
        }

        private static void SetBoneIDsFromSkeletonEntry( List<Bone> bones, SkeletonEntry skeletonEntry )
        {
            foreach ( var bone in bones )
            {
                var boneEntry = skeletonEntry.Bones.FirstOrDefault( x => x.Name == bone.Name );
                if ( boneEntry != null )
                {
                    if ( boneEntry.HasParent )
                    {
                        var parentName = skeletonEntry.BoneNames1[ boneEntry.ParentNameIndex ];
                        var parentBone = bones.FirstOrDefault( x => x.Name == parentName );
                        if ( parentBone != null )
                            bone.ParentID = parentBone.ID;
                    }
                }
            }
        }

        private static Ai.Node ConvertAiNodeFromBone( Ai.Node parent, Matrix4x4 inverseParentTransform, Bone bone, bool appendTags = false )
        {
            Matrix4x4.Invert( bone.Matrix, out Matrix4x4 inverse );
            var transform = Matrix4x4.Multiply( inverseParentTransform, inverse );

            var aiNode = new Ai.Node( bone.Name, parent );
            aiNode.Transform = new Ai.Matrix4x4(
                transform.M11, transform.M12, transform.M13, transform.M14,
                transform.M21, transform.M22, transform.M23, transform.M24,
                transform.M31, transform.M32, transform.M33, transform.M34,
                transform.M41, transform.M42, transform.M43, transform.M44 );

            if ( appendTags )
                aiNode.Name = $"{aiNode.Name}{Tag.Create( "ID", bone.ID )}";

            parent.Children.Add( aiNode );
            return aiNode;
        }

        private static void ConvertAiNodesFromBonesRecursively( Ai.Node parent, Matrix4x4 inverseParentTransform, string parentName, List<Bone> bones, Dictionary<string, string> boneParentMap, bool appendTags = false )
        {
            foreach ( var bone in bones )
            {
                if ( boneParentMap[ bone.Name ] == parentName )
                {
                    var boneNode = ConvertAiNodeFromBone( parent, inverseParentTransform, bone, appendTags );
                    ConvertAiNodesFromBonesRecursively( boneNode, bone.Matrix, bone.Name, bones, boneParentMap, appendTags );
                }
            }
        }

        private static void ConvertAiNodesFromBones( Ai.Scene aiScene, List<Bone> bones, Dictionary<string, string> boneParentMap, bool appendTags = false )
        {
            ConvertAiNodesFromBonesRecursively( aiScene.RootNode, Matrix4x4.Identity, null, bones, boneParentMap, appendTags );
        }

        private static Ai.Node ConvertAiNodeFromMesh( Mesh mesh, Ai.Scene aiScene, bool appendTags = false )
        {
            var aiNode = new Ai.Node( mesh.Name );

            foreach ( var subMesh in mesh.SubMeshes )
            {
                foreach ( var indexTable in subMesh.IndexTables )
                {
                    var aiSubMeshNode = new Ai.Node( subMesh.Name );
                    var aiMesh = new Ai.Mesh( subMesh.Name, Ai.PrimitiveType.Triangle );

                    var material = mesh.Materials[ indexTable.MaterialIndex ];
                    aiMesh.MaterialIndex = aiScene.Materials.FindIndex( x => x.Name.Equals( material.Name ) );

                    if ( subMesh.Vertices != null )
                        aiMesh.Vertices.AddRange( subMesh.Vertices.Select(
                            x => new Ai.Vector3D( x.X, x.Y, x.Z ) ) );

                    if ( subMesh.Normals != null )
                        aiMesh.Normals.AddRange( subMesh.Normals.Select(
                            x => new Ai.Vector3D( x.X, 1.0f - x.Y, x.Z ) ) );

                    if ( subMesh.UVChannel1 != null )
                        aiMesh.TextureCoordinateChannels[ 0 ].AddRange( subMesh.UVChannel1.Select(
                            x => new Ai.Vector3D( x.X, 1.0f - x.Y, 0.0f ) ) );

                    if ( subMesh.UVChannel2 != null )
                        aiMesh.TextureCoordinateChannels[ 1 ].AddRange( subMesh.UVChannel2.Select(
                            x => new Ai.Vector3D( x.X, x.Y, 0.0f ) ) );

                    // Why does 3DS Max go retarded when colors are exported?
                    //if ( submesh.Colors != null )
                    //    aiMesh.VertexColorChannels[ 0 ].AddRange( submesh.Colors.Select(
                    //        x => new Ai.Color4D( x.R, x.G, x.B, x.A ) ) );

                    foreach ( var triangle in indexTable.GetTriangles() )
                    {
                        var aiFace = new Ai.Face();
                        aiFace.Indices.Add( triangle.A );
                        aiFace.Indices.Add( triangle.B );
                        aiFace.Indices.Add( triangle.C );
                        aiMesh.Faces.Add( aiFace );
                    }

                    if ( indexTable.BoneIndices != null )
                    {
                        for ( int i = 0; i < indexTable.BoneIndices.Length; i++ )
                        {
                            int boneIndex = indexTable.BoneIndices[ i ];
                            var bone = mesh.Bones[ boneIndex ];
                            var aiBone = new Ai.Bone();
                            aiBone.Name = bone.Name;
                            aiBone.OffsetMatrix = new Ai.Matrix4x4(
                                bone.Matrix.M11, bone.Matrix.M12, bone.Matrix.M13, bone.Matrix.M14,
                                bone.Matrix.M21, bone.Matrix.M22, bone.Matrix.M23, bone.Matrix.M24,
                                bone.Matrix.M31, bone.Matrix.M32, bone.Matrix.M33, bone.Matrix.M34,
                                bone.Matrix.M41, bone.Matrix.M42, bone.Matrix.M43, bone.Matrix.M44 );

                            if ( appendTags )
                                aiBone.Name = $"{aiBone.Name}{Tag.Create( "ID", bone.ID )}";

                            for ( int j = 0; j < subMesh.BoneWeights.Length; j++ )
                            {
                                var weight = subMesh.BoneWeights[ j ];
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
                aiNode.Name = $"{aiNode.Name}{Tag.Create( "ID", mesh.ID )}";

            return aiNode;
        }

        private static void ConvertAiNodesFromMeshes( Ai.Scene aiScene, List<Mesh> meshes, bool appendTags = false )
        {
            foreach ( var mesh in meshes )
                aiScene.RootNode.Children.Add( ConvertAiNodeFromMesh( mesh, aiScene, appendTags ) );
        }

        private static Ai.TextureSlot ConvertTextureSlotFromTextureID( int textureID, Ai.TextureType type, TextureSet textureList )
        {
            if ( textureID < 0 )
                return default( Ai.TextureSlot );

            var texture = textureList.Textures.FirstOrDefault( x => x.ID == textureID );
            if ( texture != null )
            {
                return new Ai.TextureSlot(
                    Path.Combine( TexturesDirectoryName, texture.Name + ".dds" ), type, 0, Ai.TextureMapping.FromUV, 0, 0, Ai.TextureOperation.Add, Ai.TextureWrapMode.Wrap, Ai.TextureWrapMode.Wrap, 0 );
            }

            return default( Ai.TextureSlot );
        }

        private static Ai.Material ConvertAiMaterialFromMaterial( Material material, TextureSet textures )
        {
            var aiMaterial = new Ai.Material();
            aiMaterial.Name = material.Name;

            var diffuse = ConvertTextureSlotFromTextureID( material.Diffuse.TextureID, Ai.TextureType.Diffuse, textures );
            var ambient = ConvertTextureSlotFromTextureID( material.Ambient.TextureID, Ai.TextureType.Ambient, textures );
            var normal = ConvertTextureSlotFromTextureID( material.Normal.TextureID, Ai.TextureType.Normals, textures );
            var specular = ConvertTextureSlotFromTextureID( material.Specular.TextureID, Ai.TextureType.Specular, textures );
            var reflection = ConvertTextureSlotFromTextureID( material.Reflection.TextureID, Ai.TextureType.Reflection, textures );
            var specularPower = ConvertTextureSlotFromTextureID( material.SpecularPower.TextureID, Ai.TextureType.Shininess, textures );

            aiMaterial.AddMaterialTexture( ref diffuse );
            aiMaterial.AddMaterialTexture( ref ambient );
            aiMaterial.AddMaterialTexture( ref normal );
            aiMaterial.AddMaterialTexture( ref specular );
            aiMaterial.AddMaterialTexture( ref reflection );
            aiMaterial.AddMaterialTexture( ref specularPower );

            return aiMaterial;
        }

        private static void ConvertAiMaterialsFromMaterials( Ai.Scene aiScene, List<Material> materials, TextureSet textures )
        {
            foreach ( var material in materials )
            {
                if ( !aiScene.Materials.Any( x => x.Name.Equals( material.Name ) ) )
                    aiScene.Materials.Add( ConvertAiMaterialFromMaterial( material, textures ) );
            }
        }
    }
}
