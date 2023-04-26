#include "TangentGeneratorCore.h"

using namespace Collections::Generic;
using namespace Runtime::InteropServices;
using namespace Numerics;

namespace MikuMikuLibrary::Objects::Processing
{
    ref struct Context
    {
        ref struct Pair
        {
            Mesh^ Mesh;
            SubMesh^ SubMesh;
        };

        List<int>^ Offsets;
        List<Pair^>^ Pairs;
        int IndexCount;

        Mesh^ FindMesh(int faceIndex, int& vertexIndex)
        {
            for (int i = 0; i < Offsets->Count; i++)
            {
                if (i == (Offsets->Count - 1) || Offsets[i + 1] > faceIndex)
                {
                    Pair^ pair = Pairs[i];
                    vertexIndex = pair->SubMesh->Indices[faceIndex - Offsets[i]];
                    return pair->Mesh;
                }
            }

            return nullptr;
        }
    };

    Context^ GetContext(const SMikkTSpaceContext* pContext)
    {
        return (Context^)GCHandle::FromIntPtr(IntPtr(pContext->m_pUserData)).Target;
    }

    static int getNumFaces(const SMikkTSpaceContext* pContext)
    {
        return GetContext(pContext)->IndexCount / 3;
    }

    static int getNumVerticesOfFace(const SMikkTSpaceContext* pContext, const int iFace)
    {
        return 3;
    }

    static void getPosition(const SMikkTSpaceContext* pContext, float fvPosOut[], const int iFace, const int iVert)
    {
        Context^ context = GetContext(pContext);

        int vertexIndex;
        Mesh^ mesh = context->FindMesh(iFace * 3 + iVert, vertexIndex);

        fvPosOut[0] = mesh->Positions[vertexIndex].X;
        fvPosOut[1] = mesh->Positions[vertexIndex].Y;
        fvPosOut[2] = mesh->Positions[vertexIndex].Z;
    }

    static void getNormal(const SMikkTSpaceContext* pContext, float fvNormOut[], const int iFace, const int iVert)
    {
        Context^ context = GetContext(pContext);

        int vertexIndex;
        Mesh^ mesh = context->FindMesh(iFace * 3 + iVert, vertexIndex);

        fvNormOut[0] = mesh->Normals[vertexIndex].X;
        fvNormOut[1] = mesh->Normals[vertexIndex].Y;
        fvNormOut[2] = mesh->Normals[vertexIndex].Z;
    }

    static void getTexCoord(const SMikkTSpaceContext* pContext, float fvTexcOut[], const int iFace, const int iVert)
    {
        Context^ context = GetContext(pContext);

        int vertexIndex;
        Mesh^ mesh = context->FindMesh(iFace * 3 + iVert, vertexIndex);

        fvTexcOut[0] = mesh->TexCoords0[vertexIndex].X;
        fvTexcOut[1] = mesh->TexCoords0[vertexIndex].Y;
    }

    static void setTSpaceBasic(const SMikkTSpaceContext* pContext, const float fvTangent[], const float fSign, const int iFace, const int iVert)
    {
        Context^ context = GetContext(pContext);

        int vertexIndex;
        Mesh^ mesh = context->FindMesh(iFace * 3 + iVert, vertexIndex);

        mesh->Tangents[vertexIndex] = Vector4(fvTangent[0], fvTangent[1], fvTangent[2], fSign);
    }

    static SMikkTSpaceInterface mikktSpaceInterface = { getNumFaces, getNumVerticesOfFace, getPosition, getNormal, getTexCoord, setTSpaceBasic, nullptr };

    void TangentGeneratorCore::Generate(Objects::Object^ obj)
    {
        Context^ context = gcnew Context();

        context->Offsets = gcnew List<int>();
        context->Pairs = gcnew List<Context::Pair^>();
        context->IndexCount = 0;

        for each (Mesh^ mesh in obj->Meshes)
        {
            if (mesh->Positions == nullptr || mesh->Normals == nullptr || mesh->TexCoords0 == nullptr)
                continue;

            if (mesh->Tangents == nullptr)
                mesh->Tangents = gcnew array<Vector4>(mesh->Positions->Length);

            for each (SubMesh^ subMesh in mesh->SubMeshes)
            {
                context->Offsets->Add(context->IndexCount);
                context->IndexCount += subMesh->Indices->Length;

                Context::Pair^ pair = gcnew Context::Pair();
                pair->Mesh = mesh;
                pair->SubMesh = subMesh;
                context->Pairs->Add(pair);
            }
        }

        GCHandle handle = GCHandle::Alloc(context);

        SMikkTSpaceContext ctx = { &mikktSpaceInterface, GCHandle::ToIntPtr(handle).ToPointer() };
        genTangSpaceDefault(&ctx);

        handle.Free();
    }
}
