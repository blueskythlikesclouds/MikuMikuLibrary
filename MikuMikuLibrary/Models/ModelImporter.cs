using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Maths;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Ai = Assimp;

namespace MikuMikuLibrary.Models
{
    public static class ModelImporter
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

            var transformation = GetMatrix4x4FromAiMatrix4x4( aiScene.RootNode.Transform );
            foreach ( var aiNode in aiScene.RootNode.Children )
            {
                var mesh = ConvertMeshFromAiNode( aiNode, aiScene, transformation, texturesDirectory, model.TextureSet );

                if ( mesh != null )
                    model.Meshes.Add( mesh );
            }

            model.TextureIDs.AddRange( model.TextureSet.Textures.Select( x => x.ID ) );
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

            model.TextureIDs.AddRange( model.TextureSet.Textures.Select( x => x.ID ) );

            return model;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static Matrix4x4 GetMatrix4x4FromAiMatrix4x4( Ai.Matrix4x4 matrix, bool transpose = true )
        {
            if ( transpose )
            {
                return new Matrix4x4( matrix.A1, matrix.B1, matrix.C1, matrix.D1,
                                      matrix.A2, matrix.B2, matrix.C2, matrix.D2,
                                      matrix.A3, matrix.B3, matrix.C3, matrix.D3,
                                      matrix.A4, matrix.B4, matrix.C4, matrix.D4 );
            }

            else
            {
                return new Matrix4x4( matrix.A1, matrix.A2, matrix.A3, matrix.A4,
                                      matrix.B1, matrix.B2, matrix.B3, matrix.B4,
                                      matrix.C1, matrix.C2, matrix.C3, matrix.C4,
                                      matrix.D1, matrix.D2, matrix.D3, matrix.D4 );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static Matrix4x4 GetWorldTransformation( Ai.Node aiNode, bool transpose = true )
        {
            var transform = GetMatrix4x4FromAiMatrix4x4( aiNode.Transform, transpose );
            var parent = aiNode.Parent;

            while ( parent != null )
            {
                var parentTransform = GetMatrix4x4FromAiMatrix4x4( parent.Transform, transpose );
                transform = parentTransform * transform;
                parent = parent.Parent;
            }

            return transform;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static Vector2 ClampTextureCoordinates( Vector2 uv )
        {
            return new Vector2(
                uv.X > 1f ? ( uv.X - ( int )uv.X ) : uv.X < 0f ? 1f + ( uv.X - ( int )uv.X ) : uv.X,
                uv.Y > 1f ? ( uv.Y - ( int )uv.Y ) : uv.Y < 0f ? 1f + ( uv.Y - ( int )uv.Y ) : uv.Y );
        }

        private static Bone ConvertBoneFromAiBone( Ai.Bone aiBone, Ai.Scene aiScene, int boneID )
        {
            Matrix4x4 inverseTransformation;

            var aiBoneNode = aiScene.RootNode.FindNode( aiBone.Name );
            if ( aiBoneNode != null )
                Matrix4x4.Invert( GetWorldTransformation( aiBoneNode, false ), out inverseTransformation );
            else
                inverseTransformation = GetMatrix4x4FromAiMatrix4x4( aiBone.OffsetMatrix, false );

            return new Bone
            {
                Name = aiBone.Name,
                ID = boneID,
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
            texture.ID = sRandom.Next( 800000, int.MaxValue );
            textureSet.Textures.Add( texture );

            return texture;
        }

        private static Texture ConvertTexture( string textureName, string texturesDirectory, TextureSet textureSet )
        {
            return ConvertTexture( Path.Combine( texturesDirectory, textureName ), textureSet );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static Color GetColorFromAiColor4D( Ai.Color4D color )
        {
            return new Color( color.R, color.G, color.B, color.A );
        }

        private static Material ConvertMaterialFromAiMaterial( Ai.Material aiMaterial, string texturesDirectory, TextureSet textureSet )
        {
            Material material = new Material();

            material.Shader = "BLINN";
            material.Field02 = 2688;
            material.Name = aiMaterial.Name;
            material.Field25 = 1;

            if ( aiMaterial.HasColorDiffuse )
                material.DiffuseColor = GetColorFromAiColor4D( aiMaterial.ColorDiffuse );
            else
                material.DiffuseColor = new Color( 1, 1, 1, 1 );

            if ( aiMaterial.HasColorAmbient )
                material.AmbientColor = GetColorFromAiColor4D( aiMaterial.ColorAmbient );
            else
                material.AmbientColor = new Color( 1, 1, 1, 1 );

            if ( aiMaterial.HasColorSpecular )
                material.SpecularColor = GetColorFromAiColor4D( aiMaterial.ColorSpecular );
            else
                material.SpecularColor = new Color( 0.5f, 0.5f, 0.5f, 1 );

            if ( aiMaterial.HasColorEmissive )
                material.EmissionColor = GetColorFromAiColor4D( aiMaterial.ColorEmissive );
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
                material.Diffuse.TextureID = texture.ID;
                material.Diffuse.Field02 = 241;
            }

            if ( aiMaterial.HasTextureAmbient && ( texture = ConvertTexture( aiMaterial.TextureAmbient.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Ambient.TextureID = texture.ID;
                material.Ambient.Field02 = 241;
            }

            if ( aiMaterial.HasTextureNormal && ( texture = ConvertTexture( aiMaterial.TextureNormal.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 256;
                material.Normal.TextureID = texture.ID;
                material.Normal.Field02 = 242;
            }

            if ( aiMaterial.HasTextureSpecular && ( texture = ConvertTexture( aiMaterial.TextureSpecular.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 128;
                material.Specular.TextureID = texture.ID;
                material.Specular.Field02 = 243;
            }

            if ( aiMaterial.HasTextureReflection && ( texture = ConvertTexture( aiMaterial.TextureReflection.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 33832;
                material.Reflection.TextureID = texture.ID;
                material.Reflection.Field02 = 1017;
            }

            if ( aiMaterial.GetMaterialTexture( Ai.TextureType.Shininess, 0, out Ai.TextureSlot shininess ) && ( texture = ConvertTexture( shininess.FilePath, texturesDirectory, textureSet ) ) != null )
            {
                material.Field00 |= 8192;
                material.SpecularPower.TextureID = texture.ID;
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

            var transformation = parentTransformation * GetMatrix4x4FromAiMatrix4x4( aiNode.Transform );
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
                    subMesh.Vertices[ vertexOffset + i ] = Vector3.Transform( new Vector3( aiMesh.Vertices[ i ].X, aiMesh.Vertices[ i ].Y, aiMesh.Vertices[ i ].Z ), transformation );

                if ( aiMesh.HasNormals )
                {
                    if ( subMesh.Normals == null )
                        subMesh.Normals = new Vector3[ vertexCount ];

                    for ( int i = 0; i < aiMesh.Normals.Count; i++ )
                        subMesh.Normals[ vertexOffset + i ] = Vector3.Normalize( Vector3.TransformNormal( new Vector3( aiMesh.Normals[ i ].X, aiMesh.Normals[ i ].Y, aiMesh.Normals[ i ].Z ), transformation ) );
                }

                if ( aiMesh.HasTangentBasis )
                {
                    if ( subMesh.Tangents == null )
                        subMesh.Tangents = new Vector4[ vertexCount ];

                    for ( int i = 0; i < aiMesh.Tangents.Count; i++ )
                    {
                        Vector3 tangent = Vector3.TransformNormal( new Vector3( aiMesh.Tangents[ i ].X, aiMesh.Tangents[ i ].Y, aiMesh.Tangents[ i ].Z ), transformation );
                        Vector3 bitangent = Vector3.TransformNormal( new Vector3( aiMesh.BiTangents[ i ].X, aiMesh.BiTangents[ i ].Y, aiMesh.BiTangents[ i ].Z ), transformation );
                        float direction = Vector3.Dot( bitangent, Vector3.Cross( subMesh.Normals[ vertexOffset + i ], tangent ) ) > 0 ? 1.0f : -1.0f;

                        subMesh.Tangents[ vertexOffset + i ] = new Vector4( tangent, direction );
                    }
                }

                if ( aiMesh.HasTextureCoords( 0 ) )
                {
                    if ( subMesh.UVChannel1 == null )
                        subMesh.UVChannel1 = new Vector2[ vertexCount ];

                    for ( int i = 0; i < aiMesh.TextureCoordinateChannels[ 0 ].Count; i++ )
                        subMesh.UVChannel1[ vertexOffset + i ] = ClampTextureCoordinates( new Vector2( aiMesh.TextureCoordinateChannels[ 0 ][ i ].X, 1f - aiMesh.TextureCoordinateChannels[ 0 ][ i ].Y ) );
                }

                if ( aiMesh.HasTextureCoords( 1 ) )
                {
                    if ( subMesh.UVChannel2 == null )
                        subMesh.UVChannel2 = new Vector2[ vertexCount ];

                    for ( int i = 0; i < aiMesh.TextureCoordinateChannels[ 1 ].Count; i++ )
                        subMesh.UVChannel2[ vertexOffset + i ] = ClampTextureCoordinates( new Vector2( aiMesh.TextureCoordinateChannels[ 1 ][ i ].X, 1f - aiMesh.TextureCoordinateChannels[ 1 ][ i ].Y ) );
                }

                if ( aiMesh.HasVertexColors( 0 ) )
                {
                    if ( subMesh.Colors == null )
                        subMesh.Colors = Enumerable.Repeat( Color.White, vertexCount ).ToArray();

                    for ( int i = 0; i < aiMesh.VertexColorChannels[ 0 ].Count; i++ )
                        subMesh.Colors[ vertexOffset + i ] = new Color( aiMesh.VertexColorChannels[ 0 ][ i ].R, aiMesh.VertexColorChannels[ 0 ][ i ].G, aiMesh.VertexColorChannels[ 0 ][ i ].B, aiMesh.VertexColorChannels[ 0 ][ i ].A );
                }

                var indexTable = new IndexTable();

                if ( aiMesh.HasBones )
                {
                    if ( subMesh.BoneWeights == null )
                        subMesh.BoneWeights = Enumerable.Repeat( BoneWeight.Empty, vertexCount ).ToArray();

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

                ushort[] triangleStrip = TriangleStripUtilities.GenerateStrips( indexTable.Indices );
                if ( triangleStrip != null )
                {
                    indexTable.PrimitiveType = IndexTablePrimitiveType.TriangleStrip;
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

            var transformation = parentTransformation * GetMatrix4x4FromAiMatrix4x4( aiNode.Transform );
            foreach ( var aiChildNode in aiNode.Children )
                ConvertSubMeshesFromAiNodesRecursively( mesh, aiChildNode, aiScene, transformation, boneMap, materialMap, texturesDirectory, textureSet );
        }

        private static Mesh ConvertMeshFromAiNode( Ai.Node aiNode, Ai.Scene aiScene, Matrix4x4 parentTransformation, string texturesDirectory, TextureSet textureSet )
        {
            var mesh = new Mesh
            {
                Name = aiNode.Name,
                Skin = new MeshSkin()
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
