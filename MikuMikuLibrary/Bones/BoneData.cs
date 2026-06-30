using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Bones;

public class BoneData : BinaryFile
{
    public override BinaryFileFlags Flags =>
        BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

    public List<Skeleton> Skeletons { get; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        uint signature = reader.ReadUInt32();
        int skeletonCount = reader.ReadInt32();
        long skeletonsOffset = reader.ReadOffset();
        long skeletonNamesOffset = reader.ReadOffset();

        reader.ReadAtOffset(skeletonsOffset, () =>
        {
            Skeletons.Capacity = skeletonCount;

            for (int i = 0; i < skeletonCount; i++)
            {
                reader.ReadOffset(() =>
                {
                    var skeleton = new Skeleton();
                    skeleton.Read(reader);
                    Skeletons.Add(skeleton);
                });
            }
        });

        reader.ReadAtOffset(skeletonNamesOffset, () =>
        {
            foreach (var skeleton in Skeletons)
                skeleton.Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
        });
    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        writer.Write(0x09102720);
        writer.Write(Skeletons.Count);
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (var skeleton in Skeletons)
                writer.WriteOffset(16, AlignmentMode.Left, () => skeleton.Write(writer));
        });
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (var skeleton in Skeletons)
                writer.WriteStringOffset(skeleton.Name);
        });
        writer.WriteNulls(5 * sizeof(uint));
    }

    public BoneData()
    {
        Skeletons = new List<Skeleton>();
    }
}
