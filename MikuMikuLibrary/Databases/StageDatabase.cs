using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

        public string Name { get; set; }
        public string Auth3dName { get; set; }
        public short ObjectId1 { get; set; }
        public short ObjectIdFlag1 { get; set; }
        public short ObjectGroundId { get; set; }
        public short ObjectGroundIdFlag { get; set; }
        public short ObjectId3 { get; set; }
        public short ObjectIdFlag3 { get; set; }
        public short ObjectSkyId { get; set; }
        public short ObjectSkyIdFlag { get; set; }
        public short ObjectId5 { get; set; }
        public short ObjectIdFlag5 { get; set; }
        public short ObjectReflectId { get; set; }
        public short ObjectReflectIdFlag { get; set; }
        public short ObjectId7 { get; set; }
        public short ObjectIdFlag7 { get; set; }
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
        public float RingOutHeight { get; set; }
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
        public int Id { get; set; }
        public List<int> Auth3dIds { get; }
        
        internal void Read( EndianBinaryReader reader, BinaryFormat format )
        {
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Auth3dName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            ObjectId1 = reader.ReadInt16();
            ObjectIdFlag1 = reader.ReadInt16();
            ObjectGroundId = reader.ReadInt16();
            ObjectGroundIdFlag = reader.ReadInt16();
            ObjectId3 = reader.ReadInt16();
            ObjectIdFlag3 = reader.ReadInt16();
            ObjectSkyId = reader.ReadInt16();
            ObjectSkyIdFlag = reader.ReadInt16();
            ObjectId5 = reader.ReadInt16();
            ObjectIdFlag5 = reader.ReadInt16();
            ObjectReflectId = reader.ReadInt16();
            ObjectReflectIdFlag = reader.ReadInt16();
            ObjectId7 = reader.ReadInt16();
            ObjectIdFlag7 = reader.ReadInt16();
            LensFlareScaleX = reader.ReadInt32();
            LensFlareScaleY = reader.ReadInt32();
            LensFlareScaleZ = reader.ReadInt32();
            Field00 = reader.ReadSingle();
            Field01 = reader.ReadInt32();
            Field02 = reader.ReadInt32();
            if ( format > BinaryFormat.DT )
                Field03 = reader.ReadInt32();
            CollisionFilePath = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field04 = reader.ReadInt32();
            Field05 = reader.ReadInt32();
            reader.ReadOffset( () =>
            {
                Field06 = reader.ReadInt32();
                Field07 = reader.ReadInt32();
                Field08 = reader.ReadInt32();
            } );
            Field09 = reader.ReadInt32();
            if ( format == BinaryFormat.FT )
                Field10 = reader.ReadInt32();
            RingRectangleX = reader.ReadSingle();
            RingRectangleY = reader.ReadSingle();
            RingRectangleWidth = reader.ReadSingle();
            RingRectangleHeight = reader.ReadSingle();
            RingRingHeight = reader.ReadSingle();
            RingOutHeight = reader.ReadSingle();
        }

        internal void Write( EndianBinaryWriter writer, BinaryFormat format )
        {
            writer.AddStringToStringTable( Name );
            writer.AddStringToStringTable( Auth3dName );
            writer.Write( ObjectId1 );
            writer.Write( ObjectIdFlag1 );
            writer.Write( ObjectGroundId );
            writer.Write( ObjectGroundIdFlag );
            writer.Write( ObjectId3 );
            writer.Write( ObjectIdFlag3 );
            writer.Write( ObjectSkyId );
            writer.Write( ObjectSkyIdFlag );
            writer.Write( ObjectId5 );
            writer.Write( ObjectIdFlag5 );
            writer.Write( ObjectReflectId );
            writer.Write( ObjectReflectIdFlag );
            writer.Write( ObjectId7 );
            writer.Write( ObjectIdFlag7 );
            writer.Write( LensFlareScaleX );
            writer.Write( LensFlareScaleY );
            writer.Write( LensFlareScaleZ );
            writer.Write( Field00 );
            writer.Write( Field01 );
            writer.Write( Field02 );
            if ( format > BinaryFormat.DT )
                writer.Write( Field03 );
            writer.AddStringToStringTable( CollisionFilePath );
            writer.Write( Field04 );
            writer.Write( Field05 );
            writer.ScheduleWriteOffsetIf( !( Field06 == 0 && Field07 == 0 && Field08 == 0 ), 4, AlignmentMode.Left, () =>
            {
                writer.Write( Field06 );
                writer.Write( Field07 );
                writer.Write( Field08 );
            } );
            writer.Write( Field09 );
            if ( format == BinaryFormat.FT )
                writer.Write( Field10 );
            writer.Write( RingRectangleX );
            writer.Write( RingRectangleY );
            writer.Write( RingRectangleWidth );
            writer.Write( RingRectangleHeight );
            writer.Write( RingRingHeight );
            writer.Write( RingOutHeight );
        }

        internal void ReadStageEffects( EndianBinaryReader reader )
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

        internal void WriteStageEffects( EndianBinaryWriter writer )
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

        internal void ReadAuth3dIds( EndianBinaryReader reader, int count )
        {
            Id = reader.ReadInt32();
            
            reader.ReadOffset( () =>
            {
                Auth3dIds.Capacity = count;
                for ( int i = 0; i < count; i++ )
                    Auth3dIds.Add( reader.ReadInt32() );
                    
                Auth3dIds.Remove( -1 );
            } );
        }

        internal void WriteAuth3dIds( EndianBinaryWriter writer )
        {
            writer.Write( Id );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var id in Auth3dIds )
                    writer.Write( id );

                writer.Write( -1 );
            } );
        }

        public StageEntry()
        {
            Auth3dIds = new List<int>();
        }
    }

    public class StageDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save;

        public List<StageEntry> Stages { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int count = reader.ReadInt32();
            long stagesOffset = reader.ReadOffset();
            long stageEffectsOffset = reader.ReadOffset();
            long auth3dIdCountsOffset = reader.ReadOffset();
            long auth3dIdsOffset = reader.ReadOffset();
            
            if ( reader.ReadBoolean() )
            {
                Format = ( BinaryFormat ) reader.ReadByte();
            }
            else
            {
                long entrySize = ( stageEffectsOffset - stagesOffset ) / count;
                Format = entrySize == 104 ? BinaryFormat.DT :
                         entrySize == 108 ? BinaryFormat.F :
                         entrySize >= 112 ? BinaryFormat.FT :
                         throw new InvalidDataException();
            }
                     
            reader.ReadAtOffset( stagesOffset, () =>
            {
                Stages.Capacity = count;
                for ( int i = 0; i < count; i++ )
                {
                    var stageEntry = new StageEntry();
                    {
                        stageEntry.Read( reader, Format );
                    }
                    Stages.Add( stageEntry );
                }
            } );

            reader.ReadAtOffset( stageEffectsOffset, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.ReadStageEffects( reader );
            } );
            
            reader.ReadAtOffset( auth3dIdCountsOffset, () =>
            {
                var auth3dIdCounts = reader.ReadInt32s( count );
                reader.ReadAtOffset( auth3dIdsOffset, () =>
                {
                    for ( int i = 0; i < count; i++ )
                        Stages[ i ].ReadAuth3dIds( reader, auth3dIdCounts[ i ] );
                } );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( Stages.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.Write( writer, Format );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.WriteStageEffects( writer );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    writer.Write( stageEntry.Auth3dIds.Count + 1 );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var stageEntry in Stages )
                    stageEntry.WriteAuth3dIds( writer );
            } );
            
            // HACK: We store the format in the reserved part of header.
            writer.Write( true );
            writer.Write( ( byte ) Format );
        }

        public StageDatabase()
        {
            Stages = new List<StageEntry>();
        }
    }
}
