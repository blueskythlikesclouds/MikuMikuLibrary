#include "OptimizerCore.h"

using namespace Collections;
using namespace Numerics;

namespace MikuMikuLibrary::Objects::Processing
{
    using namespace Numerics;

    void OptimizerCore::Optimize(Mesh^ mesh, bool generateStrips)
    {
        const size_t vertexCount = mesh->Positions->Length;

        for each (SubMesh^ subMesh in mesh->SubMeshes)
        {
            if (subMesh->PrimitiveType == PrimitiveType::TriangleStrip)
            {
                subMesh->PrimitiveType = PrimitiveType::Triangles;
                subMesh->Indices = Stripifier::Unstripify(subMesh->Indices);
            }

            const pin_ptr<uint> indices = &subMesh->Indices[0];

            if (generateStrips)
                meshopt_optimizeVertexCacheStrip(indices, indices, subMesh->Indices->Length, vertexCount);
            else
                meshopt_optimizeVertexCache(indices, indices, subMesh->Indices->Length, vertexCount);
        }

        std::vector<uint> remap(vertexCount);
        size_t newVertexCount;

        if (mesh->SubMeshes->Count == 1)
        {
            const pin_ptr<uint> indices = &mesh->SubMeshes[0]->Indices[0];

            newVertexCount = meshopt_optimizeVertexFetchRemap(
                remap.data(), indices, mesh->SubMeshes[0]->Indices->Length, vertexCount);
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
                const pin_ptr<uint> subMeshIndices = &subMesh->Indices[0];
                memcpy(indices.data() + indexOffset, subMeshIndices, subMesh->Indices->Length * sizeof(uint));

                indexOffset += subMesh->Indices->Length;
            }

            newVertexCount = meshopt_optimizeVertexFetchRemap(
                remap.data(), indices.data(), indices.size(), vertexCount);
        }

        if (mesh->Positions != nullptr)
        {
            const pin_ptr<Vector3> positions = &mesh->Positions[0];
            mesh->Positions = gcnew array<Vector3>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector3> destination = &mesh->Positions[0];
            meshopt_remapVertexBuffer(destination, positions, vertexCount, sizeof(Vector3), remap.data());
        }

        if (mesh->Normals != nullptr)
        {
            const pin_ptr<Vector3> normals = &mesh->Normals[0];
            mesh->Normals = gcnew array<Vector3>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector3> destination = &mesh->Normals[0];
            meshopt_remapVertexBuffer(destination, normals, vertexCount, sizeof(Vector3), remap.data());
        }

        if (mesh->Tangents != nullptr)
        {
            const pin_ptr<Vector4> tangents = &mesh->Tangents[0];
            mesh->Tangents = gcnew array<Vector4>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector4> destination = &mesh->Tangents[0];
            meshopt_remapVertexBuffer(destination, tangents, vertexCount, sizeof(Vector4), remap.data());
        }

        if (mesh->TexCoords0 != nullptr)
        {
            const pin_ptr<Vector2> texCoords0 = &mesh->TexCoords0[0];
            mesh->TexCoords0 = gcnew array<Vector2>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector2> destination = &mesh->TexCoords0[0];
            meshopt_remapVertexBuffer(destination, texCoords0, vertexCount, sizeof(Vector2), remap.data());
        }

        if (mesh->TexCoords1 != nullptr)
        {
            const pin_ptr<Vector2> texCoords1 = &mesh->TexCoords1[0];
            mesh->TexCoords1 = gcnew array<Vector2>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector2> destination = &mesh->TexCoords1[0];
            meshopt_remapVertexBuffer(destination, texCoords1, vertexCount, sizeof(Vector2), remap.data());
        }

        if (mesh->TexCoords2 != nullptr)
        {
            const pin_ptr<Vector2> texCoords2 = &mesh->TexCoords2[0];
            mesh->TexCoords2 = gcnew array<Vector2>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector2> destination = &mesh->TexCoords2[0];
            meshopt_remapVertexBuffer(destination, texCoords2, vertexCount, sizeof(Vector2), remap.data());
        }

