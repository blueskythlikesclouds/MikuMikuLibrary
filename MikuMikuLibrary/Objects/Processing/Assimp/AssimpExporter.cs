using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Ai = Assimp;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Textures;
using MikuMikuLibrary.Textures.Processing;

namespace MikuMikuLibrary.Objects.Processing.Assimp
{
    public static class AssimpExporter
    {
        private static readonly IReadOnlyDictionary<MaterialTextureType, Ai.TextureType> sTextureTypeMap =
            new Dictionary<MaterialTextureType, Ai.TextureType>
            {
                { MaterialTextureType.Color, Ai.TextureType.Diffuse },
                { MaterialTextureType.Normal, Ai.TextureType.Normals },
                { MaterialTextureType.Specular, Ai.TextureType.Specular },
                { MaterialTextureType.Height, Ai.TextureType.Height },
                { MaterialTextureType.Reflection, Ai.TextureType.Reflection },
                { MaterialTextureType.Transparency, Ai.TextureType.Opacity },
                { MaterialTextureType.EnvironmentCube, Ai.TextureType.Reflection },
                { MaterialTextureType.EnvironmentSphere, Ai.TextureType.Reflection }
            };

        public static void ExportToFile( ObjectSet objectSet, string outputFilePath )
        {
            AssimpSceneHelper.Export( ExportToAiScene( objectSet ), outputFilePath, Ai.PostProcessSteps.FlipUVs );

            if ( objectSet.TextureSet == null )
                return;

            string outputDirectoryPath = Path.GetDirectoryName( outputFilePath );

            foreach ( var texture in objectSet.TextureSet.Textures )
            {
                string extension = TextureFormatUtilities.IsBlockCompressed( texture.Format ) && !texture.IsYCbCr ? ".dds" : ".png";
                TextureDecoder.DecodeToFile( texture, Path.Combine( outputDirectoryPath, texture.Name + extension ) );
            }
        }

        public static Ai.Scene ExportToAiScene( ObjectSet objectSet )
        {
            if ( objectSet.TextureSet != null )
            {
                foreach ( var texture in objectSet.TextureSet.Textures
                    .Where( texture => string.IsNullOrEmpty( texture.Name ) ) )
                {
                    texture.Name = Guid.NewGuid().ToString();
                }
            }

            var aiScene = new Ai.Scene { RootNode = new Ai.Node( "RootNode" ) };

            var convertedMaterials = new HashSet<string>();
            foreach ( var obj in objectSet.Objects )
            {
                foreach ( var material in obj.Materials )
                {
                    var aiMaterial = CreateAiMaterialFromMaterial( material, objectSet.TextureSet );
                    if ( convertedMaterials.Contains( aiMaterial.Name ) )
                        aiMaterial.Name += "+" + material.Name;

                    aiScene.Materials.Add( aiMaterial );
                    convertedMaterials.Add( aiMaterial.Name );
                }
            }

            var convertedBones = new Dictionary<string, Ai.Node>();
            var gblctrNode = new Ai.Node( "gblctr" );

            foreach ( var obj in objectSet.Objects )
            {
                if ( obj.Skin != null )
                    CreateAiNodesFromBoneInfos( obj.Skin.Bones, aiScene, gblctrNode, null, convertedBones );

                aiScene.RootNode.Children.Add( CreateAiNodeFromObject( obj, aiScene ) );
            }

            if ( gblctrNode.HasChildren )
                aiScene.RootNode.Children.Add( gblctrNode );

            return aiScene;
        }

        private static Ai.Node CreateAiNodeFromObject( Object obj, Ai.Scene aiScene )
        {
            var aiObjectNode = new Ai.Node( obj.Name );

            foreach ( var mesh in obj.Meshes )
            {
                for ( int i = 0; i < mesh.SubMeshes.Count; i++ )
                {
                    var subMesh = mesh.SubMeshes[ i ];

                    string name = mesh.Name;
                    if ( i > 0 )
                        name += "." + i.ToString( "D3" );

                    var aiSubMeshNode = new Ai.Node( name );
                    var aiMesh = CreateAiMeshFromSubMesh( subMesh, mesh, obj, aiScene, name );

                    aiSubMeshNode.MeshIndices.Add( aiScene.Meshes.Count );
                    aiScene.Meshes.Add( aiMesh );

                    aiObjectNode.Children.Add( aiSubMeshNode );
                }
            }

            return aiObjectNode;
        }

        private static Ai.Node CreateAiNodeFromBoneInfo( BoneInfo boneInfo, Matrix4x4 inverseParentTransformation )
        {
            var aiNode = new Ai.Node( boneInfo.Name );

            Matrix4x4.Invert( boneInfo.InverseBindPoseMatrix, out var transformation );

            aiNode.Transform = Matrix4x4.Multiply( transformation, inverseParentTransformation ).ToAssimpTransposed();

            return aiNode;
        }

        private static void CreateAiNodesFromBoneInfos( List<BoneInfo> boneInfos, Ai.Scene aiScene,
            Ai.Node aiParentBone, BoneInfo parentBoneInfo, Dictionary<string, Ai.Node> convertedBones )
        {
            foreach ( var boneInfo in boneInfos )
            {
                if ( boneInfo.Parent != parentBoneInfo )
                    continue;

                if ( !convertedBones.TryGetValue( boneInfo.Name, out var aiBoneNode ) )
                {
                    aiBoneNode = CreateAiNodeFromBoneInfo( boneInfo, parentBoneInfo?.InverseBindPoseMatrix ?? Matrix4x4.Identity );

                    if ( aiParentBone == null )
                        aiScene.RootNode.Children.Add( aiBoneNode );
                    else
                        aiParentBone.Children.Add( aiBoneNode );

                    convertedBones.Add( boneInfo.Name, aiBoneNode );
                }

                CreateAiNodesFromBoneInfos( boneInfos, aiScene, aiBoneNode, boneInfo, convertedBones );
            }
        }

