using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters;


[TypeConverter(typeof(ExpandableObjectConverter))]
public class OsageInternalSkinParameter
{
    public string Name { get; set; }
    public float Force { get; set; }
    public float ForceGain { get; set; }
    public float AirResistance { get; set; }
    public float RotationY { get; set; }
    public float RotationZ { get; set; }
    public float HingeY { get; set; }
    public float HingeZ { get; set; }
    public float CollisionRadius { get; set; }
    public float Friction { get; set; }
    public float WindAffection { get; set; }
    public List<OsageInternalCollisionParameter> Collisions { get; }

    internal void Read(EndianBinaryReader reader)
    {
        long start = reader.Position;
        reader.SkipNulls(4);  // Truthfully, i don't know if this is inertial_cancel or just a reserved field. it seems to always be 0 though so i won't read it.
                              //  Besides, if i did read and write it, and someone wrote it as anything but 0, then it would from that point on be falsely read as FT, which wouldn't be good.

        Force = reader.ReadSingle();
        ForceGain = reader.ReadSingle();
        AirResistance = reader.ReadSingle();
        RotationY = reader.ReadSingle();
        RotationZ = reader.ReadSingle();
        HingeY = reader.ReadSingle();
        HingeZ = reader.ReadSingle();
        Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);

        reader.ReadOffset(() => 
        {
            while (true)
            {
                OsageInternalCollisionParameter collisionParameter = new OsageInternalCollisionParameter();
                collisionParameter.Read(reader);
                if (collisionParameter.CollisionType == OsageInternalCollisionType.End)
                {
                    break;
                }
                Collisions.Add(collisionParameter);

            };
        });

        CollisionRadius = reader.ReadSingle();
        Friction = reader.ReadSingle();
        WindAffection = reader.ReadSingle();
    }

    internal void Write(EndianBinaryWriter writer)
    {
        writer.WriteNulls(4);
        writer.Write(Force);
        writer.Write(ForceGain);
        writer.Write(AirResistance);
        writer.Write(RotationY);
        writer.Write(RotationZ);
        writer.Write(HingeY);
        writer.Write(HingeZ);
        writer.WriteStringOffset(Name);
        writer.WriteOffset(16, AlignmentMode.Left, () =>
        {
            foreach (var coll in Collisions)
            {
                coll.Write(writer);
            }

            new OsageInternalCollisionParameter() { CollisionType = OsageInternalCollisionType.End }.Write(writer);  // please suggest a better way to handle this
        });
        writer.Write(CollisionRadius);
        writer.Write(Friction);
        writer.Write(WindAffection);
    }

    public OsageInternalSkinParameter()
    {
        Collisions = new List<OsageInternalCollisionParameter>();
    }
}
