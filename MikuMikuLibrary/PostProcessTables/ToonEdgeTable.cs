// Code by Thatrandomlurker
using System;
using System.Collections.Generic;
using System.Text;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.PostProcessTables.ToonEdgeTable
{
    public class ToonSetting
    {
        public string Name { get; set; }
        public uint SettingFlags { get; set; }
        public float ToonShineIntensity { get; set; }
        public float ToonShineFocus { get; set; }
        public float EdgeAR { get; set; }
        public float EdgeAG { get; set; }
        public float EdgeAB { get; set; }
        public float EdgeBR { get; set; }
        public float EdgeBG { get; set; }
        public float EdgeBB { get; set; }
        public float CharaEdgeThickness { get; set; }
        public float StageEdgeThickness { get; set; }
        public float CharaEdgeFace { get; set; }
        public float StageEdgeFace { get; set; }

        internal void Read(EndianBinaryReader reader)
        {
            SettingFlags = reader.ReadUInt32();
            ToonShineIntensity = reader.ReadSingle();
            ToonShineFocus = reader.ReadSingle();
            EdgeAR = reader.ReadSingle();
            EdgeAG = reader.ReadSingle();
            EdgeAB = reader.ReadSingle();
            EdgeBR = reader.ReadSingle();
            EdgeBG = reader.ReadSingle();
            EdgeBB = reader.ReadSingle();
            CharaEdgeThickness = reader.ReadSingle();
            StageEdgeThickness = reader.ReadSingle();
            CharaEdgeFace = reader.ReadSingle();
            StageEdgeFace = reader.ReadSingle();
            Name = "";
        }

        internal void Write(EndianBinaryWriter writer)
        {
            writer.Write( SettingFlags );
            writer.Write( ToonShineIntensity );
            writer.Write( ToonShineFocus );
            writer.Write( EdgeAR );
            writer.Write( EdgeAG );
            writer.Write( EdgeAB );
            writer.Write( EdgeBR );
            writer.Write( EdgeBG );
            writer.Write( EdgeBB );
            writer.Write( CharaEdgeThickness );
            writer.Write( StageEdgeThickness );
            writer.Write( CharaEdgeFace );
            writer.Write( StageEdgeFace );
        }
    }

    public class ToonEdgeTable : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<ToonSetting> ToonEntries { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            uint ToonSettingCount = reader.ReadUInt32();
            if ( Format == BinaryFormat.X )
                reader.SeekCurrent( 4 );
            uint ToonSettingOffset = reader.ReadUInt32();
            if ( Format == BinaryFormat.X )
                reader.SeekCurrent( 4 );

            reader.ReadAtOffset( ToonSettingOffset, () =>
            {
                for ( int i = 0; i < ToonSettingCount; i++ )
                {
                    ToonSetting ToonEntry = new ToonSetting();
                    ToonEntry.Read( reader );
                    ToonEntry.Name = $"Toon Setting { i }";
                    ToonEntries.Add( ToonEntry );
                    if ( Format == BinaryFormat.X )
                    {
                        reader.SeekCurrent( 4 );
                    }
                }
            } );

        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( ToonEntries.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var ToonSetting in ToonEntries )
                {
                    ToonSetting.Write( writer );
                }
            } );
        }

        public ToonEdgeTable()
        {
            ToonEntries = new List<ToonSetting>();
        }
    }
}
