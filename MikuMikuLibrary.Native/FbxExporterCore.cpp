#include "FbxExporterCore.h"
#include "Utf8String.h"

using namespace Collections::Generic;
using namespace IO;
using namespace Numerics;
using namespace Runtime::InteropServices;

namespace MikuMikuLibrary::Objects::Processing::Fbx
{
    using namespace Extensions;
    using namespace Materials;
    using namespace Misc;
    using namespace Textures;
    using namespace Textures::Processing;

    FbxAMatrix CreateFbxAMatrixFromNumerics( Matrix4x4 matrix )
    {
        FbxAMatrix lMatrix;

        lMatrix.mData[ 0 ][ 0 ] = matrix.M11; lMatrix.mData[ 0 ][ 1 ] = matrix.M12; lMatrix.mData[ 0 ][ 2 ] = matrix.M13; lMatrix.mData[ 0 ][ 3 ] = matrix.M14;
        lMatrix.mData[ 1 ][ 0 ] = matrix.M21; lMatrix.mData[ 1 ][ 1 ] = matrix.M22; lMatrix.mData[ 1 ][ 2 ] = matrix.M23; lMatrix.mData[ 1 ][ 3 ] = matrix.M24;
        lMatrix.mData[ 2 ][ 0 ] = matrix.M31; lMatrix.mData[ 2 ][ 1 ] = matrix.M32; lMatrix.mData[ 2 ][ 2 ] = matrix.M33; lMatrix.mData[ 2 ][ 3 ] = matrix.M34;
        lMatrix.mData[ 3 ][ 0 ] = matrix.M41; lMatrix.mData[ 3 ][ 1 ] = matrix.M42; lMatrix.mData[ 3 ][ 2 ] = matrix.M43; lMatrix.mData[ 3 ][ 3 ] = matrix.M44;

        return lMatrix;
    }

