using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class ClothBlock : IBlock
    {
        public string Signature => "CLS";

        public string Field00 { get; set; }
        public string Field04 { get; set; }
        public uint Field08 { get; set; }
        public uint Field10 { get; set; }
        public uint Field14 { get; set; }
        public List<Vector3> Field18 { get; set; }
        public List<Field1CData> Field1C { get; set; }
        public List<Field20Data> Field20 { get; set; }
        public ushort[] Field24 { get; set; }
        public ushort[] Field28 { get; set; }
        public uint Field2C { get; set; }
        public uint Field30 { get; set; }

        public void Read( EndianBinaryReader reader, StringSet stringSet )
        {
            Field00 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field04 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Field08 = reader.ReadUInt32();
            int count = reader.ReadInt32();
            Field10 = reader.ReadUInt32();
            Field14 = reader.ReadUInt32();

            reader.ReadOffset( () =>
            {
                Field18.Capacity = count;

                for ( int i = 0; i < count; i++ )
                    Field18.Add( reader.ReadVector3() );
            } );

            reader.ReadOffset( () =>
            {
                Field1C.Capacity = count;

                for ( int i = 0; i < count; i++ )
                {
                    var data = new Field1CData();
                    data.Read( reader );
                    Field1C.Add( data );
                }
            } );           
            
            reader.ReadOffset( () =>
            {
                Field20.Capacity = count;

                for ( int i = 0; i < count; i++ )
                {
                    var data = new Field20Data();
                    data.Read( reader );
                    Field20.Add( data );
                }
            } );

            reader.ReadOffset( () =>
            {
                Field24 = reader.ReadUInt16s( reader.ReadUInt16() );
            } );            
            
            reader.ReadOffset( () =>
            {
                Field28 = reader.ReadUInt16s( reader.ReadUInt16() );
            } );

            Field2C = reader.ReadUInt32();
            Field30 = reader.ReadUInt32();
        }

        public void Write( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( Field00 );
            writer.AddStringToStringTable( Field04 );
            writer.Write( Field08 );
            writer.Write( Field18.Count );
            writer.Write( Field10 );
            writer.Write( Field14 );
            
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in Field18 )
                    writer.Write( value );
            } );            
            
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in Field1C )
                    value.Write( writer );
            } );

            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var value in Field20 )
                    value.Write( writer );
            } );

            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                writer.Write( ( ushort ) Field24.Length );
                writer.Write( Field24 );
            } );           
            
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                writer.Write( ( ushort ) Field28.Length );
                writer.Write( Field28 );
            } );

            writer.Write( Field2C );
            writer.Write( Field30 );
        }

        public ClothBlock()
        {
            Field18 = new List<Vector3>();
            Field1C = new List<Field1CData>();
            Field20 = new List<Field20Data>();
        }

        public class Field1CData
        {
            public struct SubData
            {
                public string Field00;
                public float Field04;
                public uint Field08;
                public uint Field0C;

                internal void Read( EndianBinaryReader reader )
                {
                    Field00 = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                    Field04 = reader.ReadSingle();
                    Field08 = reader.ReadUInt32();
                    Field0C = reader.ReadUInt32();
                }

                internal void Write( EndianBinaryWriter writer )
                {
                    writer.AddStringToStringTable( Field00 );
                    writer.Write( Field04 );
                    writer.Write( Field08 );
                    writer.Write( Field0C );
                }
            }

            public float Field00;
            public float Field04;
            public float Field08;
            public float Field0C;
            public float Field10;
            public float Field14;
            public float Field18;
            public uint Field1C;
            public uint Field20;
            public uint Field24;
            public SubData SubData0;
            public SubData SubData1;
            public SubData SubData2;
            public SubData SubData3;

            internal void Read( EndianBinaryReader reader )
            {
                Field00 = reader.ReadSingle();
                Field04 = reader.ReadSingle();
                Field08 = reader.ReadSingle();
                Field0C = reader.ReadSingle();
                Field10 = reader.ReadSingle();
                Field14 = reader.ReadSingle();
                Field18 = reader.ReadSingle();
                Field1C = reader.ReadUInt32();
                Field20 = reader.ReadUInt32();
                Field24 = reader.ReadUInt32();
                SubData0.Read( reader );
                SubData1.Read( reader );
                SubData2.Read( reader );
                SubData3.Read( reader );
            }

            internal void Write( EndianBinaryWriter writer )
            {
                writer.Write( Field00 );
                writer.Write( Field04 );
                writer.Write( Field08 );
                writer.Write( Field0C );
                writer.Write( Field10 );
                writer.Write( Field14 );
                writer.Write( Field18 );
                writer.Write( Field1C );
                writer.Write( Field20 );
                writer.Write( Field24 );
                SubData0.Write( writer );
                SubData1.Write( writer );
                SubData2.Write( writer );
                SubData3.Write( writer );
            }
        }

        public class Field20Data
        {
            public struct SubData
            {
                public uint Field00;
                public float Field04;
                public float Field08;
                public float Field0C;
                public float Field10;
                public float Field14;
                public float Field18;
                public float Field1C;
                public float Field20;
                public float Field24;
                public float Field28;

                internal void Read( EndianBinaryReader reader )
                {
                    Field00 = reader.ReadUInt32();
                    Field04 = reader.ReadSingle();
                    Field08 = reader.ReadSingle();
                    Field0C = reader.ReadSingle();
                    Field10 = reader.ReadSingle();
                    Field14 = reader.ReadSingle();
                    Field18 = reader.ReadSingle();
                    Field1C = reader.ReadSingle();
                    Field20 = reader.ReadSingle();
                    Field24 = reader.ReadSingle();
                    Field28 = reader.ReadSingle();
                }

                internal void Write( EndianBinaryWriter writer )
                {
                    writer.Write( Field00 );
                    writer.Write( Field04 );
                    writer.Write( Field08 );
                    writer.Write( Field0C );
                    writer.Write( Field10 );
                    writer.Write( Field14 );
                    writer.Write( Field18 );
                    writer.Write( Field1C );
                    writer.Write( Field20 );
                    writer.Write( Field24 );
                    writer.Write( Field28 );
                }
            }

            public SubData SubData0;
            public SubData SubData1;
            public SubData SubData2;
            public SubData SubData3;
            public SubData SubData4;
            public SubData SubData5;
            public SubData SubData6;
            public SubData SubData7;
            public SubData SubData8;
            public SubData SubData9;

            internal void Read( EndianBinaryReader reader )
            {
                SubData0.Read( reader );
                SubData1.Read( reader );
                SubData2.Read( reader );
                SubData3.Read( reader );
                SubData4.Read( reader );
                SubData5.Read( reader );
                SubData6.Read( reader );
                SubData7.Read( reader );
                SubData8.Read( reader );
                SubData9.Read( reader );
            }

            internal void Write( EndianBinaryWriter writer )
            {
                SubData0.Write( writer );
                SubData1.Write( writer );
                SubData2.Write( writer );
                SubData3.Write( writer );
                SubData4.Write( writer );
                SubData5.Write( writer );
                SubData6.Write( writer );
                SubData7.Write( writer );
                SubData8.Write( writer );
                SubData9.Write( writer );
            }
        }
    }
}