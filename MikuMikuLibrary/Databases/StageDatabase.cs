using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    public struct StageObjectInfo
    {
        public uint Id { get; set; }
        public uint SetId { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Id = reader.ReadUInt16();
            SetId = reader.ReadUInt16();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Id );
            writer.Write( SetId );
        }
    }

    // Most of the fields were tested & documented by Stewie1.0
    public class Stage
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
            Stars = 20
        }

        public string Name { get; set; }
        public string Auth3dName { get; set; }
        public StageObjectInfo[] Objects { get; set; }
        public int LensFlareScaleX { get; set; }
        public int LensFlareScaleY { get; set; }
        public int LensFlareScaleZ { get; set; }
        public float Field00 { get; set; }
        public int Field01 { get; set; } // It's always set to 0
        public uint RenderTextureId { get; set; }
        public uint RenderTextureIdFlag { get; set; }
        public uint MovieTextureId { get; set; }
        public uint MovieTextureIdFlag { get; set; }
        public string CollisionFilePath { get; set; } // Unused
        public int Field04 { get; set; }
        public uint Field04Flag { get; set; }
        public int Field05 { get; set; }
        public uint Field05Flag { get; set; }
        public int Field06 { get; set; }
        public uint Field06Flag { get; set; }
        public int Field07 { get; set; }
        public uint Field07Flag { get; set; }
        public int Field08 { get; set; }
        public uint Field08Flag { get; set; }
        public int Field09 { get; set; }
        public int Field10 { get; set; }
        public float RingRectangleX { get; set; }
        public float RingRectangleY { get; set; }
        public float RingRectangleWidth { get; set; }
        public float RingRectangleLength { get; set; }
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
        public StageEffect[] StageEffects { get; set; }
        public uint Id { get; set; }
        public uint UnknownId { get; set; }
        public List<uint> Auth3dIds { get; }
        

        internal void Read( EndianBinaryReader reader, BinaryFormat format )
        {
            if ( format == BinaryFormat.F2nd || format == BinaryFormat.X )
            {
                reader.Seek( 4, SeekOrigin.Current );

                Id = reader.ReadUInt32();
                Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Auth3dName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

                for ( int i = 0; i < 5; i++ )
                {
                    Objects[ i ].Id = reader.ReadUInt32();
                    Objects[ i ].SetId = reader.ReadUInt32();
                }

                Field00 = reader.ReadSingle();
                Field01 = reader.ReadInt32();
                RenderTextureId = reader.ReadUInt32();
                RenderTextureIdFlag = reader.ReadUInt32();
                MovieTextureId = reader.ReadUInt32();
                MovieTextureIdFlag = reader.ReadUInt32();
                Field04 = reader.ReadInt32();
                Field04Flag = reader.ReadUInt32();
                Field05 = reader.ReadInt32();
                Field05Flag = reader.ReadUInt32();
                Field06 = reader.ReadInt32();
                Field06Flag = reader.ReadUInt32();
                Field07 = reader.ReadInt32();
                Field07Flag = reader.ReadUInt32();
                Field08 = reader.ReadInt32();
                Field08Flag = reader.ReadUInt32();
                LensFlareScaleX = reader.ReadInt32();
                LensFlareScaleY = reader.ReadInt32();
                LensFlareScaleZ = reader.ReadInt32();
                RingRectangleX = reader.ReadSingle();
                RingRectangleY = reader.ReadSingle();
                RingRectangleWidth = reader.ReadSingle();
                RingRectangleLength = reader.ReadSingle();
                RingRectangleHeight = reader.ReadSingle();
                UnknownId = reader.ReadUInt32();

                ReadStageEffects( reader );

                int auth3dIdsCount = reader.ReadInt32();
                long auth3dIdsOffset = reader.ReadOffset();

                reader.ReadAtOffset( auth3dIdsOffset, () =>
                {
                    for ( int i = 0; i < auth3dIdsCount; i++ )
                        Auth3dIds.Add( reader.ReadUInt32() );
                });

                reader.Seek( 4, SeekOrigin.Current );
            }
            else
            {
                Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Auth3dName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

                for ( int i = 0; i < 7; i++ )
                {
                    Objects[ i ].Id = reader.ReadUInt16();
                    Objects[ i ].SetId = reader.ReadUInt16();
                }

                LensFlareScaleX = reader.ReadInt32();
                LensFlareScaleY = reader.ReadInt32();
                LensFlareScaleZ = reader.ReadInt32();
                Field00 = reader.ReadSingle();
                Field01 = reader.ReadInt32();
                RenderTextureId = reader.ReadUInt32();

                if ( format > BinaryFormat.DT )
                    MovieTextureId = reader.ReadUInt32();

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
        }

        internal void Write( EndianBinaryWriter writer, BinaryFormat format )
        {
            if ( format == BinaryFormat.F2nd || format == BinaryFormat.X )
            {
                writer.WriteNulls( 4 );
                writer.Write( Id );
                writer.AddStringToStringTable( Name );
                writer.AddStringToStringTable( Auth3dName );

                for ( int i = 0; i < 5; i++ )
                {
                    writer.Write( Objects[ i ].Id );
                    writer.Write( Objects[ i ].SetId );
                }

                writer.Write( Field00 );
                writer.Write( Field01 );
                writer.Write( RenderTextureId );
                writer.Write( RenderTextureIdFlag );
                writer.Write( MovieTextureId );
                writer.Write( MovieTextureIdFlag );
                writer.Write( Field04 );
                writer.Write( Field04Flag );
                writer.Write( Field05 );
                writer.Write( Field05Flag );
                writer.Write( Field06 );
                writer.Write( Field06Flag );
                writer.Write( Field07 );
                writer.Write( Field07Flag );
                writer.Write( Field08 );
                writer.Write( Field08Flag );
                writer.Write( LensFlareScaleX );
                writer.Write( LensFlareScaleY );
                writer.Write( LensFlareScaleZ );
                writer.Write( RingRectangleX );
                writer.Write( RingRectangleY );
                writer.Write( RingRectangleWidth );
                writer.Write( RingRectangleLength );
                writer.Write( RingRectangleHeight );
                writer.Write( UnknownId );

                WriteStageEffects( writer );

                writer.Write( Auth3dIds.Count );

                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
                {
                    foreach ( uint id in Auth3dIds )
                        writer.Write( id );
                } );

                writer.WriteNulls( 4 );
            }
            else
            {
                writer.AddStringToStringTable( Name );
                writer.AddStringToStringTable( Auth3dName );

                for ( int i = 0; i < 7; i++ )
                {
                    writer.Write( ( ushort ) Objects[ i ].Id );
                    writer.Write( ( ushort ) Objects[ i ].SetId );
                }

                writer.Write( LensFlareScaleX );
                writer.Write( LensFlareScaleY );
                writer.Write( LensFlareScaleZ );
                writer.Write( Field00 );
                writer.Write( Field01 );
                writer.Write( RenderTextureId );

                if ( format > BinaryFormat.DT )
                    writer.Write( MovieTextureId );

                writer.AddStringToStringTable( CollisionFilePath );
                writer.Write( Field04 );
                writer.Write( Field05 );

                writer.ScheduleWriteOffsetIf( !(Field06 == 0 && Field07 == 0 && Field08 == 0), 4, AlignmentMode.Left, () =>
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
                writer.Write( RingRectangleWidth) ;
                writer.Write( RingRectangleHeight );
                writer.Write( RingRingHeight );
                writer.Write( RingOutHeight );
            }
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

            for ( int i = 0; i < 16; i++ )
                StageEffects[ i ] = ( StageEffect ) reader.ReadInt32();
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

            foreach ( var stageEffect in StageEffects )
                writer.Write( ( int ) stageEffect );
        }

        internal void ReadAuth3dIds( EndianBinaryReader reader, int count )
        {
            Id = reader.ReadUInt32();

            reader.ReadOffset( () =>
            {
                Auth3dIds.Capacity = count;

                for ( int i = 0; i < count; i++ )
                    Auth3dIds.Add( reader.ReadUInt32() );

                Auth3dIds.Remove( 0xFFFFFFFF );
            } );
        }

        internal void WriteAuth3dIds( EndianBinaryWriter writer )
        {
            writer.Write( Id );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( uint id in Auth3dIds )
                    writer.Write( id );

                writer.Write( -1 );
            } );
        }

        public Stage()
        {
            Objects = new StageObjectInfo[ 7 ];
            StageEffects = new StageEffect[ 16 ];
            Auth3dIds = new List<uint>();
        }
    }

    public class StageDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat ;

        public List<Stage> Stages { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int count = reader.ReadInt32();
            long stagesOffset = reader.ReadOffset();

            if (section != null)
            {
                reader.ReadAtOffset( stagesOffset, () =>
                {
                    Stages.Capacity = count;

                    for ( int i = 0; i < count; i++ )
                    {
                        var stage = new Stage();
                        {
                            stage.Read( reader, Format );
                        }
                        Stages.Add( stage );
                    }

                } );

            }
            else
            {
                long stageEffectsOffset = reader.ReadOffset();
                long auth3dIdCountsOffset = reader.ReadOffset();
                long auth3dIdsOffset = reader.ReadOffset();

                if ( reader.ReadBoolean() )
                {
                    Format = ( BinaryFormat ) reader.ReadByte();
                }

                else
                {
                    long size = ( stageEffectsOffset - stagesOffset ) / count;
                    Format = size == 104 ? BinaryFormat.DT :
                        size == 108 ? BinaryFormat.F :
                        size >= 112 ? BinaryFormat.FT :
                        throw new InvalidDataException();
                }

                reader.ReadAtOffset( stagesOffset, () =>
                {
                    Stages.Capacity = count;

                    for ( int i = 0; i < count; i++ )
                    {
                        var stage = new Stage();
                        {
                            stage.Read( reader, Format );
                        }
                        Stages.Add( stage );
                    }
                } );

                reader.ReadAtOffset( stageEffectsOffset, () =>
                {
                    foreach ( var stage in Stages )
                        stage.ReadStageEffects( reader );
                } );

                reader.ReadAtOffset( auth3dIdCountsOffset, () =>
                {
                   var auth3dIdCounts = reader.ReadInt32s( count );

                   reader.ReadAtOffset(auth3dIdsOffset, () =>
                   {
                       for ( int i = 0; i < count; i++ )
                           Stages[ i ].ReadAuth3dIds( reader, auth3dIdCounts[ i ] );
                   } );
                } );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
            {
                writer.Write( Stages.Count );

                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var stage in Stages )
                        stage.Write( writer, Format );
                } );

            }
            else
            {
                writer.Write( Stages.Count );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var stage in Stages )
                        stage.Write( writer, Format );
                } );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var stage in Stages )
                        stage.WriteStageEffects( writer );
                } );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var stage in Stages )
                        writer.Write( stage.Auth3dIds.Count + 1 );
                } );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var stage in Stages )
                        stage.WriteAuth3dIds( writer );
                } );

                // HACK: We store the format in the reserved part of header.
                writer.Write( true );
                writer.Write( ( byte ) Format );
            }
        }

        public StageDatabase()
        {
            Stages = new List<Stage>();
        }
    }
}