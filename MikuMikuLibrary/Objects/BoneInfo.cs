namespace MikuMikuLibrary.Objects;

public class BoneInfo
{
    public uint Id { get; set; } = 0xFFFFFFFF;
    public Matrix4x4 InverseBindPoseMatrix { get; set; }
    public string Name { get; set; }
    public bool IsEx { get; set; }
    public BoneInfo Parent { get; set; }
}