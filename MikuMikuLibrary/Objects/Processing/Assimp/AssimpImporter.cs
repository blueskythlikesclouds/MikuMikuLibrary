using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;
using Ai = Assimp;

namespace MikuMikuLibrary.Objects.Processing.Assimp
{
    public static class AssimpImporter
    {
        public static ObjectSet ImportFromFile( string filePath )
        {
            string texturesDirectoryPath = Path.GetDirectoryName( filePath );
            return ImportFromAiScene( AssimpSceneHelper.Import( filePath ), texturesDirectoryPath );
        }

        public static ObjectSet ImportFromAiScene( Ai.Scene aiScene, string texturesDirectoryPath )
        {
            var objectSet = new ObjectSet
            {
                TextureSet = new TextureSet()
            };

            foreach ( var aiNode in aiScene.RootNode.Children )
            {
                var obj = CreateObjectFromAiNode( aiNode, aiScene, objectSet, texturesDirectoryPath );

                if ( obj?.Meshes?.Count > 0 )
                    objectSet.Objects.Add( obj );
            }

            objectSet.TextureIds.AddRange( objectSet.TextureSet.Textures.Select( x => x.Id ) );

            return objectSet;
        }

        public static ObjectSet ImportFromFileWithSingleObject( string filePath )
        {
            string texturesDirectory = Path.GetDirectoryName( filePath );
            return ImportFromAiSceneWithSingleObject( AssimpSceneHelper.Import( filePath ), texturesDirectory );
        }

        public static ObjectSet ImportFromAiSceneWithSingleObject( Ai.Scene aiScene, string texturesDirectoryPath )
        {
            var objectSet = new ObjectSet
            {
                TextureSet = new TextureSet()
            };

            var obj = CreateObjectFromAiNode( aiScene.RootNode, aiScene, objectSet, texturesDirectoryPath );

            if ( obj?.Meshes?.Count > 0 )
                objectSet.Objects.Add( obj );

            objectSet.TextureIds.AddRange( objectSet.TextureSet.Textures.Select( x => x.Id ) );

            return objectSet;
        }

        private static Matrix4x4 GetWorldTransform( Ai.Node aiNode )
        {
            var transform = aiNode.Transform;

            while ( ( aiNode = aiNode.Parent ) != null )
                transform *= aiNode.Transform;

            return transform.ToNumericsTransposed();
        }

        private static Object CreateObjectFromAiNode( Ai.Node aiNode, Ai.Scene aiScene, ObjectSet objectSet, string texturesDirectoryPath )
        {
            var obj = new Object();

            obj.Name = aiNode.Name;
            obj.Id = MurmurHash.Calculate( aiNode.Name );
            obj.Skin = new Skin();

            var aabb = new AxisAlignedBoundingBox();

            CreateRecursively( aiNode );

            void CreateRecursively( Ai.Node aiChildNode )
            {
                if ( aiChildNode.HasMeshes )
                {
                    var meshes = CreateMeshesFromAiNode( aiChildNode, aiScene, obj, ref aabb, objectSet,
                        texturesDirectoryPath );

                    if ( meshes?.Count > 0 )
                        obj.Meshes.AddRange( meshes );
                }

                foreach ( var aiAlsoChildNode in aiChildNode.Children )
                    CreateRecursively( aiAlsoChildNode );
            }

            obj.BoundingSphere = aabb.ToBoundingSphere();

            if ( obj.Skin.Bones.Count > 0 )
            {
                foreach ( var boneInfo in obj.Skin.Bones )
                {
                    var aiBoneNode = aiScene.RootNode.FindNode( boneInfo.Name );
                    if ( aiBoneNode.Parent == null || aiBoneNode.Parent == aiScene.RootNode )
                        continue;

                    boneInfo.Parent = obj.Skin.Bones.FirstOrDefault( x => x.Name == aiBoneNode.Parent.Name );
                }
            }

            else
            {
                obj.Skin = null;
            }

            return obj;
        }