    FbxNode* CreateFbxNodeFromMesh( Mesh^ mesh, Object^ object, FbxScene* lScene, List<IntPtr>^ materials, Dictionary<String^, IntPtr>^ convertedBones )
    {
        FbxNode* lNode = FbxNode::Create( lScene, Utf8String( mesh->Name ).ToCStr() );
        FbxMesh* lMesh = FbxMesh::Create( lScene, Utf8String( mesh->Name + "_mesh" ).ToCStr() );

        lMesh->InitControlPoints( mesh->Positions->Length );

        for ( int i = 0; i < mesh->Positions->Length; i++ )
        {
            Vector3 position = mesh->Positions[ i ];
            lMesh->SetControlPointAt( FbxVector4( position.X, position.Y, position.Z ), i );
        }

        if ( mesh->Normals != nullptr )
        {
            FbxGeometryElementNormal* lElementNormal = lMesh->CreateElementNormal();
            lElementNormal->SetMappingMode( FbxLayerElement::eByControlPoint );
            lElementNormal->SetReferenceMode( FbxLayerElement::eDirect );

            for each ( Vector3 normal in mesh->Normals )
                lElementNormal->GetDirectArray().Add( FbxVector4( normal.X, normal.Y, normal.Z ) );
        }

        for ( int i = 0; i < 4; i++ )
        {
            array<Vector2>^ texCoords = mesh->GetTexCoordsChannel( i );

            if ( texCoords == nullptr )
                continue;

            FbxGeometryElementUV* lElementUV = lMesh->CreateElementUV( Utf8String( String::Format( "UVChannel_{0}", i ) ).ToCStr() );
            lElementUV->SetMappingMode( FbxLayerElement::eByControlPoint );
            lElementUV->SetReferenceMode( FbxLayerElement::eDirect );

            for each ( Vector2 texCoord in texCoords )
                lElementUV->GetDirectArray().Add( FbxVector2( texCoord.X, 1 - texCoord.Y ) );
        }

        for ( int i = 0; i < 2; i++ )
        {
            array<Color>^ colors = mesh->GetColorsChannel( i );

            if ( colors == nullptr )
                continue;

            FbxGeometryElementVertexColor* lElementVertexColor = lMesh->CreateElementVertexColor();
            lElementVertexColor->SetMappingMode( FbxLayerElement::eByControlPoint );
            lElementVertexColor->SetReferenceMode( FbxLayerElement::eDirect );

            for each ( Color color in colors )
                lElementVertexColor->GetDirectArray().Add( FbxColor( color.R, color.G, color.B, color.A ) );
        }

        FbxGeometryElementMaterial* lElementMaterial = lMesh->CreateElementMaterial();
        lElementMaterial->SetMappingMode( FbxLayerElement::eByPolygon );
        lElementMaterial->SetReferenceMode( FbxLayerElement::eIndexToDirect );

        HashSet<unsigned int>^ vertexIndices = gcnew HashSet<unsigned int>( mesh->Positions->Length );
        Dictionary<int, int>^ materialMap = gcnew Dictionary<int, int>( materials->Count );

        FbxSkin* lSkin = nullptr;
        Dictionary<int, IntPtr>^ clusterMap = nullptr;

        if ( object->Skin != nullptr )
        {
            lSkin = FbxSkin::Create( lScene, Utf8String( mesh->Name + "_skin" ).ToCStr() );
            clusterMap = gcnew Dictionary<int, IntPtr>( object->Skin->Bones->Count );
        }

        for ( int i = 0; i < mesh->SubMeshes->Count; i++ )
        {
            SubMesh^ subMesh = mesh->SubMeshes[ i ];
            List<Triangle>^ triangles = subMesh->GetTriangles();

            int materialIndex;

            if ( !materialMap->TryGetValue( subMesh->MaterialIndex, materialIndex ) )
            {
                materialIndex = lNode->AddMaterial( ( FbxSurfacePhong* ) materials[ subMesh->MaterialIndex ].ToPointer() );
                materialMap->Add( subMesh->MaterialIndex, materialIndex );
            }

            for each ( Triangle triangle in triangles )
            {
                vertexIndices->Add( triangle.A );
                vertexIndices->Add( triangle.B );
                vertexIndices->Add( triangle.C );

                lMesh->BeginPolygon( materialIndex, -1, -1, false );
                lMesh->AddPolygon( triangle.A );
                lMesh->AddPolygon( triangle.B );
                lMesh->AddPolygon( triangle.C );
                lMesh->EndPolygon();
            }

            if ( lSkin != nullptr && subMesh->BoneIndices != nullptr )
            {
                for ( int j = 0; j < subMesh->BoneIndices->Length; j++ )
                {
                    ushort boneIndex = subMesh->BoneIndices[ j ];
                    BoneInfo^ boneInfo = object->Skin->Bones[ boneIndex ];
                    FbxNode* lBoneNode = ( FbxNode* ) convertedBones[ boneInfo->Name ].ToPointer();

                    IntPtr clusterPtr;
                    FbxCluster* lCluster;

                    if ( !clusterMap->TryGetValue( boneIndex, clusterPtr ) )
                    {
                        lCluster = FbxCluster::Create( lScene, Utf8String( String::Format( "{0}_{1}_{2}_cluster", mesh->Name, i, boneInfo->Name ) ).ToCStr() );
                        lCluster->SetLink( lBoneNode );
                        lCluster->SetLinkMode( FbxCluster::eTotalOne );

                        Matrix4x4 worldTransformation;
                        Matrix4x4::Invert( boneInfo->InverseBindPoseMatrix, worldTransformation );

                        lCluster->SetTransformLinkMatrix( CreateFbxAMatrixFromNumerics( worldTransformation ) );

                        clusterMap->Add( boneIndex, IntPtr( lCluster ) );
                    }
                    else
                    {
                        lCluster = ( FbxCluster* ) clusterPtr.ToPointer();
                    }

                    for each ( unsigned int index in vertexIndices )
                    {
                        BoneWeight boneWeight = mesh->BoneWeights[ index ];

                        if ( boneWeight.Index1 == j )
                            lCluster->AddControlPointIndex( index, boneWeight.Weight1 );

                        if ( boneWeight.Index2 == j )
                            lCluster->AddControlPointIndex( index, boneWeight.Weight2 );

                        if ( boneWeight.Index3 == j )
                            lCluster->AddControlPointIndex( index, boneWeight.Weight3 );

                        if ( boneWeight.Index4 == j )
                            lCluster->AddControlPointIndex( index, boneWeight.Weight4 );
                    }

                    lSkin->AddCluster( lCluster );
                }
            }

            vertexIndices->Clear();
        }

        if ( lSkin != nullptr )
        {
            if ( lSkin->GetClusterCount() > 0 )
                lMesh->AddDeformer( lSkin );

            else
                lSkin->Destroy();
        }

        lNode->SetNodeAttribute( lMesh );
        lNode->SetShadingMode( FbxNode::eTextureShading );

        return lNode;
    }

