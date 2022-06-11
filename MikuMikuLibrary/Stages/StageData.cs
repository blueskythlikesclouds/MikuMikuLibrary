using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Stages;

public class StageData : BinaryFile
{
    public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

    public List<Stage> Stages { get; }

    public override void Read(EndianBinaryReader reader, ISection section = null)
    {
        int count = reader.ReadInt32();
        long stagesOffset = reader.ReadOffset();

        if (section != null)
        {
            reader.ReadAtOffset(stagesOffset, () =>
            {
                Stages.Capacity = count;

                for (int i = 0; i < count; i++)
                {
                    var stage = new Stage();
                    {
                        stage.Read(reader, Format);
                    }
                    Stages.Add(stage);
                }

            });

        }
        else
        {
            long stageEffectsOffset = reader.ReadOffset();
            long auth3dIdCountsOffset = reader.ReadOffset();
            long auth3dIdsOffset = reader.ReadOffset();

            if (reader.ReadBoolean())
            {
                Format = (BinaryFormat)reader.ReadByte();
            }

            else
            {
                long size = (stageEffectsOffset - stagesOffset) / count;
                Format = size == 104 ? BinaryFormat.DT :
                    size == 108 ? BinaryFormat.F :
                    size >= 112 ? BinaryFormat.FT :
                    throw new InvalidDataException();
            }

            reader.ReadAtOffset(stagesOffset, () =>
            {
                Stages.Capacity = count;

                for (int i = 0; i < count; i++)
                {
                    var stage = new Stage();
                    {
                        stage.Read(reader, Format);
                    }
                    Stages.Add(stage);
                }
            });

            reader.ReadAtOffset(stageEffectsOffset, () =>
            {
                foreach (var stage in Stages)
                    stage.ReadStageEffects(reader);
            });

            reader.ReadAtOffset(auth3dIdCountsOffset, () =>
            {
                var auth3dIdCounts = reader.ReadInt32s(count);

                reader.ReadAtOffset(auth3dIdsOffset, () =>
                {
                    for (int i = 0; i < count; i++)
                        Stages[i].ReadAuth3dIds(reader, auth3dIdCounts[i]);
                });
            });
        }
    }

    public override void Write(EndianBinaryWriter writer, ISection section = null)
    {
        if (section != null)
        {
            writer.Write(Stages.Count);

            writer.WriteOffset(16, AlignmentMode.Left, () =>
            {
                foreach (var stage in Stages)
                    stage.Write(writer, Format);
            });

        }
        else
        {
            writer.Write(Stages.Count);
            writer.WriteOffset(16, AlignmentMode.Left, () =>
            {
                foreach (var stage in Stages)
                    stage.Write(writer, Format);
            });
            writer.WriteOffset(16, AlignmentMode.Left, () =>
            {
                foreach (var stage in Stages)
                    stage.WriteStageEffects(writer);
            });
            writer.WriteOffset(16, AlignmentMode.Left, () =>
            {
                foreach (var stage in Stages)
                    writer.Write(stage.Auth3dIds.Count + 1);
            });
            writer.WriteOffset(16, AlignmentMode.Left, () =>
            {
                foreach (var stage in Stages)
                    stage.WriteAuth3dIds(writer);
            });

            // HACK: We store the format in the reserved part of header.
            writer.Write(true);
            writer.Write((byte)Format);
        }
    }

    public StageData()
    {
        Stages = new List<Stage>();
    }
}