        private static List<Mesh> CreateMeshesFromAiNode( Ai.Node aiNode, Ai.Scene aiScene, Object obj, 
            ref AxisAlignedBoundingBox objectAABB, ObjectSet objectSet, string texturesDirectoryPath )
        {
            var aiMeshes = aiNode.MeshIndices.Select( x => aiScene.Meshes[ x ] ).Where( x => x.PrimitiveType == Ai.PrimitiveType.Triangle )
                .OrderBy( x => x.VertexCount ).ThenBy( x => x.FaceCount ).ToList();

            if ( aiMeshes.Count == 0 )
                return null;

            var meshes = new List<Mesh>();
            var transform = GetWorldTransform( aiNode );

            int subIndex = 0;

            for ( int index = 0; index < aiMeshes.Count; )
            {
                int begin = index;
                int end = index + 1;

                int vertexCount = aiMeshes[ index ].VertexCount;

                // Collect meshes till we hit the vertex count limit.
                while ( end < aiMeshes.Count )
                {
                    if ( vertexCount + aiMeshes[ end ].VertexCount > 32768 )
                        break;

                    vertexCount += aiMeshes[ end++ ].VertexCount;
                }

                var mesh = new Mesh { Name = aiNode.Name };
                if ( index != 0 )
                    mesh.Name += "." + ( ++subIndex ).ToString( "D3" );

                var aabbMesh = new AxisAlignedBoundingBox();

                int vertexOffset = 0;

                for ( int it = begin; it < end; it++ ) // we goin C++
                {
                    var aiMesh = aiMeshes[ it ];
                    var aabbSubMesh = new AxisAlignedBoundingBox();

                    if ( aiMesh.HasVertices )
                    {
                        if ( mesh.Positions == null )
                            mesh.Positions = new Vector3[ vertexCount ];

                        for ( int i = 0; i < aiMesh.VertexCount; i++ )
                        {
                            var position = Vector3.Transform( aiMesh.Vertices[ i ].ToNumerics(), transform );

                            mesh.Positions[ vertexOffset + i ] = position;

                            aabbSubMesh.AddPoint( position );
                        }
                    }

                    if ( aiMesh.HasNormals )
                    {
                        if ( mesh.Normals == null )
                            mesh.Normals = new Vector3[ vertexCount ];

                        for ( int i = 0; i < aiMesh.VertexCount; i++ )
                            mesh.Normals[ vertexOffset + i ] = Vector3.Normalize( Vector3.TransformNormal( aiMesh.Normals[ i ].ToNumerics(), transform ) );
                    }

                    for ( int i = 0; i < 4; i++ )
                    {
                        if ( !aiMesh.HasTextureCoords( i ) )
                            continue;

                        var texCoords = mesh.GetTexCoordsChannel( i );

                        if ( texCoords == null )
                        {
                            texCoords = new Vector2[ vertexCount ];
                            mesh.SetTexCoordsChannel( i, texCoords );
                        }

                        for ( int j = 0; j < aiMesh.VertexCount; j++ )
                        {
                            var texCoord = new Vector2(
                                aiMesh.TextureCoordinateChannels[ i ][ j ].X,
                                aiMesh.TextureCoordinateChannels[ i ][ j ].Y );

                            texCoords[ vertexOffset + j ] = texCoord;
                        }
                    }

                    for ( int i = 0; i < 2; i++ )
                    {
                        if ( !aiMesh.HasVertexColors( i ) )
                            continue;

                        var colors = mesh.GetColorsChannel( i );

                        if ( colors == null )
                        {
                            colors = new Color[ vertexCount ];
                            for ( int j = 0; j < vertexCount; j++ )
                                colors[ j ] = Color.White;

                            mesh.SetColorsChannel( i, colors );
                        }

                        for ( int j = 0; j < aiMesh.VertexCount; j++ )
                            colors[ vertexOffset + j ] = aiMesh.VertexColorChannels[ i ][ j ].ToMML();
                    }

                    var subMesh = new SubMesh();

                    if ( aiMesh.HasBones )
                    {
                        if ( mesh.BoneWeights == null )
                        {
                            mesh.BoneWeights = new BoneWeight[ vertexCount ];
                            for ( int i = 0; i < mesh.BoneWeights.Length; i++ )
                                mesh.BoneWeights[ i ] = BoneWeight.Empty;
                        }

                        subMesh.BoneIndices = new ushort[ aiMesh.BoneCount ];

                        for ( int i = 0; i < aiMesh.BoneCount; i++ )
                        {
                            var aiBone = aiMesh.Bones[ i ];

                            int boneIndex = obj.Skin.Bones.FindIndex(
                                x => x.Name == aiBone.Name );

                            if ( boneIndex == -1 )
                            {
                                boneIndex = obj.Skin.Bones.Count;

                                var aiBoneNode = aiScene.RootNode.FindNode( aiBone.Name );

                                // This is not right, but I'm not sure how to transform the bind pose matrix
                                // while not having duplicate bones.
                                Matrix4x4.Invert( GetWorldTransform( aiBoneNode ), out var inverseBindPoseMatrix );

                                obj.Skin.Bones.Add( new BoneInfo
                                {
                                    Name = aiBoneNode.Name,
                                    InverseBindPoseMatrix = inverseBindPoseMatrix
                                } );
                            }

                            foreach ( var boneWeight in aiBone.VertexWeights )
                                mesh.BoneWeights[ vertexOffset + boneWeight.VertexID ].AddWeight( i, boneWeight.Weight );

                            subMesh.BoneIndices[ i ] = ( ushort ) boneIndex;
                        }

                        subMesh.BonesPerVertex = 4;
                    }

                    subMesh.Indices = aiMesh.Faces.Where( x => x.IndexCount == 3 ).SelectMany( x => x.Indices )
                        .Select( x => ( uint ) ( vertexOffset + x ) ).ToArray();

                    subMesh.PrimitiveType = PrimitiveType.Triangles;
                    subMesh.IndexFormat = IndexFormat.UInt16;

                    var aiMaterial = aiScene.Materials[ aiMesh.MaterialIndex ];

                    int materialIndex = obj.Materials.FindIndex( x => x.Name == aiMaterial.Name );

                    if ( materialIndex == -1 )
                    {
                        materialIndex = obj.Materials.Count;
                        obj.Materials.Add( CreateMaterialFromAiMaterial( aiMaterial, objectSet.TextureSet, texturesDirectoryPath ) );
                    }

                    subMesh.MaterialIndex = ( uint ) materialIndex;

                    subMesh.BoundingSphere = aabbSubMesh.ToBoundingSphere();
                    subMesh.BoundingBox = aabbSubMesh.ToBoundingBox();

                    mesh.SubMeshes.Add( subMesh );

                    vertexOffset += aiMesh.VertexCount;

                    aabbMesh.Merge( aabbSubMesh );
                }

                if ( mesh.BoneWeights != null )
                {
                    for ( int i = 0; i < mesh.BoneWeights.Length; i++ )
                        mesh.BoneWeights[ i ].Validate();
                }

                mesh.GenerateTangents();

                mesh.BoundingSphere = aabbMesh.ToBoundingSphere();
                meshes.Add( mesh );

                objectAABB.Merge( aabbMesh );

                // Go to the next mesh
                index = end;
            }

            return meshes;
        }

