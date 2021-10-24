using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    [Flags]
    public enum StageFlags
    {
        Flag0 = 1 << 0,
        LightCharaAmbient = 1 << 1,
        Flag2 = 1 << 2,
        Flag3 = 1 << 3,
    };

    public enum StageReflectBlurFilter
    {
        Filter4 = 0,
        Filter9 = 1,
        Filter16 = 2,
        Filter32 = 3,
    };

    public enum StageReflectRefractResolution
    {
        Resolution256x256 = 0,
        Resolution512x256 = 1,
        Resolution512x512 = 2,
    };

    public class StageReflect
    {
        public StageReflectRefractResolution ResolutionMode { get; set; }
        public int BlurNum { get; set; }
        public StageReflectBlurFilter BlurFilter { get; set; }

        internal static StageReflect Read( EndianBinaryReader reader )
        {
            StageReflect reflect = new StageReflect();
            reflect.ResolutionMode = ( StageReflectRefractResolution ) reader.ReadInt32();
            reflect.BlurNum = reader.ReadInt32();
            reflect.BlurFilter = ( StageReflectBlurFilter ) reader.ReadInt32();
            return reflect;
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( int ) ResolutionMode );
            writer.Write( BlurNum );
            writer.Write( ( int ) BlurFilter );
        }
    }
    
    public class StageRefract
    {
        public StageReflectRefractResolution Mode { get; set; }

        internal static StageRefract Read( EndianBinaryReader reader )
        {
            StageRefract refract = new StageRefract();
            refract.Mode = ( StageReflectRefractResolution ) reader.ReadInt32();
            return refract;
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( ( int ) Mode );
        }
    }

    public struct StageObjectInfo
    {
        public uint Id { get; set; }
        public uint SetId { get; set; }

        internal static StageObjectInfo ReadClassic( EndianBinaryReader reader )
        {
            StageObjectInfo objectInfo = default;
            objectInfo.Id = reader.ReadUInt16();
            objectInfo.SetId = reader.ReadUInt16();
            return objectInfo;
        }

        internal void WriteClassic( EndianBinaryWriter writer )
        {
            writer.Write( ( ushort ) Id );
            writer.Write( ( ushort ) SetId );
        }

        internal static StageObjectInfo ReadModern( EndianBinaryReader reader )
        {
            StageObjectInfo objectInfo = default;
            objectInfo.SetId = reader.ReadUInt32();
            objectInfo.Id = reader.ReadUInt32();
            return objectInfo;
        }

        internal void WriteModern( EndianBinaryWriter writer )
        {
            writer.Write( SetId );
            writer.Write( Id );
        }
    }

    public struct StageObjects
    {
        public StageObjectInfo Ground { get; set; }
        public StageObjectInfo Unknown { get; set; }
        public StageObjectInfo Sky { get; set; }
        public StageObjectInfo Shadow { get; set; }
        public StageObjectInfo Reflect { get; set; }
        public StageObjectInfo Refract { get; set; }

        internal static StageObjects ReadClassic( EndianBinaryReader reader )
        {
            StageObjects objects = default;
            objects.Ground = StageObjectInfo.ReadClassic( reader );
            objects.Unknown = StageObjectInfo.ReadClassic( reader );
            objects.Sky = StageObjectInfo.ReadClassic( reader );
            objects.Shadow = StageObjectInfo.ReadClassic( reader );
            objects.Reflect = StageObjectInfo.ReadClassic( reader );
            objects.Refract = StageObjectInfo.ReadClassic( reader );
            return objects;
        }

        internal void WriteClassic( EndianBinaryWriter writer )
        {
            Ground.WriteClassic( writer );
            Unknown.WriteClassic( writer );
            Sky.WriteClassic( writer );
            Shadow.WriteClassic( writer );
            Reflect.WriteClassic( writer );
            Refract.WriteClassic( writer );
        }

        internal static StageObjects ReadModern( EndianBinaryReader reader )
        {
            StageObjects objects = default;
            objects.Ground = StageObjectInfo.ReadModern( reader );
            objects.Sky = StageObjectInfo.ReadModern( reader );
            objects.Shadow = StageObjectInfo.ReadModern( reader );
            objects.Reflect = StageObjectInfo.ReadModern( reader );
            objects.Refract = StageObjectInfo.ReadModern( reader );
            return objects;
        }

        internal void WriteModern( EndianBinaryWriter writer )
        {
            Ground.WriteModern( writer );
            Sky.WriteModern( writer );
            Shadow.WriteModern( writer );
            Reflect.WriteModern( writer );
            Refract.WriteModern( writer );
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
        public uint ObjectSetID { get; set; }
        public StageObjects Objects { get; set; }
        public uint LensFlareTexture { get; set; }
        public uint LensShaftTexture { get; set; }
        public uint LensGhostTexture { get; set; }
        public float LensShaftInvScale { get; set; }
        public uint Field00 { get; set; } // It's always set to 0
        public uint RenderTextureId { get; set; }
        public uint RenderTextureIdFlag { get; set; }
        public uint MovieTextureId { get; set; }
        public uint MovieTextureIdFlag { get; set; }
        public string CollisionFilePath { get; set; } // Unused
        public uint ReflectType { get; set; }
        public uint ReflectTypeFlag { get; set; }
        public bool RefractEnable { get; set; }
        public uint RefractEnableFlag { get; set; }
        public StageReflect Reflect { get; set; }
        public StageRefract Refract { get; set; }
        public StageFlags Flags { get; set; }
        public float RingRectangleX { get; set; }
        public float RingRectangleY { get; set; }
        public float RingRectangleWidth { get; set; }
        public float RingRectangleLength { get; set; }
        public float RingRectangleHeight { get; set; }
        public float RingRingHeight { get; set; }
        public float RingOutHeight { get; set; }
        public uint Field04 { get; set; }
        public uint Field04Flag { get; set; }
        public uint Field05 { get; set; }
        public uint Field05Flag { get; set; }
        public uint Field06 { get; set; }
        public uint Field06Flag { get; set; }
        public uint Field07 { get; set; }
        public uint Field07Flag { get; set; }
        public uint Field08 { get; set; }
        public uint Field09 { get; set; }
        public uint Field10 { get; set; }
        public long Field11 { get; set; }
        public long Field12 { get; set; }
        public uint Field13 { get; set; }
        public uint StageEffectField01 { get; set; }
        public uint StageEffectField02 { get; set; }
        public uint StageEffectField03 { get; set; }
        public uint StageEffectField04 { get; set; }
        public uint StageEffectField05 { get; set; }
        public uint StageEffectField06 { get; set; }
        public uint StageEffectField07 { get; set; }
        public uint StageEffectField08 { get; set; }
        public StageEffect[] StageEffects { get; set; }
        public uint Id { get; set; }
        public uint UnknownId { get; set; }
        public List<uint> Auth3dIds { get; }

        internal void Read( EndianBinaryReader reader, BinaryFormat format )
        {
            if ( format == BinaryFormat.F2nd || format == BinaryFormat.X )
            {
                Id = ( uint ) reader.ReadUInt64();
                Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Auth3dName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Objects = StageObjects.ReadModern( reader );
                LensShaftInvScale = reader.ReadSingle();
                Field00 = reader.ReadUInt32();
                RenderTextureId = reader.ReadUInt32();
                RenderTextureIdFlag = reader.ReadUInt32();
                MovieTextureId = reader.ReadUInt32();
                MovieTextureIdFlag = reader.ReadUInt32();
                Field04 = reader.ReadUInt32();
                Field04Flag = reader.ReadUInt32();
                Field05 = reader.ReadUInt32();
                Field05Flag = reader.ReadUInt32();
                Field06 = reader.ReadUInt32();
                Field06Flag = reader.ReadUInt32();
                Field07 = reader.ReadUInt32();
                Field07Flag = reader.ReadUInt32();
                Field08 = reader.ReadUInt32();
                Field09 = reader.ReadUInt32();
                Field10 = reader.ReadUInt32();

                if ( format == BinaryFormat.X )
                    reader.Seek( 4, SeekOrigin.Current );

                Field11 = reader.ReadOffset();
                Field12 = reader.ReadOffset();
                RingRectangleX = reader.ReadSingle();
                RingRectangleY = reader.ReadSingle();
                RingRectangleWidth = reader.ReadSingle();
                RingRectangleHeight = reader.ReadSingle();
                RingRingHeight = reader.ReadSingle();
                RingOutHeight = reader.ReadSingle();

                if ( format == BinaryFormat.X )
                    Field13 = reader.ReadUInt32();

                ReadStageEffects( reader );
                
                if ( format == BinaryFormat.X )
                    reader.Seek( 4, SeekOrigin.Current );

                uint auth3dIdsCount = reader.ReadUInt32();
                long auth3dIdsOffset = reader.ReadOffset();

                reader.ReadAtOffset( auth3dIdsOffset, () =>
                {
                    for ( int i = 0; i < auth3dIdsCount; i++ )
                        Auth3dIds.Add( reader.ReadUInt32() );
                });

                if ( format != BinaryFormat.X )
                    reader.Seek( 4, SeekOrigin.Current );
            }
            else
            {
                Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Auth3dName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                ObjectSetID = reader.ReadUInt32();
                Objects = StageObjects.ReadClassic( reader );
                LensFlareTexture = reader.ReadUInt32();
                LensShaftTexture = reader.ReadUInt32();
                LensGhostTexture = reader.ReadUInt32();
                LensShaftInvScale = reader.ReadSingle();
                Field00 = reader.ReadUInt32();
                RenderTextureId = reader.ReadUInt32();

                if ( format > BinaryFormat.DT )
                    MovieTextureId = reader.ReadUInt32();

                CollisionFilePath = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                ReflectType = reader.ReadUInt32();
                RefractEnable = reader.ReadUInt32() != 0;

                reader.ReadAtOffset( reader.ReadInt32(),
                    () => Reflect = StageReflect.Read( reader ) );
                
                reader.ReadAtOffset( reader.ReadInt32(),
                    () => Refract = StageRefract.Read( reader ) );

                if ( format == BinaryFormat.FT )
                    Flags = (StageFlags)reader.ReadUInt32();

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
                writer.Align( 8 );
                writer.Write( ( ulong ) Id );
                writer.AddStringToStringTable( Name );
                writer.AddStringToStringTable( Auth3dName );
                Objects.WriteModern( writer );
                writer.Write( LensShaftInvScale );
                writer.Write( Field00 );
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
                writer.Write( Field09 );
                writer.Write( Field10 );

                if ( format == BinaryFormat.X )
                    writer.WriteNulls( 4 );

                writer.WriteOffset( Field11 );
                writer.WriteOffset( Field12 );
                writer.Write( RingRectangleX );
                writer.Write( RingRectangleY );
                writer.Write( RingRectangleWidth );
                writer.Write( RingRectangleHeight );
                writer.Write( RingRingHeight );
                writer.Write( RingOutHeight );

                if ( format == BinaryFormat.X )
                    writer.Write( Field13 );

                WriteStageEffects( writer );
                
                if ( format == BinaryFormat.X )
                    writer.WriteNulls( 4 );

                writer.Write( Auth3dIds.Count );

                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
                {
                    foreach ( uint id in Auth3dIds )
                        writer.Write( id );
                    writer.Align( 16 );
                } );
                
                if ( format != BinaryFormat.X )
                    writer.WriteNulls( 4 );
            }
            else
            {
                writer.AddStringToStringTable( Name );
                writer.AddStringToStringTable( Auth3dName );
                writer.Write( ObjectSetID );
                Objects.WriteClassic( writer );
                writer.Write( LensFlareTexture );
                writer.Write( LensShaftTexture );
                writer.Write( LensGhostTexture );
                writer.Write( LensShaftInvScale );
                writer.Write( Field00 );
                writer.Write( RenderTextureId );

                if ( format > BinaryFormat.DT )
                    writer.Write( MovieTextureId );

                writer.AddStringToStringTable( CollisionFilePath );
                writer.Write( ReflectType );
                writer.Write( RefractEnable ? 1u : 0u );
                
                writer.ScheduleWriteOffsetIf( Reflect != null && Reflect.BlurNum != 0 , 4, AlignmentMode.Left,
                    () => Reflect.Write( writer ) );
                
                writer.ScheduleWriteOffsetIf( Refract != null , 4, AlignmentMode.Left,
                    () => Refract.Write( writer ) );

                if ( format == BinaryFormat.FT )
                    writer.Write( ( int ) Flags );

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
            StageEffectField01 = reader.ReadUInt32();
            StageEffectField02 = reader.ReadUInt32();
            StageEffectField03 = reader.ReadUInt32();
            StageEffectField04 = reader.ReadUInt32();
            StageEffectField05 = reader.ReadUInt32();
            StageEffectField06 = reader.ReadUInt32();
            StageEffectField07 = reader.ReadUInt32();
            StageEffectField08 = reader.ReadUInt32();

            for ( int i = 0; i < 16; i++ )
                StageEffects[ i ] = ( StageEffect ) reader.ReadInt32();
        }

        internal void WriteStageEffects( EndianBinaryWriter writer )
        {
            writer.Write( StageEffectField01 );
            writer.Write( StageEffectField02 );
            writer.Write( StageEffectField03 );
            writer.Write( StageEffectField04 );
            writer.Write( StageEffectField05 );
            writer.Write( StageEffectField06 );
            writer.Write( StageEffectField07 );
            writer.Write( StageEffectField08 );

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