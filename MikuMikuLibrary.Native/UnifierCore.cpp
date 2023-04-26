#include "UnifierCore.h"

using namespace Numerics;

namespace MikuMikuLibrary::Objects::Processing
{
    using namespace Numerics;

    void UnifierCore::Unify(Mesh^ mesh)
    {
        uint vertexCount = mesh->Positions->Length;
        std::vector<int> subMeshIndices;

        if (mesh->BlendIndices != nullptr && mesh->BlendWeights != nullptr)
        {
            // If the mesh uses skinning, each sub-mesh need to have their vertices isolated.
            // We can make the mesh optimizer account for this by giving it information about what sub-meshes the vertices are attached to.
            subMeshIndices.resize(vertexCount, -1);

            for (int i = 0; i < mesh->SubMeshes->Count; i++)
            {
                SubMesh^ subMesh = mesh->SubMeshes[i];
                if (subMesh->BoneIndices == nullptr)
                    continue;

                for each (uint vertexIndex in subMesh->Indices)
                {
                    if (vertexIndex > vertexCount)
                        continue;

                    int& subMeshIndex = subMeshIndices[vertexIndex];

                    // If we run into vertices shared between sub-meshes, isolate the mesh and restart the process.
                    if (subMeshIndex >= 0 && subMeshIndex != i)
                    {
                        Isolator::Isolate(mesh);

                        vertexCount = mesh->Positions->Length;

                        subMeshIndices.clear();
                        subMeshIndices.resize(vertexCount, -1);

                        i = 0;
                        break;
                    }

                    subMeshIndex = i;
                }
            }
        }

        const pin_ptr<Vector3> positions = mesh->Positions != nullptr ? &mesh->Positions[0] : nullptr;
        const pin_ptr<Vector3> normals = mesh->Normals != nullptr ? &mesh->Normals[0] : nullptr;
        const pin_ptr<Vector4> tangents = mesh->Tangents != nullptr ? &mesh->Tangents[0] : nullptr;
        const pin_ptr<Vector2> texCoords0 = mesh->TexCoords0 != nullptr ? &mesh->TexCoords0[0] : nullptr;
        const pin_ptr<Vector2> texCoords1 = mesh->TexCoords1 != nullptr ? &mesh->TexCoords1[0] : nullptr;
        const pin_ptr<Vector2> texCoords2 = mesh->TexCoords2 != nullptr ? &mesh->TexCoords2[0] : nullptr;
        const pin_ptr<Vector2> texCoords3 = mesh->TexCoords3 != nullptr ? &mesh->TexCoords3[0] : nullptr;
        const pin_ptr<Vector4> colors0 = mesh->Colors0 != nullptr ? &mesh->Colors0[0] : nullptr;
        const pin_ptr<Vector4> colors1 = mesh->Colors1 != nullptr ? &mesh->Colors1[0] : nullptr;
        const pin_ptr<Vector4> blendWeights = mesh->BlendWeights != nullptr ? &mesh->BlendWeights[0] : nullptr;
        const pin_ptr<Vector4Int> blendIndices = mesh->BlendIndices != nullptr ? &mesh->BlendIndices[0] : nullptr;

        std::vector<meshopt_Stream> streams;

        if (positions != nullptr) streams.push_back({ positions, sizeof(Vector3), sizeof(Vector3) });
        if (normals != nullptr) streams.push_back({ normals, sizeof(Vector3), sizeof(Vector3) });
        if (tangents != nullptr) streams.push_back({ tangents, sizeof(Vector4), sizeof(Vector4) });
        if (texCoords0 != nullptr) streams.push_back({ texCoords0, sizeof(Vector2), sizeof(Vector2) });
        if (texCoords1 != nullptr) streams.push_back({ texCoords1, sizeof(Vector2), sizeof(Vector2) });
        if (texCoords2 != nullptr) streams.push_back({ texCoords2, sizeof(Vector2), sizeof(Vector2) });
        if (texCoords3 != nullptr) streams.push_back({ texCoords3, sizeof(Vector2), sizeof(Vector2) });
        if (colors0 != nullptr) streams.push_back({ colors0, sizeof(Vector4), sizeof(Vector4) });
        if (colors1 != nullptr) streams.push_back({ colors1, sizeof(Vector4), sizeof(Vector4) });
        if (blendWeights != nullptr) streams.push_back({ blendWeights, sizeof(Vector4), sizeof(Vector4) });
        if (blendIndices != nullptr) streams.push_back({ blendIndices, sizeof(Vector4Int), sizeof(Vector4Int) });
        if (!subMeshIndices.empty()) streams.push_back({ subMeshIndices.data(), sizeof(uint32_t), sizeof(uint32_t) });

        std::vector<uint> remap(vertexCount);
        size_t newVertexCount;

        if (mesh->SubMeshes->Count == 1)
        {
            const pin_ptr<uint> indices = &mesh->SubMeshes[0]->Indices[0];

            newVertexCount = meshopt_generateVertexRemapMulti(
                remap.data(), indices, mesh->SubMeshes[0]->Indices->Length, vertexCount, streams.data(), streams.size());
        }
        else
        {
            size_t indexCount = 0;

            for each (SubMesh^ subMesh in mesh->SubMeshes)
                indexCount += subMesh->Indices->Length;

            std::vector<uint> indices(indexCount);
            int indexOffset = 0;

            for each (SubMesh^ subMesh in mesh->SubMeshes)
            {
                const pin_ptr<uint> source = &subMesh->Indices[0];
                memcpy(indices.data() + indexOffset, source, subMesh->Indices->Length * sizeof(uint));

                indexOffset += subMesh->Indices->Length;
            }

            newVertexCount = meshopt_generateVertexRemapMulti(
                remap.data(), indices.data(), indices.size(), vertexCount, streams.data(), streams.size());
        }

        if (positions != nullptr)
        {
            mesh->Positions = gcnew array<Vector3>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector3> destination = &mesh->Positions[0];
            meshopt_remapVertexBuffer(destination, positions, vertexCount, sizeof(Vector3), remap.data());
        }

        if (normals != nullptr)
        {
            mesh->Normals = gcnew array<Vector3>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector3> destination = &mesh->Normals[0];
            meshopt_remapVertexBuffer(destination, normals, vertexCount, sizeof(Vector3), remap.data());
        }

        if (tangents != nullptr)
        {
            mesh->Tangents = gcnew array<Vector4>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector4> destination = &mesh->Tangents[0];
            meshopt_remapVertexBuffer(destination, tangents, vertexCount, sizeof(Vector4), remap.data());
        }

        if (texCoords0 != nullptr)
        {
            mesh->TexCoords0 = gcnew array<Vector2>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector2> destination = &mesh->TexCoords0[0];
            meshopt_remapVertexBuffer(destination, texCoords0, vertexCount, sizeof(Vector2), remap.data());
        }

        if (texCoords1 != nullptr)
        {
            mesh->TexCoords1 = gcnew array<Vector2>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector2> destination = &mesh->TexCoords1[0];
            meshopt_remapVertexBuffer(destination, texCoords1, vertexCount, sizeof(Vector2), remap.data());
        }

        if (texCoords2 != nullptr)
        {
            mesh->TexCoords2 = gcnew array<Vector2>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector2> destination = &mesh->TexCoords2[0];
            meshopt_remapVertexBuffer(destination, texCoords2, vertexCount, sizeof(Vector2), remap.data());
        }

        if (texCoords3 != nullptr)
        {
            mesh->TexCoords3 = gcnew array<Vector2>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector2> destination = &mesh->TexCoords3[0];
            meshopt_remapVertexBuffer(destination, texCoords3, vertexCount, sizeof(Vector2), remap.data());
        }

        if (colors0 != nullptr)
        {
            mesh->Colors0 = gcnew array<Vector4>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector4> destination = &mesh->Colors0[0];
            meshopt_remapVertexBuffer(destination, colors0, vertexCount, sizeof(Vector4), remap.data());
        }

        if (colors1 != nullptr)
        {
            mesh->Colors1 = gcnew array<Vector4>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector4> destination = &mesh->Colors1[0];
            meshopt_remapVertexBuffer(destination, colors1, vertexCount, sizeof(Vector4), remap.data());
        }

        if (blendWeights != nullptr)
        {
            mesh->BlendWeights = gcnew array<Vector4>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector4> destination = &mesh->BlendWeights[0];
            meshopt_remapVertexBuffer(destination, blendWeights, vertexCount, sizeof(Vector4), remap.data());
        }

        if (blendIndices != nullptr)
        {
            mesh->BlendIndices = gcnew array<Vector4Int>(static_cast<int>(newVertexCount));
            const pin_ptr<Vector4Int> destination = &mesh->BlendIndices[0];
            meshopt_remapVertexBuffer(destination, blendIndices, vertexCount, sizeof(Vector4Int), remap.data());
        }

        for each (SubMesh^ subMesh in mesh->SubMeshes)
        {
            const pin_ptr<uint> indices = &subMesh->Indices[0];
            meshopt_remapIndexBuffer(indices, indices, subMesh->Indices->Length, remap.data());
        }
    }
}