        private static Material CreateMaterialFromAiMaterial( Ai.Material aiMaterial, TextureSet textureSet, string texturesDirectoryPath )
        {
            var material = new Material { Name = aiMaterial.Name };

            foreach ( string shaderName in Material.ShaderNames )
            {
                if ( material.Name.IndexOf( shaderName, StringComparison.OrdinalIgnoreCase ) == -1 )
                    continue;

                material.ShaderName = shaderName == "CHARA" ? "SKIN" : shaderName;
                break;
            }

            material.Diffuse = aiMaterial.HasColorDiffuse ? aiMaterial.ColorDiffuse.ToMML() : material.Diffuse;
            material.Ambient = aiMaterial.HasColorAmbient ? aiMaterial.ColorAmbient.ToMML() : material.Ambient;
            material.Specular = aiMaterial.HasColorSpecular ? aiMaterial.ColorSpecular.ToMML() : material.Specular;
            material.Emission = aiMaterial.HasColorEmissive ? aiMaterial.ColorEmissive.ToMML() : material.Emission;
            material.Shininess = aiMaterial.HasShininess ? aiMaterial.Shininess : material.Shininess;

            if ( material.ShaderName == "HAIR" )
                material.AnisoDirection = AnisoDirection.V;

            material.DoubleSided = aiMaterial.HasTwoSided ? aiMaterial.IsTwoSided : material.DoubleSided;

            foreach ( var aiTextureSlot in aiMaterial.GetAllMaterialTextures() )
                CreateMaterialTextureFromAiTextureSlot( aiTextureSlot, aiMaterial, material, textureSet, texturesDirectoryPath );

            if ( material.Flags.HasFlag( MaterialFlags.Normal ) )
                material.BumpMapType = material.Flags.HasFlag( MaterialFlags.Environment ) ? BumpMapType.Env : BumpMapType.Dot;

            if ( material.Flags.HasFlag( MaterialFlags.Specular ) )
                material.LineLight = 5;

            if ( material.Flags.HasFlag( MaterialFlags.Environment ) )
                material.Flags |= MaterialFlags.ColorL1Alpha | MaterialFlags.ColorL2Alpha | MaterialFlags.OverrideIBL;

            // HAIR without normal map causes the whole screen to become black.
            // TODO: Automatically add a flat normal map in this case.
            if ( material.ShaderName == "HAIR" && !material.Flags.HasFlag( MaterialFlags.Normal ) )
                material.ShaderName = "BLINN";

            material.SortMaterialTextures();

            return material;
        }