    FbxSurfacePhong* CreateFbxSurfacePhongFromMaterial( Material^ material, TextureSet^ textureSet, String^ texturesDirectoryPath, FbxScene* lScene )
    {
        FbxSurfacePhong* lSurfacePhong = FbxSurfacePhong::Create( lScene, Utf8String( material->Name ).ToCStr() );

        lSurfacePhong->ShadingModel.Set( "Phong" );

        lSurfacePhong->Diffuse.Set( FbxDouble3( material->Diffuse.R, material->Diffuse.G, material->Diffuse.B ) );
        lSurfacePhong->TransparencyFactor.Set( 1 - material->Diffuse.A );

        lSurfacePhong->Ambient.Set( FbxDouble3( material->Ambient.R, material->Ambient.G, material->Ambient.B ) );

        lSurfacePhong->Specular.Set( FbxDouble3( material->Specular.R, material->Specular.G, material->Specular.B ) );
        lSurfacePhong->ReflectionFactor.Set( material->Specular.A );

        lSurfacePhong->Emissive.Set( FbxDouble3( material->Emission.R, material->Emission.G, material->Emission.B ) );

        lSurfacePhong->Shininess.Set( material->Shininess );

        if ( textureSet == nullptr || textureSet->Textures->Count == 0 )
            return lSurfacePhong;

        HashSet<MaterialTextureType>^ exportedTypes = gcnew HashSet<MaterialTextureType>( 8 );

        for each ( MaterialTexture^ materialTexture in material->MaterialTextures )
        {
            if ( materialTexture->Type == MaterialTextureType::None || exportedTypes->Contains( materialTexture->Type ) )
                continue;

            Texture^ texture = nullptr;

            for each ( Texture^ item in textureSet->Textures )
            {
                if ( item->Id != materialTexture->TextureId )
                    continue;

                texture = item;
                break;
            }

            if ( texture == nullptr )
                continue;

            String^ extension = TextureFormatUtilities::IsBlockCompressed( texture->Format ) && !texture->IsYCbCr ? ".dds" : ".png";

            FbxFileTexture* lFileTexture = FbxFileTexture::Create( lScene,
                Utf8String( String::Format( "{0}_{1}", material->Name, Enum::GetName( materialTexture->Type.GetType(), materialTexture->Type ) ) ).ToCStr() );

            lFileTexture->SetFileName( Utf8String( Path::Combine( texturesDirectoryPath, texture->Name + extension ) ).ToCStr() );

            lFileTexture->UVSet.Set( "UVChannel_0" );

            if ( materialTexture->TextureCoordinateTranslationType == MaterialTextureCoordinateTranslationType::Sphere )
                lFileTexture->SetMappingType( FbxTexture::eSpherical );

            else if ( materialTexture->TextureCoordinateTranslationType == MaterialTextureCoordinateTranslationType::Cube )
                lFileTexture->SetMappingType( FbxTexture::eBox );

            else
                lFileTexture->SetMappingType( FbxTexture::eUV );

            switch ( materialTexture->Type )
            {
            case MaterialTextureType::Color:
                lSurfacePhong->Diffuse.ConnectSrcObject( lFileTexture );
                break;

            case MaterialTextureType::Normal:
                lFileTexture->SetTextureUse( FbxTexture::eBumpNormalMap );
                lSurfacePhong->Bump.ConnectSrcObject( lFileTexture );
                break;

            case MaterialTextureType::Specular:
                lSurfacePhong->Specular.ConnectSrcObject( lFileTexture );
                break;

            case MaterialTextureType::Reflection:
            case MaterialTextureType::EnvironmentCube:
            case MaterialTextureType::EnvironmentSphere:
                lSurfacePhong->Reflection.ConnectSrcObject( lFileTexture );
                break;

            case MaterialTextureType::Transparency:
                lSurfacePhong->TransparentColor.ConnectSrcObject( lFileTexture );
                break;

            default:
                lFileTexture->Destroy();
                break;
            }

            exportedTypes->Add( materialTexture->Type );
        }

        return lSurfacePhong;
    }

