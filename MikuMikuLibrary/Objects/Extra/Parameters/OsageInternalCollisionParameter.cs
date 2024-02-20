using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Parameters;

public enum OsageInternalCollisionType : int
{
    End = 0,
    Sphere = 1,
    Cylinder = 2,
    Plane = 3,
    Ellipse = 4
}

public class OsageInternalCollisionParameter
{
    public OsageInternalCollisionType CollisionType { get; set; }
    public uint Head { get; set; }
    public uint Tail { get; set; }
    public float CollisionRadius { get; set; }
    public Vector3 HeadPosition { get; set; }
    public Vector3 TailPosition { get; set; }

    internal void Read(EndianBinaryReader reader)
    {
        CollisionType = (OsageInternalCollisionType)reader.ReadInt32();
        Head = reader.ReadUInt32();
        Tail = reader.ReadUInt32();
        CollisionRadius = reader.ReadSingle();
        HeadPosition = reader.ReadVector3();
        TailPosition = reader.ReadVector3();
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.Write((int)CollisionType);
        writer.Write(Head);
        writer.Write(Tail);
        writer.Write(CollisionRadius);
        writer.Write(HeadPosition);
        writer.Write(TailPosition);
    }
}