        private static MaterialTexture CreateMaterialTextureFromAiTextureSlot( Ai.TextureSlot aiTextureSlot,
            Ai.Material aiMaterial, Material material, TextureSet textureSet, string texturesDirectoryPath )
        {
            MaterialTextureType type;
            MaterialFlags flags;

            // skyth y u no use dictionary
            switch ( aiTextureSlot.TextureType )
            {
                case Ai.TextureType.Diffuse:
                case Ai.TextureType.Ambient:
                    type = MaterialTextureType.Color;
                    flags = MaterialFlags.Color;
                    break;

                case Ai.TextureType.Specular:
                    type = MaterialTextureType.Specular;
                    flags = MaterialFlags.Specular;
                    break;

                case Ai.TextureType.Normals:
                    type = MaterialTextureType.Normal;
                    flags = MaterialFlags.Normal;
                    break;

                case Ai.TextureType.Opacity:
                    type = MaterialTextureType.Transparency;
                    flags = MaterialFlags.Transparency;
                    break;

                case Ai.TextureType.Reflection:
                    type = MaterialTextureType.Reflection;
                    flags = MaterialFlags.Environment;
                    break;

                default:
                    return null;
            }

            int materialTextureIndex = -1;

            for ( int i = 0; i < material.MaterialTextures.Length; i++ )
            {
                if ( material.MaterialTextures[ i ].Type != MaterialTextureType.None )
                    continue;

                materialTextureIndex = i;
                break;
            }

            if ( materialTextureIndex == -1 )
                return null;

            var texture = CreateTextureFromFilePath( aiTextureSlot.FilePath, 
                type == MaterialTextureType.Normal ? TextureFormat.ATI2 : TextureFormat.Unknown, texturesDirectoryPath, textureSet );
                    
            if ( texture == null )
                return null;

            var materialTexture = new MaterialTexture();

            materialTexture.RepeatU = aiTextureSlot.WrapModeU == Ai.TextureWrapMode.Wrap;
            materialTexture.RepeatV = aiTextureSlot.WrapModeV == Ai.TextureWrapMode.Wrap;
            materialTexture.MirrorU = aiTextureSlot.WrapModeU == Ai.TextureWrapMode.Mirror;
            materialTexture.MirrorV = aiTextureSlot.WrapModeV == Ai.TextureWrapMode.Mirror;
            materialTexture.ClampToEdge = aiTextureSlot.WrapModeU == Ai.TextureWrapMode.Clamp &&
                                          aiTextureSlot.WrapModeV == Ai.TextureWrapMode.Clamp;

            if ( texture.ArraySize == 6 || aiTextureSlot.Mapping == Ai.TextureMapping.Box )
            {
                materialTexture.TextureCoordinateTranslationType = MaterialTextureCoordinateTranslationType.Cube;

                if ( type == MaterialTextureType.Reflection )
                    type = MaterialTextureType.EnvironmentCube;
            }

            else if ( aiTextureSlot.Mapping == Ai.TextureMapping.Sphere )
            {
                materialTexture.TextureCoordinateTranslationType = MaterialTextureCoordinateTranslationType.Sphere;

                if ( type == MaterialTextureType.Reflection )
                    type = MaterialTextureType.EnvironmentSphere;
            }

            materialTexture.Blend = type == MaterialTextureType.Specular ? 1u : 7;
            materialTexture.Filter = 2;
            materialTexture.MipMap = 2;

            materialTexture.Type = type;
            materialTexture.TextureId = texture.Id;

            material.Flags |= flags;
            material.MaterialTextures[ materialTextureIndex ] = materialTexture;

            return materialTexture;
        }