    FbxNode* CreateFbxNodeFromObject( Object^ object, TextureSet^ textureSet, String^ texturesDirectoryPath, FbxScene* lScene, FbxPose* lBindPose, Dictionary<String^, IntPtr>^ convertedBones )
    {
        FbxNode* lNode = FbxNode::Create( lScene, Utf8String( object->Name ).ToCStr() );

        List<IntPtr>^ materials = gcnew List<IntPtr>( object->Materials->Count );

        for each ( Material^ material in object->Materials )
            materials->Add( IntPtr( CreateFbxSurfacePhongFromMaterial( material, textureSet, texturesDirectoryPath, lScene ) ) );

        for each ( Mesh^ mesh in object->Meshes )
        {
            FbxNode* lMeshNode = CreateFbxNodeFromMesh( mesh, object, lScene, materials, convertedBones );
            lNode->AddChild( lMeshNode );
            lBindPose->Add( lMeshNode, FbxAMatrix() );
        }

        lBindPose->Add( lNode, FbxAMatrix() );

        return lNode;
    }

    FbxNode* CreateFbxNodeFromBoneInfo( BoneInfo^ boneInfo, const Matrix4x4& inverseParentTransformation, FbxScene* lScene, FbxPose* lBindPose )
    {
        FbxNode* lNode = FbxNode::Create( lScene, Utf8String( boneInfo->Name ).ToCStr() );

        Matrix4x4 worldTransformation;
        Matrix4x4::Invert( boneInfo->InverseBindPoseMatrix, worldTransformation );

        Matrix4x4 localTransformation = Matrix4x4::Multiply( worldTransformation, inverseParentTransformation );

        Vector3 scale, translation;
        Quaternion rotation;

        Matrix4x4::Decompose( localTransformation, scale, rotation, translation );

        FbxVector4 eulerAngles;
        eulerAngles.SetXYZ( FbxQuaternion( rotation.X, rotation.Y, rotation.Z, rotation.W ) );

        lNode->LclTranslation.Set( FbxVector4( translation.X, translation.Y, translation.Z ) );
        lNode->LclRotation.Set( eulerAngles );
        lNode->LclScaling.Set( FbxVector4( scale.X, scale.Y, scale.Z ) );

        FbxSkeleton* lSkeleton = FbxSkeleton::Create( lScene, Utf8String( boneInfo->Name ).ToCStr() );

        lSkeleton->SetSkeletonType( FbxSkeleton::eLimbNode );
        lSkeleton->LimbLength = 0.1;

        lNode->SetNodeAttribute( lSkeleton );
        lBindPose->Add( lNode, CreateFbxAMatrixFromNumerics( worldTransformation ) );

        return lNode;
    }

