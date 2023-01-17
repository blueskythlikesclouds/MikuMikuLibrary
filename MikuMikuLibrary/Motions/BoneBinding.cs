namespace MikuMikuLibrary.Motions;

public class BoneBinding
{
    public string Name { get; set; }
    public KeyBinding Position { get; set; }
    public KeyBinding Rotation { get; set; }
    public KeyBinding IK { get; set; }

    public void Merge(BoneBinding other)
    {
        if (Position == null)
            Position = other.Position;
        else if (other.Position != null)
            Position.Merge(other.Position);

        if (Rotation == null)
            Rotation = other.Rotation;
        else if (other.Rotation != null)
            Rotation.Merge(other.Rotation);

        if (IK == null)
            IK = other.IK;
        else if (other.IK != null)
            IK.Merge(other.IK);
    }

    public void Sort()
    {
        Position?.Sort();
        Rotation?.Sort();
        IK?.Sort();
    }
}