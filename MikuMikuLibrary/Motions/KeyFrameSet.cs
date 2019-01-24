using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Motions
{
    public class KeyFrameSet
    {
        public List<KeyFrame> KeyFrames { get; }
        public bool IsInterpolated { get; set; }

        internal KeyFrameType Type { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            if ( Type == KeyFrameType.Static )
                KeyFrames.Add( new KeyFrame { Value = reader.ReadSingle() } );

            else if ( Type != KeyFrameType.None )
            {
                ushort keyFrameCount = reader.ReadUInt16();

                KeyFrames.Capacity = keyFrameCount;
                for ( int i = 0; i < keyFrameCount; i++ )
                    KeyFrames.Add( new KeyFrame { FrameIndex = reader.ReadUInt16() } );

                reader.Align( 4 );

                IsInterpolated = ( Type == KeyFrameType.LinearInterpolated );
                foreach ( var keyFrame in KeyFrames )
                {
                    keyFrame.Value = reader.ReadSingle();
                    if ( IsInterpolated )
                        keyFrame.Interpolation = reader.ReadSingle();
                }
            }
        }

        internal void Write( EndianBinaryWriter writer )
        {
            if ( KeyFrames.Count == 1 )
                writer.Write( KeyFrames[ 0 ].Value );

            else if ( KeyFrames.Count > 1 )
            {
                writer.Write( ( ushort )KeyFrames.Count );
                foreach ( var keyFrame in KeyFrames )
                    writer.Write( ( ushort )keyFrame.FrameIndex );

                writer.WriteAlignmentPadding( 4 );
                foreach ( var keyFrame in KeyFrames )
                {
                    writer.Write( keyFrame.Value );
                    if ( IsInterpolated )
                        writer.Write( keyFrame.Interpolation );
                }
            }
        }

        public KeyFrameSet()
        {
            KeyFrames = new List<KeyFrame>();
        }
    }
}