    void CreateFbxNodesFromBoneInfos( List<BoneInfo^>^ boneInfos, FbxScene* lScene, FbxNode* lParentNode, FbxPose* lBindPose, BoneInfo^ parentBoneInfo, Dictionary<String^, IntPtr>^ convertedBones )
    {
        for each ( BoneInfo^ boneInfo in boneInfos )
        {
            if ( boneInfo->Parent != parentBoneInfo )
                continue;

            IntPtr nodePtr;
            FbxNode* lNode;

            if ( !convertedBones->TryGetValue( boneInfo->Name, nodePtr ) )
            {
                lNode = CreateFbxNodeFromBoneInfo( boneInfo, parentBoneInfo != nullptr ? parentBoneInfo->InverseBindPoseMatrix : Matrix4x4::Identity, lScene, lBindPose );
                lParentNode->AddChild( lNode );

                convertedBones->Add( boneInfo->Name, IntPtr( lNode ) );
            }

            else
            {
                lNode = ( FbxNode* ) nodePtr.ToPointer();
            }

            CreateFbxNodesFromBoneInfos( boneInfos, lScene, lNode, lBindPose, boneInfo, convertedBones );
        }
    }

    FbxExporterCore::FbxExporterCore()
    {
        lManager = FbxManager::Create();
    }

    FbxExporterCore::~FbxExporterCore()
    {
        lManager->Destroy();
    }

    void FbxExporterCore::ExportToFile( ObjectSet^ objectSet, String^ destinationFilePath )
    {
        String^ texturesDirectoryPath = Path::GetDirectoryName( destinationFilePath );

        if ( objectSet->TextureSet != nullptr )
        {
            for each ( Texture^ texture in objectSet->TextureSet->Textures )
            {
                if ( String::IsNullOrEmpty( texture->Name ) )
                    texture->Name = Guid::NewGuid().ToString();

                String^ extension = TextureFormatUtilities::IsBlockCompressed( texture->Format ) && !texture->IsYCbCr ? ".dds" : ".png";
                TextureDecoder::DecodeToFile( texture, Path::Combine( texturesDirectoryPath, texture->Name + extension ) );
            }
        }

        int lFileFormat = lManager->GetIOPluginRegistry()->GetNativeWriterFormat();

        ::FbxExporter* lExporter = ::FbxExporter::Create( lManager, "" );
        lExporter->SetFileExportVersion( FBX_2014_00_COMPATIBLE );

        bool lExportStatus = lExporter->Initialize( Utf8String( destinationFilePath ).ToCStr(), lFileFormat, lManager->GetIOSettings() );

        if ( !lExportStatus )
            throw gcnew Exception( String::Format( "Failed to export FBX file ({0})", destinationFilePath ) );

        FbxScene* lScene = FbxScene::Create( lManager, "" );

        FbxPose* lBindPose = FbxPose::Create( lScene, "BindPoses" );
        lBindPose->SetIsBindPose( true );

        FbxNode* lRootNode = lScene->GetRootNode();

        Dictionary<String^, IntPtr>^ convertedBones = gcnew Dictionary<String^, IntPtr>();

        FbxNode* lSkeletonNode = FbxNode::Create( lScene, "gblctr" );

        FbxSkeleton* lSkeleton = FbxSkeleton::Create( lScene, "gblctr" );
        lSkeleton->SetSkeletonType( FbxSkeleton::eRoot );

        lSkeletonNode->SetNodeAttribute( lSkeleton );

        for each ( Objects::Object^ object in objectSet->Objects )
        {
            if ( object->Skin != nullptr )
                CreateFbxNodesFromBoneInfos( object->Skin->Bones, lScene, lSkeletonNode, lBindPose, nullptr, convertedBones );

            FbxNode* lObjectNode = CreateFbxNodeFromObject( object, objectSet->TextureSet, texturesDirectoryPath, lScene, lBindPose, convertedBones );
            lRootNode->AddChild( lObjectNode );
        }

        if ( convertedBones->Count > 0 )
        {
            lBindPose->Add( lSkeletonNode, FbxMatrix() );
            lRootNode->AddChild( lSkeletonNode );
        }

        else
            lSkeletonNode->Destroy( true );

        lScene->AddPose( lBindPose );

        lScene->GetGlobalSettings().SetAxisSystem( FbxAxisSystem::OpenGL );
        lScene->GetGlobalSettings().SetSystemUnit( FbxSystemUnit::m );

        lExporter->Export( lScene );
        lExporter->Destroy();

        lScene->Destroy( true );
    }
}
