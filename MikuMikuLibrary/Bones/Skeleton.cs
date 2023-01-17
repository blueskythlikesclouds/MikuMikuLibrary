using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Bones;

public class Skeleton
{
    public string Name { get; set; }

    public List<Bone> Bones { get; }
    public List<Vector3> Positions { get; }
    public List<short> ParentIndices { get; }

    public List<string> ObjectBoneNames { get; }
    public List<string> MotionBoneNames { get; }

    public uint HeelHeight { get; set; }

    internal void Read(EndianBinaryReader reader)
    {
        long bonesOffset = reader.ReadOffset();
        int positionCount = reader.ReadInt32();
        long positionsOffset = reader.ReadOffset();
        long heelHeightOffset = reader.ReadOffset();
        int objectBoneNameCount = reader.ReadInt32();
        long objectBoneNamesOffset = reader.ReadOffset();
        int motionBoneNameCount = reader.ReadInt32();
        long motionBoneNamesOffset = reader.ReadOffset();
        long parentIndicesOffset = reader.ReadOffset();

        reader.ReadAtOffset(bonesOffset, () =>
        {
            while (true)
            {
                var bone = new Bone();
                bone.Read(reader);

                if (bone.Name == "End")
                    break;

                Bones.Add(bone);
            }
        });

        reader.ReadAtOffset(positionsOffset, () =>
        {
            Positions.Capacity = positionCount;

            for (int i = 0; i < positionCount; i++)
                Positions.Add(reader.ReadVector3());
        });

        reader.ReadAtOffset(heelHeightOffset, () => { HeelHeight = reader.ReadUInt32(); });

        reader.ReadAtOffset(objectBoneNamesOffset, () =>
        {
            ObjectBoneNames.Capacity = objectBoneNameCount;

            for (int i = 0; i < objectBoneNameCount; i++)
                ObjectBoneNames.Add(reader.ReadStringOffset(StringBinaryFormat.NullTerminated));
        });

        reader.ReadAtOffset(motionBoneNamesOffset, () =>
        {
            MotionBoneNames.Capacity = motionBoneNameCount;

            for (int i = 0; i < motionBoneNameCount; i++)
                MotionBoneNames.Add(reader.ReadStringOffset(StringBinaryFormat.NullTerminated));
        });

        reader.ReadAtOffset(parentIndicesOffset, () =>
        {
            ParentIndices.Capacity = motionBoneNameCount;

            for (int i = 0; i < motionBoneNameCount; i++)
                ParentIndices.Add(reader.ReadInt16());
        });
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (var bone in Bones)
                bone.Write(writer);

            writer.Write((byte)255);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)255);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.WriteStringOffset("End");
        });
        writer.Write(Positions.Count);
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (var position in Positions)
                writer.Write(position);
        });
        writer.WriteOffset(8, AlignmentMode.Left, () => writer.Write(HeelHeight));
        writer.Write(ObjectBoneNames.Count);
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (string boneName in ObjectBoneNames)
                writer.WriteStringOffset(boneName);
        });
        writer.Write(MotionBoneNames.Count);
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (string boneName in MotionBoneNames)
                writer.WriteStringOffset(boneName);
        });
        writer.WriteOffset(8, AlignmentMode.Left, () =>
        {
            foreach (short parentId in ParentIndices)
                writer.Write(parentId);
        });
        writer.WriteNulls(8 * sizeof(uint));
    }

    public Bone GetBone(string boneName) =>
        Bones.FirstOrDefault(x => x.Name.Equals(boneName, StringComparison.OrdinalIgnoreCase));

    public Skeleton()
    {
        Bones = new List<Bone>();
        Positions = new List<Vector3>();
        ObjectBoneNames = new List<string>();
        MotionBoneNames = new List<string>();
        ParentIndices = new List<short>();
    }
}