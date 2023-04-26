using MikuMikuLibrary.Geometry;

namespace MikuMikuLibrary.Objects.Processing;

public class AabbCalculator
{
    public static void Calculate(Object obj)
    {
        var objAabb = new AxisAlignedBoundingBox();

        foreach (var mesh in obj.Meshes)
        {
            var meshAabb = new AxisAlignedBoundingBox();

            foreach (var subMesh in mesh.SubMeshes)
            {
                var subMeshAabb = new AxisAlignedBoundingBox();

                foreach (uint index in subMesh.Indices)
                {
                    if (index < mesh.Positions.Length)
                        subMeshAabb.AddPoint(mesh.Positions[index]);
                }

                subMesh.BoundingSphere = subMeshAabb.ToBoundingSphere();
                subMesh.BoundingBox = subMeshAabb.ToBoundingBox();

                meshAabb.Merge(subMeshAabb);
            }

            mesh.BoundingSphere = meshAabb.ToBoundingSphere();

            objAabb.Merge(meshAabb);
        }

        obj.BoundingSphere = objAabb.ToBoundingSphere();
    }
}