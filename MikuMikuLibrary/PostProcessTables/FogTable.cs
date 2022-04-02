// Code by Thatrandomlurker
using System;
using System.Collections.Generic;
using System.Text;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using Color = MikuMikuLibrary.Misc.Color;

namespace MikuMikuLibrary.PostProcessTables.FogTable
{
    public class FogSetting
    {
        public string Name { get; set; }
        public uint Unk1 { get; set; }
        public uint Type { get; set; }
        public float Group0Density { get; set; }
        public float Group0LinStart { get; set; }
        public float Group0LinEnd { get; set; }
        public float Group1Density { get; set; }
        public float Group1LinStart { get; set; }
        public float Group1LinEnd { get; set; }
        public Color Group0Color { get; set; }
        public float Unk2 { get; set; }
        public float Unk3 { get; set; }
        public float Unk4 { get; set; }
        public float Unk5 { get; set; }
        public Color Group1Color { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Unk1 = reader.ReadUInt32();
            Type = reader.ReadUInt32();
            Group0Density = reader.ReadSingle();
            Group0LinStart = reader.ReadSingle();
            Group0LinEnd = reader.ReadSingle();
            Group1Density = reader.ReadSingle();
            Group1LinStart = reader.ReadSingle();
            Group1LinEnd = reader.ReadSingle();
            Group0Color = reader.ReadColor();
            Unk2 = reader.ReadSingle();
            Unk3 = reader.ReadSingle();
            Unk4 = reader.ReadSingle();
            Unk5 = reader.ReadSingle();
            Group1Color = reader.ReadColor();
            Name = "";
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Unk1 );
            writer.Write( Type );
            writer.Write( Group0Density );
            writer.Write( Group0LinStart );
            writer.Write( Group0LinEnd );
            writer.Write( Group1Density );
            writer.Write( Group1LinStart );
            writer.Write( Group1LinEnd );
            writer.Write( Group0Color );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
            writer.Write( Unk5 );
            writer.Write( Group1Color );
        }
    }

    public class FogTable : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<FogSetting> FogEntries { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            uint FogSettingCount = reader.ReadUInt32();
            if ( Format == BinaryFormat.X )
                reader.SeekCurrent( 4 );
            uint FogSettingOffset = reader.ReadUInt32();
            if ( Format == BinaryFormat.X )
                reader.SeekCurrent( 4 );

            reader.ReadAtOffset( FogSettingOffset, () =>
            {
                for ( int i = 0; i < FogSettingCount; i++ )
                {
                    FogSetting FogEntry = new FogSetting();
                    FogEntry.Read( reader );
                    FogEntry.Name = $"Fog Setting { i }";
                    FogEntries.Add( FogEntry );
                }
            } );

        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( FogEntries.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var FogSetting in FogEntries )
                {
                    FogSetting.Write( writer );
                }
            } );
        }

        public FogTable()
        {
            FogEntries = new List<FogSetting>();
        }
    }
}