        private static Texture CreateTextureFromFilePath( string filePath, TextureFormat formatHint, string texturesDirectoryPath, TextureSet textureSet )
        {
            string textureName = Path.GetFileNameWithoutExtension( filePath );

            var texture = textureSet.Textures.FirstOrDefault( x =>
                x.Name.Equals( textureName, StringComparison.OrdinalIgnoreCase ) );

            if ( texture != null )
                return texture;

            string newFilePath = FindTexturePath( filePath, texturesDirectoryPath );

            if ( string.IsNullOrEmpty( newFilePath ) )
                return null;

            texture = TextureEncoder.EncodeFromFile( newFilePath, formatHint, true );

            texture.Name = textureName;
            texture.Id = MurmurHash.Calculate( textureName );

            textureSet.Textures.Add( texture );

            return texture;
        }

        private static string FindTexturePath( string filePath, string texturesDirectoryPath )
        {
            // Try full path.
            string newFilePath = TryExtensions( filePath );

            if ( !string.IsNullOrEmpty( newFilePath ) )
                return newFilePath;

            // Try relative path.
            newFilePath = TryExtensions( Path.Combine( texturesDirectoryPath, filePath ) );

            if ( !string.IsNullOrEmpty( newFilePath ) )
                return newFilePath;

            // Try textures directory.
            newFilePath = TryExtensions( Path.Combine( 
                texturesDirectoryPath, Path.GetFileName( filePath ) ) );

            if ( !string.IsNullOrEmpty( newFilePath ) )
                return newFilePath;

            return null;
        }

        private static string TryExtensions( string filePath )
        {
            // Try original extension.
            if ( File.Exists( filePath ) )
                return filePath;

            // Try DDS extension.
            filePath = Path.ChangeExtension( filePath, "dds" );

            if ( File.Exists( filePath ) ) 
                return filePath;

            // Try PNG extension. 
            filePath = Path.ChangeExtension( filePath, "png" );

            if ( File.Exists( filePath ) ) 
                return filePath;

            return null;
        }
    }
}