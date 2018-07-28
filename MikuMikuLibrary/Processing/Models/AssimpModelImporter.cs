using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using MikuMikuLibrary.Processing.Materials;
using MikuMikuLibrary.Processing.Textures;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Ai = Assimp;

namespace MikuMikuLibrary.Processing.Models
{
    public static class AssimpModelImporter
    {
        public static Model ConvertModelFromAiScene( string fileName, TextureDatabase textureDatabase = null )
        {
            return ConvertModelFromAiScene( AssimpSceneUtilities.Import( fileName ), textureDatabase );
        }

        public static Model ConvertModelFromAiScene( Ai.Scene aiScene, TextureDatabase textureDatabase = null )
        {
            var model = new Model();

            ConvertMeshesFromAiNodesRecursively(
                aiScene.RootNode, aiScene, model.Meshes, model.TextureSet );

            model.TextureIDs.AddRange( model.TextureSet.Textures.Select( x => x.ID ) );

            if ( textureDatabase != null )
                ReAssingTextureIDsToModel( model, textureDatabase );

            return model;
        }

        public static Model ConvertModelWithSingleMeshFromAiScene( Ai.Scene aiScene, TextureDatabase textureDatabase = null )
        {
            throw new NotImplementedException( "TODO" );
        }

        private static void ReAssingTextureIDsToModel( Model model, TextureDatabase textureDatabase )
        {
            for ( int i = 0; i < model.TextureSet.Textures.Count; i++ )
            {
                var texture = model.TextureSet.Textures[ i ];
                var textureEntry = textureDatabase.Textures.FirstOrDefault( x =>
                     x.Name.Equals( texture.Name, StringComparison.OrdinalIgnoreCase ) );

                if ( textureEntry == null )
                {
                    textureEntry = new TextureEntry();
                    textureEntry.ID = textureDatabase.Textures.Max( x => x.ID ) + 1;
                    textureEntry.Name = texture.Name.ToUpperInvariant();
                    textureDatabase.Textures.Add( textureEntry );
                }

                // Fix IDs within materials
                foreach ( var mesh in model.Meshes )
                {
                    foreach ( var material in mesh.Materials )
                    {
                        if ( material.Diffuse.TextureID == texture.ID )
                            material.Diffuse.TextureID = textureEntry.ID;

                        if ( material.Ambient.TextureID == texture.ID )
                            material.Ambient.TextureID = textureEntry.ID;

                        if ( material.Normal.TextureID == texture.ID )
                            material.Normal.TextureID = textureEntry.ID;

                        if ( material.Specular.TextureID == texture.ID )
                            material.Specular.TextureID = textureEntry.ID;

                        if ( material.ToonCurve.TextureID == texture.ID )
                            material.ToonCurve.TextureID = textureEntry.ID;

                        if ( material.Reflection.TextureID == texture.ID )
                            material.Reflection.TextureID = textureEntry.ID;

                        if ( material.SpecularPower.TextureID == texture.ID )
                            material.SpecularPower.TextureID = textureEntry.ID;
                    }
                }

                texture.ID = textureEntry.ID;
                model.TextureIDs[ i ] = textureEntry.ID;
            }
        }

        private static Texture ConvertTexture( string fileName, TextureSet textureSet )
        {
            fileName = Path.ChangeExtension( fileName, "dds" );

            if ( !File.Exists( fileName ) )
                return null;

            Texture texture;
            var textureName = Path.GetFileNameWithoutExtension( fileName );

            texture = textureSet.Textures.FirstOrDefault( x =>
                x.Name.Equals( textureName, StringComparison.OrdinalIgnoreCase ) );

            if ( texture != null )
                return texture;

            texture = TextureEncoder.Encode( fileName );
            texture.ID = textureSet.Textures.Count;
            texture.Name = Path.GetFileNameWithoutExtension( fileName );

            textureSet.Textures.Add( texture );
            return texture;
        }