        if (mesh->TexCoords3 != nullptr)
        {
            const pin_ptr<Vector2> texCoords3 = &mesh->TexCoords3[0];
            mesh->TexCoords3 = gcnew array<Vector2>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector2> destination = &mesh->TexCoords3[0];
            meshopt_remapVertexBuffer(destination, texCoords3, vertexCount, sizeof(Vector2), remap.data());
        }

        if (mesh->Colors0 != nullptr)
        {
            const pin_ptr<Vector4> colors0 = &mesh->Colors0[0];
            mesh->Colors0 = gcnew array<Vector4>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector4> destination = &mesh->Colors0[0];
            meshopt_remapVertexBuffer(destination, colors0, vertexCount, sizeof(Vector4), remap.data());
        }

        if (mesh->Colors1 != nullptr)
        {
            const pin_ptr<Vector4> colors1 = &mesh->Colors1[0];
            mesh->Colors1 = gcnew array<Vector4>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector4> destination = &mesh->Colors1[0];
            meshopt_remapVertexBuffer(destination, colors1, vertexCount, sizeof(Vector4), remap.data());
        }

        if (mesh->BlendWeights != nullptr)
        {
            const pin_ptr<Vector4> blendWeights = &mesh->BlendWeights[0];
            mesh->BlendWeights = gcnew array<Vector4>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector4> destination = &mesh->BlendWeights[0];
            meshopt_remapVertexBuffer(destination, blendWeights, vertexCount, sizeof(Vector4), remap.data());
        }

        if (mesh->BlendIndices != nullptr)
        {
            const pin_ptr<Vector4Int> blendIndices = &mesh->BlendIndices[0];
            mesh->BlendIndices = gcnew array<Vector4Int>(static_cast<int>(newVertexCount));

            const pin_ptr<Vector4Int> destination = &mesh->BlendIndices[0];
            meshopt_remapVertexBuffer(destination, blendIndices, vertexCount, sizeof(Vector4Int), remap.data());
        }

        for each (SubMesh^ subMesh in mesh->SubMeshes)
        {
            const pin_ptr<uint> indices = &subMesh->Indices[0];
            meshopt_remapIndexBuffer(indices, indices, subMesh->Indices->Length, remap.data());

            if (generateStrips)
            {
                subMesh->PrimitiveType = PrimitiveType::TriangleStrip;
                subMesh->Indices = Stripifier::Stripify(subMesh->Indices);
            }
        }

        // Remove bone indices leftover from splitter
        if (mesh->BlendIndices != nullptr)
        {
            BitArray^ remappedVertices = gcnew BitArray(static_cast<int>(newVertexCount), false);

            for each (SubMesh^ subMesh in mesh->SubMeshes)
            {
                std::unordered_map<ushort, int> boneTable;

                for each (uint index in subMesh->Indices)
                {
                    if (index >= newVertexCount || remappedVertices[index])
                        continue;

                    Vector4Int blendIndices = mesh->BlendIndices[index];

                    for (int i = 0; i < 4; i++)
                    {
                        int blendIndex = blendIndices[i];
                        if (blendIndex >= 0)
                        {
                            ushort boneIndex = subMesh->BoneIndices[blendIndex];
                            const auto pair = boneTable.find(boneIndex);

                            if (pair == boneTable.end())
                            {
                                blendIndex = static_cast<int>(boneTable.size());
                                boneTable.emplace(boneIndex, blendIndex);
                            }
                            else
                            {
                                blendIndex = pair->second;
                            }
                            blendIndices[i] = blendIndex;
                        }
                    }

                    mesh->BlendIndices[index] = blendIndices;
                    remappedVertices[index] = true;
                }

                subMesh->BoneIndices = gcnew array<ushort>(static_cast<int>(boneTable.size()));

                for (const auto& [boneIndex, blendIndex] : boneTable)
                    subMesh->BoneIndices[blendIndex] = boneIndex;
            }
        }
    }
}
