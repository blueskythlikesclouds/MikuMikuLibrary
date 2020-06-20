using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class ConstraintBlock : NodeBlock
    {
        public override string Signature => "CNS";

        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public int Field12 { get; set; }
        public string Field13 { get; set; }
        public float Field14 { get; set; }
        public float Field15 { get; set; }
        public float Field16 { get; set; }
        public float Field17 { get; set; }
        public float Field18 { get; set; }
        public float Field19 { get; set; }
        public float Field20 { get; set; }
        public float Field21 { get; set; }
        public float Field22 { get; set; }
        public float Field23 { get; set; }
        public float Field24 { get; set; }
        public float Field25 { get; set; }
        public float Field26 { get; set; }
        public float Field27 { get; set; }
        public float Field28 { get; set; }
        public float Field29 { get; set; }
        public float Field30 { get; set; }
        public float Field31 { get; set; }
        public float Field32 { get; set; }
        public float Field33 { get; set; }
        public float Field34 { get; set; }
        public float Field35 { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            Field10 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field11 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field14 = reader.ReadSingle();
            Field15 = reader.ReadSingle();
            Field16 = reader.ReadSingle();
            Field17 = reader.ReadSingle();
            Field18 = reader.ReadSingle();
            Field19 = reader.ReadSingle();
            Field20 = reader.ReadSingle();
            Field21 = reader.ReadSingle();
            Field22 = reader.ReadSingle();
            Field23 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field25 = reader.ReadSingle();
            Field26 = reader.ReadSingle();
            Field27 = reader.ReadSingle();
            Field28 = reader.ReadSingle();
            Field29 = reader.ReadSingle();
            Field30 = reader.ReadSingle();
            Field31 = reader.ReadSingle();
            Field32 = reader.ReadSingle();
            Field33 = reader.ReadSingle();
            Field34 = reader.ReadSingle();
            Field35 = reader.ReadSingle();
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( Field10 );
            writer.AddStringToStringTable( Field11 );
            writer.Write( Field12 );
            writer.AddStringToStringTable( Field13 );
            writer.Write( Field14 );
            writer.Write( Field15 );
            writer.Write( Field16 );
            writer.Write( Field17 );
            writer.Write( Field18 );
            writer.Write( Field19 );
            writer.Write( Field20 );
            writer.Write( Field21 );
            writer.Write( Field22 );
            writer.Write( Field23 );
            writer.Write( Field24 );
            writer.Write( Field25 );
            writer.Write( Field26 );
            writer.Write( Field27 );
            writer.Write( Field28 );
            writer.Write( Field29 );
            writer.Write( Field30 );
            writer.Write( Field31 );
            writer.Write( Field32 );
            writer.Write( Field33 );
            writer.Write( Field34 );
            writer.Write( Field35 );
        }
    }
}