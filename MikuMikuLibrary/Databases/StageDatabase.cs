using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MikuMikuLibrary.Databases
{
    // Most of the fields were tested & documented by Stewie1.0
    public class StageEntry
    {
        public enum StageEffect
        {
            Empty = -1,
            None = 0,
            Leaf = 2,
            Snow = 4,
            WaterSplash = 6,
            Rain = 7,
            WaterSplashOnFeet = 12,
            Fog = 16,
            LightProjection = 19,
            Stars = 20,
        }

        // Section 1
        public string Name { get; set; }
        public string Auth3DName { get; set; }
        public short ObjectID1 { get; set; }
        public short ObjectIDFlag1 { get; set; }
        public short ObjectGroundID { get; set; }
        public short ObjectGroundIDFlag { get; set; }
        public short ObjectID3 { get; set; }
        public short ObjectIDFlag3 { get; set; }
        public short ObjectSkyID { get; set; }
        public short ObjectSkyIDFlag { get; set; }
        public short ObjectID5 { get; set; }
        public short ObjectIDFlag5 { get; set; }
        public short ObjectReflectID { get; set; }
        public short ObjectReflectIDFlag { get; set; }
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
        public float RingRectangleX { get; set; }
        public float RingRectangleY { get; set; }
        public float RingRectangleWidth { get; set; }
        public float RingRectangleHeight { get; set; }
        public float RingRingHeight { get; set; }
        // Future Tone Exclusive
        public float RingOutHeight { get; set; }

        // Section 2
        public int Field11 { get; set; }
        public int Field12 { get; set; }
        public int Field13 { get; set; }
        public int Field14 { get; set; }
        public int Field15 { get; set; }
        public int Field16 { get; set; }
        public int Field17 { get; set; }
        public int Field18 { get; set; }
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
        public int Field19 { get; set; }

        // Section 4
        public List<int> Auth3DIDs { get; }

        internal void ReadFirst( EndianBinaryReader reader, BinaryFormat format )
        {
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Auth3DName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            ObjectID1 = reader.ReadInt16();
            ObjectIDFlag1 = reader.ReadInt16();
            ObjectGroundID = reader.ReadInt16();
            ObjectGroundIDFlag = reader.ReadInt16();
            ObjectID3 = reader.ReadInt16();
            ObjectIDFlag3 = reader.ReadInt16();
            ObjectSkyID = reader.ReadInt16();
            ObjectSkyIDFlag = reader.ReadInt16();
            ObjectID5 = reader.ReadInt16();
            ObjectIDFlag5 = reader.ReadInt16();
            ObjectReflectID = reader.ReadInt16();
            ObjectReflectIDFlag = reader.ReadInt16();
            ObjectID7 = reader.ReadInt16();
            ObjectIDFlag7 = reader.ReadInt16();
            LensFlareScaleX = reader.ReadInt32();
            LensFlareScaleY = reader.ReadInt32();
            LensFlareScaleZ = reader.ReadInt32();
            Field00 = reader.ReadSingle();
            Field01 = reader.ReadInt32();
            Field02 = reader.ReadInt32();
            Field03 = reader.ReadInt32();
            CollisionFilePath = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field04 = reader.ReadInt32();
            Field05 = reader.ReadInt32();
            reader.ReadAtOffset( reader.ReadUInt32(), () =>
            {
                Field06 = reader.ReadInt32();
                Field07 = reader.ReadInt32();
                Field08 = reader.ReadInt32();
            } );
            Field09 = reader.ReadInt32();
            Field10 = reader.ReadInt32();
            RingRectangleX = reader.ReadSingle();
            RingRectangleY = reader.ReadSingle();
            RingRectangleWidth = reader.ReadSingle();
            RingRectangleHeight = reader.ReadSingle();
            RingRingHeight = reader.ReadSingle();

            if ( format == BinaryFormat.FT )
                RingOutHeight = reader.ReadSingle();
        }

        internal void WriteFirst( EndianBinaryWriter writer, BinaryFormat format )
        {
            writer.AddStringToStringTable( Name );
            writer.AddStringToStringTable( Auth3DName );
            writer.Write( ObjectID1 );
            writer.Write( ObjectIDFlag1 );
            writer.Write( ObjectGroundID );
            writer.Write( ObjectGroundIDFlag );
            writer.Write( ObjectID3 );
            writer.Write( ObjectIDFlag3 );
            writer.Write( ObjectSkyID );
            writer.Write( ObjectSkyIDFlag );
            writer.Write( ObjectID5 );
            writer.Write( ObjectIDFlag5 );
            writer.Write( ObjectReflectID );
            writer.Write( ObjectReflectIDFlag );
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
            writer.EnqueueOffsetWriteIf( !( Field06 == 0 && Field07 == 0 && Field08 == 0 ), 4, AlignmentKind.Left, () =>
            {
                writer.Write( Field06 );
                writer.Write( Field07 );
                writer.Write( Field08 );
            } );
            writer.Write( Field09 );
            writer.Write( Field10 );
            writer.Write( RingRectangleX );
            writer.Write( RingRectangleY );
            writer.Write( RingRectangleWidth );
            writer.Write( RingRectangleHeight );
            writer.Write( RingRingHeight );

            if ( format == BinaryFormat.FT )
                writer.Write( RingOutHeight );
        }

        internal void ReadSecond( EndianBinaryReader reader )
        {
            Field11 = reader.ReadInt32();
            Field12 = reader.ReadInt32();
            Field13 = reader.ReadInt32();
            Field14 = reader.ReadInt32();
            Field15 = reader.ReadInt32();
            Field16 = reader.ReadInt32();
            Field17 = reader.ReadInt32();
            Field18 = reader.ReadInt32();
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
            writer.Write( Field11 );
            writer.Write( Field12 );
            writer.Write( Field13 );
            writer.Write( Field14 );
            writer.Write( Field15 );
            writer.Write( Field16 );
            writer.Write( Field17 );
            writer.Write( Field18 );
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
            Field19 = reader.ReadInt32();
        }

        internal void WriteThird( EndianBinaryWriter writer )
        {
            writer.Write( Field19 );
        }

        internal void ReadFourth( EndianBinaryReader reader )
        {
            reader.ReadAtOffset( reader.ReadUInt32(), () =>
            {
                int id;
                while ( ( id = reader.ReadInt32() ) >= 0 )
                    Auth3DIDs.Add( id );
            } );
        }

        internal void WriteFourth( EndianBinaryWriter writer )
        {
            writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
            {
                foreach ( var id in Auth3DIDs )
                    writer.Write( id );

                writer.Write( -1 );
            } );
        }

        public StageEntry()
        {
            Auth3DIDs = new List<int>();
        }
    }

    public class StageDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save; }
        }

        public List<StageEntry> Stages { get; }
        public bool IsFutureTone { get; set; }

        public override void Read( EndianBinaryReader reader, Section section = null )
        {
            int count = reader.ReadInt32();
            uint section1Offset = reader.ReadUInt32();
            uint section2Offset = reader.ReadUInt32();
            uint section3Offset = reader.ReadUInt32();
            uint section4Offset = reader.ReadUInt32();

            Format = ( ( section2Offset - section1Offset ) / count ) != 108 ? BinaryFormat.FT : BinaryFormat.F;

            reader.ReadAtOffset( section1Offset, () =>
            {
                Stages.Capacity = count;
                for ( int i = 0; i < count; i++ )
                {
                    var stageEntry = new StageEntry();
                    stageEntry.ReadFirst( reader, Format );
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
                for ( int i = 0; i < Stages.Count; i++ )
                {
                    int index = reader.ReadInt32();
                    Debug.Assert( i == index );
                    Stages[ index ].ReadFourth( reader );
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( Stages.Count );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.WriteFirst( writer, Format );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.WriteSecond( writer );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.WriteThird( writer );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () =>
            {
                for ( int i = 0; i < Stages.Count; i++ )
                {
                    writer.Write( i );
                    Stages[ i ].WriteFourth( writer );
                }
            } );
        }

        public StageDatabase()
        {
            Stages = new List<StageEntry>();
        }
    }
}
