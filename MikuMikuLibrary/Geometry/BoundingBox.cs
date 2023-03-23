namespace MikuMikuLibrary.Geometry;

public struct BoundingBox
{
    public Vector3 Center;
    public float Width;
    public float Height;
    public float Depth;

    public BoundingSphere ToBoundingSphere()
    {
        return new BoundingSphere
        {
            Center = Center,
            Radius = MathF.Sqrt(Width * Width + Height * Height + Depth * Depth) / 2.0f
        };
    }

    public override string ToString() =>
        $"[{Center}, <{Width}, {Height}, {Depth}>]";

    public BoundingBox(AxisAlignedBoundingBox aabb)
    {
        Center = aabb.Center;
        Width = aabb.SizeX;
        Height = aabb.SizeY;
        Depth = aabb.SizeZ;
    }
}