        private static Material ConvertMaterialFromAiMaterial( Ai.Material aiMaterial, TextureSet textureSet )
        {
            var tagList = TagList.Parse( aiMaterial.Name );

            Texture diffuse = null;
            Texture ambient = null;
            Texture normal = null;
            Texture specular = null;
            Texture toonCurve = null;
            Texture reflection = null;
            Texture specularPower = null;

            foreach ( var textureSlot in aiMaterial.GetAllMaterialTextures() )
            {
                if ( textureSlot.TextureType == Ai.TextureType.Diffuse && diffuse == null )
                    diffuse = ConvertTexture( textureSlot.FilePath, textureSet );

                else if ( textureSlot.TextureType == Ai.TextureType.Ambient && ambient == null )
                    ambient = ConvertTexture( textureSlot.FilePath, textureSet );

                else if ( textureSlot.TextureType == Ai.TextureType.Normals && normal == null )
                    normal = ConvertTexture( textureSlot.FilePath, textureSet );

                else if ( textureSlot.TextureType == Ai.TextureType.Specular && specular == null )
                    specular = ConvertTexture( textureSlot.FilePath, textureSet );

                else if ( textureSlot.TextureType == Ai.TextureType.Opacity && toonCurve == null )
                    toonCurve = ConvertTexture( textureSlot.FilePath, textureSet );

                else if ( textureSlot.TextureType == Ai.TextureType.Reflection && reflection == null )
                    reflection = ConvertTexture( textureSlot.FilePath, textureSet );

                else if ( textureSlot.TextureType == Ai.TextureType.Shininess && specularPower == null )
                    specularPower = ConvertTexture( textureSlot.FilePath, textureSet );
            }

            var preset = tagList.GetValue( "PRST", 0, "PHONGF" );

            Material material;

            switch ( preset.ToLowerInvariant() )
            {
                case "skinf":
                case "bodyf":
                    material = MaterialCreator.CreateSkinMaterialF( diffuse, ambient, specular, toonCurve );
                    break;

                case "clothf":
                    material = MaterialCreator.CreateClothMaterialF( diffuse, ambient, specular, toonCurve );
                    break;

                case "hairf":
                    material = MaterialCreator.CreateHairMaterialF( diffuse, ambient, specular, toonCurve );
                    break;

                case "phongf":
                default:
                    material = MaterialCreator.CreatePhongMaterialF( diffuse );
                    break;
            }

            material.Name = tagList.Name;
            return material;
        }

        private static Matrix4x4 GetMatrix4x4FromAiMatrix4x4( Ai.Matrix4x4 m )
        {
            return new Matrix4x4(
                m.A1, m.A2, m.A3, m.A4,
                m.B1, m.B2, m.B3, m.B4,
                m.C1, m.C2, m.C3, m.C4,
                m.D1, m.D2, m.D3, m.D4 );
        }

        private static Bone ConvertBoneFromAiBone( Ai.Bone aiBone, Ai.Scene aiScene, int defaultID = 0 )
        {
            var bone = new Bone();

            var tagList = TagList.Parse( aiBone.Name );
            bone.Name = tagList.Name;
            bone.ID = tagList.GetValue( "ID", 0, defaultID );

            Matrix4x4.Invert(
                GetMatrix4x4FromAiMatrix4x4(
                    GetWorldTransformFromAiNode( aiScene.RootNode.FindNode( aiBone.Name ) ) ),
                out Matrix4x4 inverseTransform );

            bone.Matrix = inverseTransform;

            return bone;
        }

        private static Vector2 ClampUVCoordinates( Vector2 v )
        {
            return new Vector2(
                v.X > 1f ? ( v.X - ( int )v.X ) : v.X < 0f ? 1f + ( v.X - ( int )v.X ) : v.X,
                v.Y > 1f ? ( v.Y - ( int )v.Y ) : v.Y < 0f ? 1f + ( v.Y - ( int )v.Y ) : v.Y );
        }

