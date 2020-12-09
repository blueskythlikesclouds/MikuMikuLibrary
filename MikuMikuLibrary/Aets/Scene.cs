using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Aets
{
    public class Scene
    {
        public string Name { get; set; }

        public float StartFrame { get; set; }
        public float EndFrame { get; set; }
        public float FrameRate { get; set; }

        public Color BackgroundColor { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Camera Camera { get; set; }
        public List<Composition> Compositions { get; }
        public List<Video> Videos { get; }
        public List<Audio> Audios { get; }

        internal void Read( EndianBinaryReader reader )
        {
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            StartFrame = reader.ReadSingle();
            EndFrame = reader.ReadSingle();
            FrameRate = reader.ReadSingle();

            BackgroundColor = reader.ReadColor( VectorBinaryFormat.UInt8 );
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();

            long cameraOffset = reader.ReadOffset();
            int compositionCount = reader.ReadInt32();
            long compositionsOffset = reader.ReadOffset();
            int videoCount = reader.ReadInt32();
            long videosOffset = reader.ReadOffset();
            int audioCount = reader.ReadInt32();
            long audiosOffset = reader.ReadOffset();

            reader.ReadAtOffset( cameraOffset, () =>
            {
                Camera = new Camera();
                Camera.Read( reader );
            } );

            reader.ReadAtOffset( compositionsOffset, () =>
            {
                Compositions.Capacity = compositionCount;

                for ( int i = 0; i < compositionCount; i++ )
                {
                    var composition = new Composition();
                    composition.Read( reader );
                    Compositions.Add( composition );
                }
            });           
            
            reader.ReadAtOffset( videosOffset, () =>
            {
                Videos.Capacity = videoCount;

                for ( int i = 0; i < videoCount; i++ )
                {
                    var video = new Video();
                    video.Read( reader );
                    Videos.Add( video );
                }
            });          
            
            reader.ReadAtOffset( audiosOffset, () =>
            {
                Audios.Capacity = audioCount;

                for ( int i = 0; i < audioCount; i++ )
                {
                    var audio = new Audio();
                    audio.Read( reader );
                    Audios.Add( audio );
                }
            });

            foreach ( var composition in Compositions )
            {
                foreach ( var layer in composition.Layers )
                    layer.ResolveReferences( composition, this );
            }
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.AddStringToStringTable( Name );
            writer.Write( StartFrame );
            writer.Write( EndFrame );
            writer.Write( FrameRate );
            writer.Write( BackgroundColor, VectorBinaryFormat.UInt8 );
            writer.Write( Width );
            writer.Write( Height );

            writer.ScheduleWriteOffsetIf( Camera != null, 4, AlignmentMode.Left, () =>
            {
                Camera.Write( writer );
            } );

            writer.Write( Compositions.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var composition in Compositions )
                    composition.Write( writer );
            } );         
            
            writer.Write( Videos.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var video in Videos )
                    video.Write( writer );
            } );         
            
            writer.Write( Audios.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var audio in Audios )
                    audio.Write( writer );
            } );
        }

        public Scene()
        {
            Compositions = new List<Composition>();
            Videos = new List<Video>();
            Audios = new List<Audio>();
        }
    }
}