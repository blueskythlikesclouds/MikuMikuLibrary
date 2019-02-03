using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Ai = Assimp;

namespace MikuMikuLibrary.Models.Processing.Assimp
{
    public static class Importer
    {
        private static Random sRandom = new Random();

        public static Model ConvertModelFromAiScene( string filePath )
        {
            string texturesDirectory = Path.GetDirectoryName( filePath );
            return ConvertModelFromAiScene( SceneUtilities.Import( filePath ), texturesDirectory );
        }

        public static Model ConvertModelFromAiScene( Ai.Scene aiScene, string texturesDirectory )
        {
            var model = new Model
            {
                TextureSet = new TextureSet()
            };

            var transformation = aiScene.RootNode.Transform.ToNumericsTransposed();
            foreach ( var aiNode in aiScene.RootNode.Children )
            {
                var mesh = ConvertMeshFromAiNode( aiNode, aiScene, transformation, texturesDirectory, model.TextureSet );

                if ( mesh != null )
                    model.Meshes.Add( mesh );
            }

            model.TextureIds.AddRange( model.TextureSet.Textures.Select( x => x.Id ) );
            return model;
        }

        public static Model ConvertModelFromAiSceneWithSingleMesh( string filePath )
        {
            string texturesDirectory = Path.GetDirectoryName( filePath );
            return ConvertModelFromAiSceneWithSingleMesh( SceneUtilities.Import( filePath ), texturesDirectory );
        }

        public static Model ConvertModelFromAiSceneWithSingleMesh( Ai.Scene aiScene, string texturesDirectory )
        {
            var model = new Model
            {
                TextureSet = new TextureSet()
            };

            model.Meshes.Add( ConvertMeshFromAiNode( aiScene.RootNode, aiScene, Matrix4x4.Identity, texturesDirectory, model.TextureSet ) );

            model.TextureIds.AddRange( model.TextureSet.Textures.Select( x => x.Id ) );

            return model;
        }

        private static Ai.Matrix4x4 GetGlobalTransformation( Ai.Node aiNode )
        {
            var transform = Ai.Matrix4x4.Identity;

            for ( var node = aiNode; aiNode != null; aiNode = aiNode.Parent )
                transform = transform * aiNode.Transform;

            return transform;
        }

        private static Bone ConvertBoneFromAiBone( Ai.Bone aiBone, Ai.Scene aiScene, int boneId )
        {
            Matrix4x4 inverseTransformation;

            var aiBoneNode = aiScene.RootNode.FindNode( aiBone.Name );
            if ( aiBoneNode != null )
                Matrix4x4.Invert( GetGlobalTransformation( aiBoneNode ).ToNumerics(), out inverseTransformation );
            else
                inverseTransformation = aiBone.OffsetMatrix.ToNumerics();

            return new Bone
            {
                Name = aiBone.Name,
                Id = boneId,
                Matrix = inverseTransformation,
            };
        }

        private static Texture ConvertTexture( string textureFilePath, TextureSet textureSet )
        {
            if ( !File.Exists( textureFilePath ) )
                return null;

            string textureName = Path.GetFileNameWithoutExtension( textureFilePath );

            Texture texture;
            if ( ( texture = textureSet.Textures.FirstOrDefault( x => x.Name.Equals( textureName, StringComparison.OrdinalIgnoreCase ) ) ) != null )
                return texture;

            texture = TextureEncoder.Encode( textureFilePath );
            texture.Name = textureName;
            texture.Id = sRandom.Next( int.MinValue, int.MaxValue );
            textureSet.Textures.Add( texture );

            return texture;
        }

        private static Texture ConvertTexture( string textureName, string texturesDirectory, TextureSet textureSet )
        {
            return ConvertTexture( Path.Combine( texturesDirectory, textureName ), textureSet );
        }