        private static SubMesh ConvertSubmeshFromAiMesh( Ai.Mesh aiMesh, Ai.Scene aiScene, Matrix4x4 transform, List<Bone> bones, List<Material> materials, Dictionary<string, ushort> boneMap, Dictionary<string, int> materialMap, TextureSet textureSet )
        {
            var subMesh = new SubMesh();
            subMesh.Name = aiMesh.Name;

            if ( aiMesh.HasVertices )
                subMesh.Vertices = aiMesh.Vertices.Select( x =>
                    new Vector3( x.X, x.Y, x.Z ) ).ToArray();

            if ( aiMesh.HasNormals )
                subMesh.Normals = aiMesh.Normals.Select( x =>
                    new Vector3( x.X, x.Y, x.Z ) ).ToArray();

            if ( aiMesh.HasTextureCoords( 0 ) )
                subMesh.UVChannel1 = aiMesh.TextureCoordinateChannels[ 0 ].Select( x =>
                  ClampUVCoordinates( new Vector2( x.X, x.Y ) ) ).ToArray();

            if ( aiMesh.HasTextureCoords( 1 ) )
                subMesh.UVChannel2 = aiMesh.TextureCoordinateChannels[ 1 ].Select( x =>
                  ClampUVCoordinates( new Vector2( x.X, x.Y ) ) ).ToArray();

            //if ( aiMesh.HasVertexColors( 0 ) )
            //    submesh.Colors = aiMesh.VertexColorChannels[ 0 ].Select( x =>
            //      new Color( x.R, x.G, x.B, x.A ) ).ToArray();

            var indexTable = new IndexTable();

            if ( aiMesh.HasBones )
            {
                subMesh.BoneWeights = new BoneWeight[ aiMesh.VertexCount ];
                for ( int i = 0; i < subMesh.BoneWeights.Length; i++ )
                {
                    subMesh.BoneWeights[ i ].Index1 =
                        subMesh.BoneWeights[ i ].Index2 =
                        subMesh.BoneWeights[ i ].Index3 =
                        subMesh.BoneWeights[ i ].Index4 = -1;
                }

                var boneIndices = new List<ushort>();
                foreach ( var aiBone in aiMesh.Bones )
                {
                    if ( !boneMap.ContainsKey( aiBone.Name ) )
                    {
                        boneMap.Add( aiBone.Name, ( ushort )bones.Count );
                        bones.Add( ConvertBoneFromAiBone( aiBone, aiScene, bones.Count ) );
                    }

                    ushort boneIndex = boneMap[ aiBone.Name ];
                    if ( !boneIndices.Contains( boneIndex ) )
                        boneIndices.Add( boneIndex );

                    short boneIndexForVertex = ( short )boneIndices.IndexOf( boneIndex );
                    foreach ( var weight in aiBone.VertexWeights )
                    {
                        if ( subMesh.BoneWeights[ weight.VertexID ].Index1 < 0 )
                        {
                            subMesh.BoneWeights[ weight.VertexID ].Index1 = boneIndexForVertex;
                            subMesh.BoneWeights[ weight.VertexID ].Weight1 = weight.Weight;
                        }
                        else if ( subMesh.BoneWeights[ weight.VertexID ].Index2 < 0 )
                        {
                            subMesh.BoneWeights[ weight.VertexID ].Index2 = boneIndexForVertex;
                            subMesh.BoneWeights[ weight.VertexID ].Weight2 = weight.Weight;
                        }
                        else if ( subMesh.BoneWeights[ weight.VertexID ].Index3 < 0 )
                        {
                            subMesh.BoneWeights[ weight.VertexID ].Index3 = boneIndexForVertex;
                            subMesh.BoneWeights[ weight.VertexID ].Weight3 = weight.Weight;
                        }
                        else if ( subMesh.BoneWeights[ weight.VertexID ].Index4 < 0 )
                        {
                            subMesh.BoneWeights[ weight.VertexID ].Index4 = boneIndexForVertex;
                            subMesh.BoneWeights[ weight.VertexID ].Weight4 = weight.Weight;
                        }
                    }
                }
                indexTable.BoneIndices = boneIndices.ToArray();
            }

            if ( aiMesh.HasFaces )
            {
                indexTable.Indices = new ushort[ aiMesh.FaceCount * 3 ];
                for ( int i = 0; i < aiMesh.FaceCount; i++ )
                {
                    indexTable.Indices[ ( i * 3 ) ] = ( ushort )aiMesh.Faces[ i ].Indices[ 0 ];
                    indexTable.Indices[ ( i * 3 ) + 1 ] = ( ushort )aiMesh.Faces[ i ].Indices[ 1 ];
                    indexTable.Indices[ ( i * 3 ) + 2 ] = ( ushort )aiMesh.Faces[ i ].Indices[ 2 ];
                }

                var triangleStrip = NvTriStripUtilities.Generate( indexTable.Indices );
                if ( triangleStrip != null )
                {
                    indexTable.Type = IndexTableType.TriangleStrip;
                    indexTable.Indices = triangleStrip;
                }
            }

            var aiMaterial = aiScene.Materials[ aiMesh.MaterialIndex ];
            if ( !materialMap.ContainsKey( aiMaterial.Name ) )
            {
                materialMap.Add( aiMaterial.Name, materials.Count );
                materials.Add( ConvertMaterialFromAiMaterial( aiMaterial, textureSet ) );
            }

            indexTable.MaterialIndex = materialMap[ aiMaterial.Name ];

            var subMeshBoundingBox = BoundingBox.FromPoints( subMesh.Vertices );
            subMesh.BoundingSphere = BoundingSphere.FromBoundingBox( subMeshBoundingBox );
            indexTable.BoundingSphere = subMesh.BoundingSphere;

            subMesh.IndexTables.Add( indexTable );
            return subMesh;
        }

