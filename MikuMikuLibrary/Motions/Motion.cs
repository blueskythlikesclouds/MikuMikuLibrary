using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Motions
{
    public class Motion
    {
        public int HighBits { get; set; }
        public int FrameCount { get; set; }
        public List<KeyFrameSet> KeyFrameSets { get; }
        public List<int> BoneIndices { get; }

        internal bool Read( EndianBinaryReader reader )
        {
            long keyFrameSetCountOffset = reader.ReadOffset();
            if ( keyFrameSetCountOffset == 0 )
                return false;

            long keyFrameSetTypesOffset = reader.ReadOffset();
            long keyFrameSetsOffset = reader.ReadOffset();
            long boneIndicesOffset = reader.ReadOffset();

            reader.ReadAtOffset( keyFrameSetCountOffset, () =>
            {
                int info = reader.ReadUInt16();
                int keyFrameSetCount = info & 0x3FFF;

                HighBits = info >> 14;
                FrameCount = reader.ReadUInt16();

                KeyFrameSets.Capacity = keyFrameSetCount;
                reader.ReadAtOffset( keyFrameSetTypesOffset, () =>
                {
                    for ( int i = 0, b = 0; i < keyFrameSetCount; i++ )
                    {
                        if ( i % 8 == 0 )
                            b = reader.ReadUInt16();

                        KeyFrameSets.Add( new KeyFrameSet { Type = ( KeyFrameType )( ( b >> ( i % 8 * 2 ) ) & 3 ) } );
                    }

                    reader.ReadAtOffset( keyFrameSetsOffset, () =>
                    {
                        foreach ( var keyFrameSet in KeyFrameSets )
                            keyFrameSet.Read( reader );
                    } );
                } );
            } );

            reader.ReadAtOffset( boneIndicesOffset, () =>
            {
                int index = reader.ReadUInt16();

                do
                {
                    BoneIndices.Add( index );
                    index = reader.ReadUInt16();
                } while ( index != 0 );
            } );

            return true;
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                writer.Write( ( ushort )( ( HighBits << 14 ) | KeyFrameSets.Count ) );
                writer.Write( ( ushort )FrameCount );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                for ( int i = 0, b = 0; i < KeyFrameSets.Count; i++ )
                {
                    var keyFrameSet = KeyFrameSets[ i ];

                    if ( keyFrameSet.KeyFrames.Count == 1 )
                        keyFrameSet.Type = KeyFrameType.Static;

                    else if ( keyFrameSet.KeyFrames.Count > 1 )
                        keyFrameSet.Type = keyFrameSet.IsInterpolated
                            ? KeyFrameType.LinearInterpolated
                            : KeyFrameType.Linear;

                    else
                        keyFrameSet.Type = KeyFrameType.None;

                    b |= ( int )keyFrameSet.Type << ( i % 8 * 2 );

                    if ( i == KeyFrameSets.Count - 1 || ( i != 0 && ( i % 8 ) == 7 ) )
                    {
                        writer.Write( ( ushort )b );
                        b = 0;
                    }
                }
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var keyFrameSet in KeyFrameSets )
                    keyFrameSet.Write( writer );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var index in BoneIndices )
                    writer.Write( ( ushort )index );

                writer.Write( ( ushort )0 );
            } );
        }

        public Motion()
        {
            KeyFrameSets = new List<KeyFrameSet>();
            BoneIndices = new List<int>();
        }
    }
}