        private static Ai.Mesh CreateAiMeshFromSubMesh( SubMesh subMesh, Mesh mesh, Object obj, Ai.Scene aiScene, string name )
        {
            var aiMesh = new Ai.Mesh( name, Ai.PrimitiveType.Triangle );

            if ( mesh.Positions != null )
            {
                aiMesh.Vertices.Capacity = mesh.Positions.Length;
                aiMesh.Vertices.AddRange( mesh.Positions.Select( x => x.ToAssimp() ) );
            }

            if ( mesh.Normals != null )
            {
                aiMesh.Normals.Capacity = mesh.Normals.Length;
                aiMesh.Normals.AddRange( mesh.Normals.Select( x => x.ToAssimp() ) );
            }

            for ( int i = 0; i < 4; i++ )
            {
                var texCoords = mesh.GetTexCoordsChannel( i );

                if ( texCoords == null )
                    continue;

                aiMesh.TextureCoordinateChannels[ i ].Capacity = texCoords.Length;
                aiMesh.TextureCoordinateChannels[ i ].AddRange( texCoords.Select( x => new Ai.Vector3D( x.ToAssimp(), 0 ) ) );
            }

            for ( int i = 0; i < 2; i++ )
            {
                var colors = mesh.GetColorsChannel( i );

                if ( colors == null )
                    continue;

                aiMesh.VertexColorChannels[ i ].Capacity = colors.Length;
                aiMesh.VertexColorChannels[ i ].AddRange( colors.Select( x => x.ToAssimp() ) );
            }

            if ( mesh.BoneWeights != null )
            {
                for ( int i = 0; i < subMesh.BoneIndices.Length; i++ )
                {
                    ushort boneIndex = subMesh.BoneIndices[ i ];
                    var bone = obj.Skin.Bones[ boneIndex ];

                    var aiBone = new Ai.Bone();

                    aiBone.Name = bone.Name;
                    aiBone.OffsetMatrix = bone.InverseBindPoseMatrix.ToAssimpTransposed();

                    for ( int j = 0; j < mesh.BoneWeights.Length; j++ )
                    {
                        var boneWeight = mesh.BoneWeights[ j ];

                        if ( boneWeight.Index1 == i )
                            aiBone.VertexWeights.Add( new Ai.VertexWeight( j, boneWeight.Weight1 ) );

                        if ( boneWeight.Index2 == i )
                            aiBone.VertexWeights.Add( new Ai.VertexWeight( j, boneWeight.Weight2 ) );

                        if ( boneWeight.Index3 == i )
                            aiBone.VertexWeights.Add( new Ai.VertexWeight( j, boneWeight.Weight3 ) );

                        if ( boneWeight.Index4 == i )
                            aiBone.VertexWeights.Add( new Ai.VertexWeight( j, boneWeight.Weight4 ) );
                    }

                    aiMesh.Bones.Add( aiBone );
                }
            }

            var triangles = subMesh.GetTriangles();

            aiMesh.Faces.Capacity = triangles.Count;

            aiMesh.Faces.AddRange( triangles.Select( x =>
            {
                var aiFace = new Ai.Face();
                aiFace.Indices.Capacity = 3;
                aiFace.Indices.Add( ( int ) x.A );
                aiFace.Indices.Add( ( int ) x.B );
                aiFace.Indices.Add( ( int ) x.C );
                return aiFace;
            } ) );

            var material = obj.Materials[ ( int ) subMesh.MaterialIndex ];

            int materialIndex = aiScene.Materials.FindIndex( x => x.Name == material.Name + "+" + obj.Name );

            if ( materialIndex == -1 )
                materialIndex = aiScene.Materials.FindIndex( x => x.Name == material.Name );

            aiMesh.MaterialIndex = materialIndex;

            return aiMesh;
        }

        private static Ai.Material CreateAiMaterialFromMaterial( Material material, TextureSet textureSet )
        {
            var aiMaterial = new Ai.Material();

            aiMaterial.Name = material.Name;
            aiMaterial.ColorDiffuse = material.Diffuse.ToAssimp();
            aiMaterial.ColorAmbient = material.Ambient.ToAssimp();
            aiMaterial.ColorSpecular = material.Ambient.ToAssimp();
            aiMaterial.ColorEmissive = material.Emission.ToAssimp();
            aiMaterial.Shininess = material.Shininess;
            aiMaterial.IsTwoSided = material.DoubleSided;

            var exportedTypes = new HashSet<MaterialTextureType>();

            foreach ( var materialTexture in material.MaterialTextures )
            {
                if ( !sTextureTypeMap.TryGetValue( materialTexture.Type, out var type ) ||
                     exportedTypes.Contains( materialTexture.Type ) )
                    continue;

                exportedTypes.Add( materialTexture.Type );

                var texture = textureSet?.Textures?.FirstOrDefault( x => x.Id == materialTexture.TextureId );
                if ( texture == null )
                    continue;

                var aiTextureSlot = new Ai.TextureSlot();

                if ( TextureFormatUtilities.IsBlockCompressed( texture.Format ) && !texture.IsYCbCr )
                    aiTextureSlot.FilePath = texture.Name + ".dds";

                else
                    aiTextureSlot.FilePath = texture.Name + ".png";

                aiTextureSlot.TextureType = type;

                aiMaterial.AddMaterialTexture( in aiTextureSlot );
            }

            return aiMaterial;
        }
    }
}