        private static Material ConvertMaterialFromAiMaterial( Ai.Material aiMaterial, string texturesDirectory, TextureSet textureSet )
        {
            Material material = new Material();

            material.Shader = "BLINN";
            material.Field02 = 2688;
            material.Name = aiMaterial.Name;
            material.Field25 = 1;

            if ( aiMaterial.HasColorDiffuse )
                material.DiffuseColor = aiMaterial.ColorDiffuse.ToMML();
            else
                material.DiffuseColor = new Color( 1, 1, 1, 1 );

            if ( aiMaterial.HasColorAmbient )
                material.AmbientColor = aiMaterial.ColorAmbient.ToMML();
            else
                material.AmbientColor = new Color( 1, 1, 1, 1 );

            if ( aiMaterial.HasColorSpecular )
                material.SpecularColor = aiMaterial.ColorSpecular.ToMML();
            else
                material.SpecularColor = new Color( 0.5f, 0.5f, 0.5f, 1 );

            if ( aiMaterial.HasColorEmissive )
                material.EmissionColor = aiMaterial.ColorEmissive.ToMML();
            else
                material.EmissionColor = new Color( 0, 0, 0, 1 );

            if ( aiMaterial.HasShininess && aiMaterial.ShadingMode == Ai.ShadingMode.Phong )
                material.Shininess = aiMaterial.Shininess;
            else
                material.Shininess = 50f;

            Texture texture;
            if ( aiMaterial.HasTextureDiffuse && ( texture = ConvertTexture( aiMaterial.TextureDiffuse.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 1;
                material.Diffuse.TextureId = texture.Id;
                material.Diffuse.Field02 = 241;
            }

            if ( aiMaterial.HasTextureAmbient && ( texture = ConvertTexture( aiMaterial.TextureAmbient.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Ambient.TextureId = texture.Id;
                material.Ambient.Field02 = 241;
            }

            if ( aiMaterial.HasTextureNormal && ( texture = ConvertTexture( aiMaterial.TextureNormal.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 256;
                material.Normal.TextureId = texture.Id;
                material.Normal.Field02 = 242;
            }

            if ( aiMaterial.HasTextureSpecular && ( texture = ConvertTexture( aiMaterial.TextureSpecular.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 128;
                material.Specular.TextureId = texture.Id;
                material.Specular.Field02 = 243;
            }

            if ( aiMaterial.HasTextureReflection && ( texture = ConvertTexture( aiMaterial.TextureReflection.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 33832;
                material.Reflection.TextureId = texture.Id;
                material.Reflection.Field02 = 1017;
            }

            if ( aiMaterial.GetMaterialTexture( Ai.TextureType.Shininess, 0, out Ai.TextureSlot shininess ) && ( texture = ConvertTexture( shininess.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 8192;
                material.SpecularPower.TextureId = texture.Id;
                material.SpecularPower.Field02 = 246;
            }

            foreach ( var materialTexture in material.MaterialTextures )
            {
                if ( materialTexture == material.Diffuse )
                    materialTexture.Field00 = materialTexture.IsActive ? 82288 : 48;

                materialTexture.Field05 = 1;

                if ( materialTexture.IsActive )
                {
                    materialTexture.Field01 = 23331040;
                    materialTexture.Field06 = 1;
                    materialTexture.Field11 = 1;
                    materialTexture.Field16 = 1;
                    materialTexture.Field21 = 1;
                }
            }

            return material;
        }

        private static SubMesh ConvertSubMeshFromAiNode( Ai.Node aiNode, Ai.Scene aiScene, Matrix4x4 parentTransformation, Dictionary<string, int> boneMap, List<Bone> bones, Dictionary<string, int> materialMap, List<Material> materials, string texturesDirectory, TextureSet textureSet )
        {
            if ( !aiNode.HasMeshes )
                return null;

            // Select meshes that have triangles
            var aiMeshes = aiNode.MeshIndices.Select( x => aiScene.Meshes[ x ] ).Where( x =>
                x.PrimitiveType == Ai.PrimitiveType.Triangle && x.Faces.Any( y => y.IndexCount == 3 ) ).ToList();

            if ( aiMeshes.Count == 0 )
                return null;

            var transformation = parentTransformation * aiNode.Transform.ToNumericsTransposed();
            int vertexCount = aiMeshes.Sum( x => x.VertexCount );

            var subMesh = new SubMesh
            {
                Name = aiNode.Name,
                Vertices = new Vector3[ vertexCount ],
            };

            int vertexOffset = 0;
            foreach ( var aiMesh in aiMeshes )
            {
                for ( int i = 0; i < aiMesh.Vertices.Count; i++ )
                    subMesh.Vertices[ vertexOffset + i ] = Vector3.Transform( aiMesh.Vertices[ i ].ToNumerics(), transformation );

                if ( aiMesh.HasNormals )
                {
                    if ( subMesh.Normals == null )
                        subMesh.Normals = new Vector3[ vertexCount ];

                    for ( int i = 0; i < aiMesh.Normals.Count; i++ )
                        subMesh.Normals[ vertexOffset + i ] = Vector3.Normalize( Vector3.TransformNormal( aiMesh.Normals[ i ].ToNumerics(), transformation ) );
                }

                if ( aiMesh.HasTangentBasis )
                {
                    if ( subMesh.Tangents == null )
                        subMesh.Tangents = new Vector4[ vertexCount ];

                    for ( int i = 0; i < aiMesh.Tangents.Count; i++ )
                    {
                        Vector3 tangent = Vector3.TransformNormal( aiMesh.Tangents[ i ].ToNumerics(), transformation );
                        Vector3 bitangent = Vector3.TransformNormal( aiMesh.BiTangents[ i ].ToNumerics(), transformation );
                        int direction = Math.Sign( Vector3.Dot( bitangent, Vector3.Cross( subMesh.Normals[ vertexOffset + i ], tangent ) ) );

                        subMesh.Tangents[ vertexOffset + i ] = new Vector4( tangent, direction );
                    }
                }

                if ( aiMesh.HasTextureCoords( 0 ) )
                {
                    if ( subMesh.UVChannel1 == null )
                        subMesh.UVChannel1 = new Vector2[ vertexCount ];

                    for ( int i = 0; i < aiMesh.TextureCoordinateChannels[ 0 ].Count; i++ )
                        subMesh.UVChannel1[ vertexOffset + i ] = new Vector2( aiMesh.TextureCoordinateChannels[ 0 ][ i ].X, 1f - aiMesh.TextureCoordinateChannels[ 0 ][ i ].Y );
                }

                if ( aiMesh.HasTextureCoords( 1 ) )
                {
                    if ( subMesh.UVChannel2 == null )
                        subMesh.UVChannel2 = new Vector2[ vertexCount ];

                    for ( int i = 0; i < aiMesh.TextureCoordinateChannels[ 1 ].Count; i++ )
                        subMesh.UVChannel2[ vertexOffset + i ] = new Vector2( aiMesh.TextureCoordinateChannels[ 1 ][ i ].X, 1f - aiMesh.TextureCoordinateChannels[ 1 ][ i ].Y );
                }

                if ( aiMesh.HasVertexColors( 0 ) )
                {
                    if ( subMesh.Colors == null )
                    {
                        subMesh.Colors = new Color[ vertexCount ];
                        for ( int i = 0; i < subMesh.Colors.Length; i++ )
                            subMesh.Colors[ i ] = Color.White;
                    }

                    for ( int i = 0; i < aiMesh.VertexColorChannels[ 0 ].Count; i++ )
                        subMesh.Colors[ vertexOffset + i ] = new Color( aiMesh.VertexColorChannels[ 0 ][ i ].R, aiMesh.VertexColorChannels[ 0 ][ i ].G, aiMesh.VertexColorChannels[ 0 ][ i ].B, aiMesh.VertexColorChannels[ 0 ][ i ].A );
                }

                var indexTable = new IndexTable();

                if ( aiMesh.HasBones )
                {
                    if ( subMesh.BoneWeights == null )
                    {
                        subMesh.BoneWeights = new BoneWeight[ vertexCount ];
                        for ( int i = 0; i < subMesh.BoneWeights.Length; i++ )
                            subMesh.BoneWeights[ i ] = BoneWeight.Empty;
                    }

                    indexTable.BoneIndices = new ushort[ aiMesh.Bones.Count ];
                    for ( int i = 0; i < aiMesh.Bones.Count; i++ )
                    {
                        var aiBone = aiMesh.Bones[ i ];

                        if ( !boneMap.TryGetValue( aiBone.Name, out int boneIndex ) )
                        {
                            boneIndex = bones.Count;
                            boneMap[ aiBone.Name ] = boneIndex;
                            bones.Add( ConvertBoneFromAiBone( aiBone, aiScene, boneIndex ) );
                        }

                        indexTable.BoneIndices[ i ] = ( ushort )boneIndex;

                        foreach ( var aiWeight in aiBone.VertexWeights )
                            subMesh.BoneWeights[ vertexOffset + aiWeight.VertexID ].AddWeight( i, aiWeight.Weight );
                    }
                }

                indexTable.Indices = aiMesh.Faces.Where( x => x.IndexCount == 3 ).SelectMany( x => x.Indices ).Select( x => ( ushort )( vertexOffset + x ) ).ToArray();

                ushort[] triangleStrip = Stripifier.Stripify( indexTable.Indices );
                if ( triangleStrip != null )
                {
                    indexTable.PrimitiveType = PrimitiveType.TriangleStrip;
                    indexTable.Indices = triangleStrip;
                }

                var aiMaterial = aiScene.Materials[ aiMesh.MaterialIndex ];
                if ( !materialMap.TryGetValue( aiMaterial.Name, out int materialIndex ) )
                {
                    materialIndex = materials.Count;
                    materialMap[ aiMaterial.Name ] = materialIndex;
                    materials.Add( ConvertMaterialFromAiMaterial( aiMaterial, texturesDirectory, textureSet ) );
                }

                indexTable.MaterialIndex = materialIndex;

                var axisAlignedBoundingBox = new AxisAlignedBoundingBox( subMesh.Vertices.Skip( vertexOffset ).Take( aiMesh.Vertices.Count ) );

                indexTable.BoundingSphere = axisAlignedBoundingBox.ToBoundingSphere();
                indexTable.BoundingBox = axisAlignedBoundingBox.ToBoundingBox();

                subMesh.IndexTables.Add( indexTable );

                vertexOffset += aiMesh.VertexCount;
            }

            subMesh.BoundingSphere = new AxisAlignedBoundingBox( subMesh.Vertices ).ToBoundingSphere();

            return subMesh;
        }

        private static void ConvertSubMeshesFromAiNodesRecursively( Mesh mesh, Ai.Node aiNode, Ai.Scene aiScene, Matrix4x4 parentTransformation, Dictionary<string, int> boneMap, Dictionary<string, int> materialMap, string texturesDirectory, TextureSet textureSet )
        {
            var subMesh = ConvertSubMeshFromAiNode( aiNode, aiScene, parentTransformation, boneMap, mesh.Skin.Bones, materialMap, mesh.Materials, texturesDirectory, textureSet );
            if ( subMesh != null )
                mesh.SubMeshes.Add( subMesh );

            var transformation = parentTransformation * aiNode.Transform.ToNumericsTransposed();
            foreach ( var aiChildNode in aiNode.Children )
                ConvertSubMeshesFromAiNodesRecursively( mesh, aiChildNode, aiScene, transformation, boneMap, materialMap, texturesDirectory, textureSet );
        }

        private static Mesh ConvertMeshFromAiNode( Ai.Node aiNode, Ai.Scene aiScene, Matrix4x4 parentTransformation, string texturesDirectory, TextureSet textureSet )
        {
            var mesh = new Mesh
            {
                Name = aiNode.Name,
                Skin = new Skin()
            };

            var boneMap = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );
            var materialMap = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );

            ConvertSubMeshesFromAiNodesRecursively( mesh, aiNode, aiScene, parentTransformation, boneMap, materialMap, texturesDirectory, textureSet );

            if ( mesh.Skin.Bones.Count == 0 )
                mesh.Skin = null;

            mesh.BoundingSphere = new AxisAlignedBoundingBox( mesh.SubMeshes.SelectMany( x => x.Vertices ) ).ToBoundingSphere();

            return mesh.SubMeshes.Count != 0 ? mesh : null;
        }
    }
}
