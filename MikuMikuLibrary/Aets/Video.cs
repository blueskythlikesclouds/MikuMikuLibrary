using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Aets
{
    public class VideoSource
    {
        public string Name { get; set; }
        public uint Id { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Id = reader.ReadUInt32();
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Name );
            writer.Write( Id );
        }
    }

    public class Video
    {
        internal long ReferenceOffset { get; private set; }

        public Color Color { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public float Frames { get; set; }
        
        public List<VideoSource> Sources { get; }

        internal void Read( EndianBinaryReader reader )
        {
            ReferenceOffset = reader.Offset;

            Color = reader.ReadColor( VectorBinaryFormat.UInt8 );
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            Frames = reader.ReadSingle();

            int sourceCount = reader.ReadInt32();
            reader.ReadOffset( () =>
            {
                Sources.Capacity = sourceCount;

                for ( int i = 0; i < sourceCount; i++ )
                {
                    var source = new VideoSource();
                    source.Read( reader );
                    Sources.Add( source );
                }
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            ReferenceOffset = writer.Offset;

            writer.Write( Color, VectorBinaryFormat.UInt8 );
            writer.Write( Width );
            writer.Write( Height );
            writer.Write( Frames );
            writer.Write( Sources.Count );
            writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
            {
                foreach ( var source in Sources )
                    source.Write( writer );
            } );
        }

        public Video()
        {
            Sources = new List<VideoSource>();
        }
    }
}