        private static Ai.Matrix4x4 GetWorldTransformFromAiNode( Ai.Node aiNode )
        {
            var transform = aiNode.Transform;
            var parent = aiNode.Parent;
            while ( parent != null )
            {
                transform *= parent.Transform;
                parent = parent.Parent;
            }

            return transform;
        }

        private static Mesh ConvertMeshFromAiNode( Ai.Node aiNode, Ai.Scene aiScene, TextureSet textureSet )
        {
            if ( aiNode.HasMeshes )
            {
                var transform = GetMatrix4x4FromAiMatrix4x4(
                    GetWorldTransformFromAiNode( aiNode ) );

                var mesh = new Mesh();

                var tagList = TagList.Parse( aiNode.Name );
                mesh.Name = tagList.Name;
                mesh.ID = tagList.GetValue<int>( "ID" );

                var meshBoundingBox = new BoundingBox();
                var boneMap = new Dictionary<string, ushort>();
                var materialMap = new Dictionary<string, int>();

                foreach ( var meshIndex in aiNode.MeshIndices )
                {
                    var aiMesh = aiScene.Meshes[ meshIndex ];

                    var submesh = ConvertSubmeshFromAiMesh(
                        aiMesh, aiScene, transform, mesh.Bones, mesh.Materials, boneMap, materialMap, textureSet );

                    foreach ( var vertex in submesh.Vertices )
                        meshBoundingBox.AddPoint( vertex );

                    if ( submesh.Name == aiNode.Name )
                        submesh.Name = $"{mesh.Name}{mesh.SubMeshes.Count}";

                    mesh.SubMeshes.Add( submesh );
                }

                mesh.BoundingSphere = BoundingSphere.FromBoundingBox( meshBoundingBox );

                return mesh;
            }

            return null;
        }

        private static void ConvertMeshesFromAiNodesRecursively( Ai.Node aiNode, Ai.Scene aiScene, List<Mesh> meshes, TextureSet textureSet )
        {
            var mesh = ConvertMeshFromAiNode( aiNode, aiScene, textureSet );

            if ( mesh != null )
                meshes.Add( mesh );

            foreach ( var aiChildNode in aiNode.Children )
                ConvertMeshesFromAiNodesRecursively( aiChildNode, aiScene, meshes, textureSet );
        }
    }
}
