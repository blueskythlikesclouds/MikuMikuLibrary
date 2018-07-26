using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    public class StageEntry
    {
        // Section 1
        public string Name { get; set; }
        public string Auth3DName { get; set; }
        public int ObjectID { get; set; }
        public int Field03 { get; set; }
        public int Field04 { get; set; }
        public int Field05 { get; set; }
        public int Field06 { get; set; }
        public int Field07 { get; set; }
        public int Field08 { get; set; }
        public int Field09 { get; set; }
        public int Field10 { get; set; }
        public int Field11 { get; set; }
        public int Field12 { get; set; }
        public int Field13 { get; set; }
        public int Field14 { get; set; }
        public int Field15 { get; set; }
        public string Field16 { get; set; }
        public int Field17 { get; set; }
        public int Field18 { get; set; }
        public int Field19 { get; set; }
        public int Field20 { get; set; }
        public float Field21 { get; set; }
        public float Field22 { get; set; }
        public float Field23 { get; set; }
        public float Field24 { get; set; }
        public float Field25 { get; set; }
        public float Field26 { get; set; }
        // Future Tone Exclusive
        public float Field27 { get; set; }

        // Section 2
        public int Field28 { get; set; }

        // Section 3
        public int Field29 { get; set; }
        public int Field30 { get; set; }
        public int Field31 { get; set; }
        public int Field32 { get; set; }
        public int Field33 { get; set; }
        public int Field34 { get; set; }
        public int Field35 { get; set; }
        public int Field36 { get; set; }
        public int Field37 { get; set; }
        public int Field38 { get; set; }
        public int Field39 { get; set; }
        public int Field40 { get; set; }
        public int Field41 { get; set; }
        public int Field42 { get; set; }
        public int Field43 { get; set; }
        public int Field44 { get; set; }
        public int Field45 { get; set; }
        public int Field46 { get; set; }
        public int Field47 { get; set; }
        public int Field48 { get; set; }
        public int Field49 { get; set; }
        public int Field50 { get; set; }
        public int Field51 { get; set; }
        public int Field52 { get; set; }

        // Section 4
        public int Index { get; set; }
        public int Field54 { get; set; }
        public int Field55 { get; set; }

        internal void ReadFirst( EndianBinaryReader reader, bool isFutureTone )
        {
            Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Auth3DName = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            ObjectID = reader.ReadInt32();
            Field03 = reader.ReadInt32();
            Field04 = reader.ReadInt32();
            Field05 = reader.ReadInt32();
            Field06 = reader.ReadInt32();
            Field07 = reader.ReadInt32();
            Field08 = reader.ReadInt32();
            Field09 = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            Field11 = reader.ReadInt32();
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadInt32();
            Field14 = reader.ReadInt32();
            Field15 = reader.ReadInt32();
            Field16 = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Field17 = reader.ReadInt32();
            Field18 = reader.ReadInt32();
            Field19 = reader.ReadInt32();
            Field20 = reader.ReadInt32();
            Field21 = reader.ReadSingle();
            Field22 = reader.ReadSingle();
            Field23 = reader.ReadSingle();
            Field24 = reader.ReadSingle();
            Field25 = reader.ReadSingle();
            Field26 = reader.ReadSingle();

            if ( isFutureTone )
                Field27 = reader.ReadSingle();
        }

        internal void WriteFirst( EndianBinaryWriter writer, bool isFutureTone )
        {
            writer.AddStringToStringTable( Name );
            writer.AddStringToStringTable( Auth3DName );
            writer.Write( ObjectID );
            writer.Write( Field03 );
            writer.Write( Field04 );
            writer.Write( Field05 );
            writer.Write( Field06 );
            writer.Write( Field07 );
            writer.Write( Field08 );
            writer.Write( Field09 );
            writer.Write( Field10 );
            writer.Write( Field11 );
            writer.Write( Field12 );
            writer.Write( Field13 );
            writer.Write( Field14 );
            writer.Write( Field15 );
            writer.AddStringToStringTable( Field16 );
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

            if ( isFutureTone )
                writer.Write( Field27 );
        }

        internal void ReadSecond( EndianBinaryReader reader )
        {
            Field29 = reader.ReadInt32();
            Field30 = reader.ReadInt32();
            Field31 = reader.ReadInt32();
            Field32 = reader.ReadInt32();
            Field33 = reader.ReadInt32();
            Field34 = reader.ReadInt32();
            Field35 = reader.ReadInt32();
            Field36 = reader.ReadInt32();
            Field37 = reader.ReadInt32();
            Field38 = reader.ReadInt32();
            Field39 = reader.ReadInt32();
            Field40 = reader.ReadInt32();
            Field41 = reader.ReadInt32();
            Field42 = reader.ReadInt32();
            Field43 = reader.ReadInt32();
            Field44 = reader.ReadInt32();
            Field45 = reader.ReadInt32();
            Field46 = reader.ReadInt32();
            Field47 = reader.ReadInt32();
            Field48 = reader.ReadInt32();
            Field49 = reader.ReadInt32();
            Field50 = reader.ReadInt32();
            Field51 = reader.ReadInt32();
            Field52 = reader.ReadInt32();
        }

        internal void WriteSecond( EndianBinaryWriter writer )
        {
            writer.Write( Field29 );
            writer.Write( Field30 );
            writer.Write( Field31 );
            writer.Write( Field32 );
            writer.Write( Field33 );
            writer.Write( Field34 );
            writer.Write( Field35 );
            writer.Write( Field36 );
            writer.Write( Field37 );
            writer.Write( Field38 );
            writer.Write( Field39 );
            writer.Write( Field40 );
            writer.Write( Field41 );
            writer.Write( Field42 );
            writer.Write( Field43 );
            writer.Write( Field44 );
            writer.Write( Field45 );
            writer.Write( Field46 );
            writer.Write( Field47 );
            writer.Write( Field48 );
            writer.Write( Field49 );
            writer.Write( Field50 );
            writer.Write( Field51 );
            writer.Write( Field52 );
        }

        internal void ReadThird( EndianBinaryReader reader )
        {
            Field28 = reader.ReadInt32();
        }

        internal void WriteThird( EndianBinaryWriter writer )
        {
            writer.Write( Field28 );
        }

        internal void ReadFourth( EndianBinaryReader reader )
        {
            Index = reader.ReadInt32();
            reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
            {
                Field54 = reader.ReadInt32();
                Field55 = reader.ReadInt32();
            } );
        }

        internal void WriteFourth( EndianBinaryWriter writer )
        {
            writer.Write( Index );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                writer.Write( Field54 );
                writer.Write( Field55 );
            } );
        }
    }

    public class StageDatabase : BinaryFile
    {
        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public List<StageEntry> Stages { get; }
        public bool IsFutureTone { get; set; }

        protected override void InternalRead( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                int count = reader.ReadInt32();
                uint section1Offset = reader.ReadUInt32();
                uint section2Offset = reader.ReadUInt32();
                uint section3Offset = reader.ReadUInt32();
                uint section4Offset = reader.ReadUInt32();

                IsFutureTone = ( ( float )( section2Offset - section1Offset ) / count ) == 112;

                reader.ReadAtOffset( section1Offset, () =>
                {
                    Stages.Capacity = count;
                    for ( int i = 0; i < count; i++ )
                    {
                        var stageEntry = new StageEntry();
                        stageEntry.ReadFirst( reader, IsFutureTone );
                        Stages.Add( stageEntry );
                    }
                } );

                reader.ReadAtOffset( section2Offset, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.ReadSecond( reader );
                } );

                reader.ReadAtOffset( section3Offset, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.ReadThird( reader );
                } );

                reader.ReadAtOffset( section4Offset, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.ReadFourth( reader );
                } );
            }
        }

        protected override void InternalWrite( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( Stages.Count );
                writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.WriteFirst( writer, IsFutureTone );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.WriteSecond( writer );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.WriteThird( writer );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
                {
                    foreach ( var stageEntry in Stages )
                        stageEntry.WriteFourth( writer );
                } );
            }
        }

        public StageDatabase()
        {
            Stages = new List<StageEntry>();
        }
    }
}
