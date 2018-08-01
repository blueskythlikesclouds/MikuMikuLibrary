using MikuMikuLibrary.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.Databases
{
    // Most of the fields were tested & documented by Stewie1.0
    public class StageEntry
    {
        public enum StageEffect
        {
            Empty = -1,
            None = 0,
            // Brighter Stage?
            BrighterStage = 2,
            Snow = 4,
            WaterSplash = 6,
            Rain = 7,
            WaterSplashOnFeet = 12,
            Fog = 16,
            // Reflection on Character(?) (Have to test it in FT)
            ReflectionOnCharacter = 19,
            // This one only works in FT. It crashes in F
            Reflection = 20,
        }

        // Section 1
        public string Name { get; set; }
        public string Auth3DName { get; set; }
        public short ObjectID1 { get; set; }
        public short ObjectIDFlag1 { get; set; }
        public short ObjectID2 { get; set; }
        public short ObjectIDFlag2 { get; set; }
        public short ObjectID3 { get; set; }
        public short ObjectIDFlag3 { get; set; }
        public short ObjectID4 { get; set; }
        public short ObjectIDFlag4 { get; set; }
        public short ObjectID5 { get; set; }
        public short ObjectIDFlag5 { get; set; }
        public short ObjectID6 { get; set; }
        public short ObjectIDFlag6 { get; set; }
        public short ObjectID7 { get; set; }
        public short ObjectIDFlag7 { get; set; }
        public int LensFlareScaleX { get; set; }
        public int LensFlareScaleY { get; set; }
        public int LensFlareScaleZ { get; set; }
        public float Field00 { get; set; }
        public int Field01 { get; set; } // It's always set to 0
        public int Field02 { get; set; }
        public int Field03 { get; set; }
        public string CollisionFilePath { get; set; } // Unused
        public int Field04 { get; set; }
        public int Field05 { get; set; }
        public int Field06 { get; set; }
        public int Field07 { get; set; }
        public int Field08 { get; set; }
        public int Field09 { get; set; }
        public int Field10 { get; set; }
        public float Field11 { get; set; }
        public float Field12 { get; set; }
        public float Field13 { get; set; }
        public float Field14 { get; set; }
        public float Field15 { get; set; }
        // Future Tone Exclusive
        public float Field16 { get; set; }

        // Section 2
        public int Field18 { get; set; }
        public int Field19 { get; set; }
        public int Field20 { get; set; }
        public int Field21 { get; set; }
        public int Field22 { get; set; }
        public int Field23 { get; set; }
        public int Field24 { get; set; }
        public int Field25 { get; set; }
        public StageEffect StageEffect1 { get; set; }
        public StageEffect StageEffect2 { get; set; }
        public StageEffect StageEffect3 { get; set; }
        public StageEffect StageEffect4 { get; set; }
        public StageEffect StageEffect5 { get; set; }
        public StageEffect StageEffect6 { get; set; }
        public StageEffect StageEffect7 { get; set; }
        public StageEffect StageEffect8 { get; set; }
        public StageEffect StageEffect9 { get; set; }
        public StageEffect StageEffect10 { get; set; }
        public StageEffect StageEffect11 { get; set; }
        public StageEffect StageEffect12 { get; set; }
        public StageEffect StageEffect13 { get; set; }
        public StageEffect StageEffect14 { get; set; }
        public StageEffect StageEffect15 { get; set; }
        public StageEffect StageEffect16 { get; set; }

        // Section 3
        public int Field17 { get; set; }

        // Section 4
        public int Index { get; set; }
        public int Field26 { get; set; }
        public int Field27 { get; set; }

        internal void ReadFirst( EndianBinaryReader reader, bool isFutureTone )
        {
            Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Auth3DName = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            ObjectID1 = reader.ReadInt16();
            ObjectIDFlag1 = reader.ReadInt16();
            ObjectID2 = reader.ReadInt16();
            ObjectIDFlag2 = reader.ReadInt16();
            ObjectID3 = reader.ReadInt16();
            ObjectIDFlag3 = reader.ReadInt16();
            ObjectID4 = reader.ReadInt16();
            ObjectIDFlag4 = reader.ReadInt16();
            ObjectID5 = reader.ReadInt16();
            ObjectIDFlag5 = reader.ReadInt16();
            ObjectID6 = reader.ReadInt16();
            ObjectIDFlag6 = reader.ReadInt16();
            ObjectID7 = reader.ReadInt16();
            ObjectIDFlag7 = reader.ReadInt16();
            LensFlareScaleX = reader.ReadInt32();
            LensFlareScaleY = reader.ReadInt32();
            LensFlareScaleZ = reader.ReadInt32();
            Field00 = reader.ReadSingle();
            Field01 = reader.ReadInt32();
            Field02 = reader.ReadInt32();
            Field03 = reader.ReadInt32();
            CollisionFilePath = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            Field04 = reader.ReadInt32();
            Field05 = reader.ReadInt32();
            reader.ReadAtOffsetAndSeekBackIfNotZero( reader.ReadUInt32(), () =>
            {
                Field06 = reader.ReadInt32();
                Field07 = reader.ReadInt32();
                Field08 = reader.ReadInt32();
            } );
            Field09 = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            Field11 = reader.ReadSingle();
            Field12 = reader.ReadSingle();
            Field13 = reader.ReadSingle();
            Field14 = reader.ReadSingle();
            Field15 = reader.ReadSingle();

            if ( isFutureTone )
                Field16 = reader.ReadSingle();
        }

        internal void WriteFirst( EndianBinaryWriter writer, bool isFutureTone )
        {
            writer.AddStringToStringTable( Name );
            writer.AddStringToStringTable( Auth3DName );
            writer.Write( ObjectID1 );
            writer.Write( ObjectIDFlag1 );
            writer.Write( ObjectID2 );
            writer.Write( ObjectIDFlag2 );
            writer.Write( ObjectID3 );
            writer.Write( ObjectIDFlag3 );
            writer.Write( ObjectID4 );
            writer.Write( ObjectIDFlag4 );
            writer.Write( ObjectID5 );
            writer.Write( ObjectIDFlag5 );
            writer.Write( ObjectID6 );
            writer.Write( ObjectIDFlag6 );
            writer.Write( ObjectID7 );
            writer.Write( ObjectIDFlag7 );
            writer.Write( LensFlareScaleX );
            writer.Write( LensFlareScaleY );
            writer.Write( LensFlareScaleZ );
            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
            writer.Write( Field03 );
            writer.AddStringToStringTable( CollisionFilePath );
            writer.Write( Field04 );
            writer.Write( Field05 );
            writer.EnqueueOffsetWriteAlignedIf( !( Field06 == 0 && Field07 == 0 && Field08 == 0 ), 4, AlignmentKind.Left, () =>
            {
                writer.Write( Field06 );
                writer.Write( Field07 );
                writer.Write( Field08 );
            } );
            writer.Write( Field09 );
            writer.Write( Field10 );
            writer.Write( Field11 );
            writer.Write( Field12 );
            writer.Write( Field13 );
            writer.Write( Field14 );
            writer.Write( Field15 );

            if ( isFutureTone )
                writer.Write( Field16 );
        }

        internal void ReadSecond( EndianBinaryReader reader )
        {
            Field18 = reader.ReadInt32();
            Field19 = reader.ReadInt32();
            Field20 = reader.ReadInt32();
            Field21 = reader.ReadInt32();
            Field22 = reader.ReadInt32();
            Field23 = reader.ReadInt32();
            Field24 = reader.ReadInt32();
            Field25 = reader.ReadInt32();
            StageEffect1 = ( StageEffect )reader.ReadInt32();
            StageEffect2 = ( StageEffect )reader.ReadInt32();
            StageEffect3 = ( StageEffect )reader.ReadInt32();
            StageEffect4 = ( StageEffect )reader.ReadInt32();
            StageEffect5 = ( StageEffect )reader.ReadInt32();
            StageEffect6 = ( StageEffect )reader.ReadInt32();
            StageEffect7 = ( StageEffect )reader.ReadInt32();
            StageEffect8 = ( StageEffect )reader.ReadInt32();
            StageEffect9 = ( StageEffect )reader.ReadInt32();
            StageEffect10 = ( StageEffect )reader.ReadInt32();
            StageEffect11 = ( StageEffect )reader.ReadInt32();
            StageEffect12 = ( StageEffect )reader.ReadInt32();
            StageEffect13 = ( StageEffect )reader.ReadInt32();
            StageEffect14 = ( StageEffect )reader.ReadInt32();
            StageEffect15 = ( StageEffect )reader.ReadInt32();
            StageEffect16 = ( StageEffect )reader.ReadInt32();
        }

        internal void WriteSecond( EndianBinaryWriter writer )
        {
            writer.Write( Field18 );
            writer.Write( Field19 );
            writer.Write( Field20 );
            writer.Write( Field21 );
            writer.Write( Field22 );
            writer.Write( Field23 );
            writer.Write( Field24 );
            writer.Write( Field25 );
            writer.Write( ( int )StageEffect1 );
            writer.Write( ( int )StageEffect2 );
            writer.Write( ( int )StageEffect3 );
            writer.Write( ( int )StageEffect4 );
            writer.Write( ( int )StageEffect5 );
            writer.Write( ( int )StageEffect6 );
            writer.Write( ( int )StageEffect7 );
            writer.Write( ( int )StageEffect8 );
            writer.Write( ( int )StageEffect9 );
            writer.Write( ( int )StageEffect10 );
            writer.Write( ( int )StageEffect11 );
            writer.Write( ( int )StageEffect12 );
            writer.Write( ( int )StageEffect13 );
            writer.Write( ( int )StageEffect14 );
            writer.Write( ( int )StageEffect15 );
            writer.Write( ( int )StageEffect16 );
        }

        internal void ReadThird( EndianBinaryReader reader )
        {
            Field17 = reader.ReadInt32();
        }

        internal void WriteThird( EndianBinaryWriter writer )
        {
            writer.Write( Field17 );
        }

        internal void ReadFourth( EndianBinaryReader reader )
        {
            Index = reader.ReadInt32();
            reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
            {
                Field26 = reader.ReadInt32();
                Field27 = reader.ReadInt32();
            } );
        }

        internal void WriteFourth( EndianBinaryWriter writer )
        {
            writer.Write( Index );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                writer.Write( Field26 );
                writer.Write( Field27 );
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

                IsFutureTone = ( ( section2Offset - section1Offset ) / count ) != 108;

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
