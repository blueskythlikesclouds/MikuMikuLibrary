using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Skeletons
{
    public enum BoneType
    {
        Rotation = 0,
        Type1 = 1,
        Position = 2,
        PositionRotation = 3,
        HeadIKRotation = 4,
        ArmIKRotation = 5,
        LegIKRotation = 6
    }

    public class Bone
    {
        public BoneType Type { get; set; }
        public bool HasParent { get; set; }
        public byte Parent { get; set; }
        public byte PoleTarget { get; set; }
        public byte Mirror { get; set; }
        public byte DisableMotAnim { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Type = ( BoneType ) reader.ReadByte();
            HasParent = reader.ReadBoolean();
            Parent = reader.ReadByte();
            PoleTarget = reader.ReadByte();
            Mirror = reader.ReadByte();
            DisableMotAnim = reader.ReadByte();
            reader.SeekCurrent( 2 );
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( byte ) Type );
            writer.Write( HasParent );
            writer.Write( Parent );
            writer.Write( PoleTarget );
            writer.Write( Mirror );
            writer.Write( DisableMotAnim );
            writer.WriteNulls( sizeof( ushort ) );
            writer.AddStringToStringTable( Name );
        }
    }
}