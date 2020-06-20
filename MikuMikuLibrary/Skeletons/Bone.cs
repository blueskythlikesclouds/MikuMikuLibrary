using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Skeletons
{
    public enum BoneType
    {
        Rotation = 0,
        Type1 = 1,
        Position = 2,
        Type3 = 3,
        Type4 = 4,
        Type5 = 5,
        Type6 = 6
    }

    public class Bone
    {
        public BoneType Type { get; set; }
        public bool HasParent { get; set; }
        public byte Field01 { get; set; }
        public byte Field02 { get; set; }
        public byte Field03 { get; set; }
        public byte Field04 { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Type = ( BoneType ) reader.ReadByte();
            HasParent = reader.ReadBoolean();
            Field01 = reader.ReadByte();
            Field02 = reader.ReadByte();
            Field03 = reader.ReadByte();
            Field04 = reader.ReadByte();
            reader.SeekCurrent( 2 );
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( byte ) Type );
            writer.Write( HasParent );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.Write( Field04 );
            writer.WriteNulls( sizeof( ushort ) );
            writer.AddStringToStringTable( Name );
        }
    }
}