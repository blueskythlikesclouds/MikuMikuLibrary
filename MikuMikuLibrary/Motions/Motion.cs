using System.Collections.Generic;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Motions
{
    public class Motion : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public int ID { get; set; }
        public string Name { get; set; }
        public int HighBits { get; set; }
        public int FrameCount { get; set; }
        public List<KeySet> KeySets { get; }
        public List<int> BoneIndices { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            long keySetCountOffset = reader.ReadOffset();
            long keySetTypesOffset = reader.ReadOffset();
            long keySetsOffset = reader.ReadOffset();
            long boneIndicesOffset = reader.ReadOffset();

            reader.ReadAtOffset( keySetCountOffset, () =>
            {
                int info = reader.ReadUInt16();
                int keySetCount = info & 0x3FFF;
                HighBits = info >> 14;
                FrameCount = reader.ReadUInt16();

                KeySets.Capacity = keySetCount;
                reader.ReadAtOffset( keySetTypesOffset, () =>
                {
                    for ( int i = 0, b = 0; i < keySetCount; i++ )
                    {
                        if ( i % 8 == 0 )
                            b = reader.ReadUInt16();

                        KeySets.Add( new KeySet { Type = ( KeySetType ) ( ( b >> ( i % 8 * 2 ) ) & 3 ) } );
                    }

                    reader.ReadAtOffset( keySetsOffset, () =>
                    {
                        foreach ( var keySet in KeySets )
                            keySet.Read( reader );
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
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                writer.Write( ( ushort ) ( ( HighBits << 14 ) | KeySets.Count ) );
                writer.Write( ( ushort ) FrameCount );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                for ( int i = 0, b = 0; i < KeySets.Count; i++ )
                {
                    var keySet = KeySets[ i ];

                    if ( keySet.Keys.Count == 1 )
                        keySet.Type = KeySetType.Static;

                    else if ( keySet.Keys.Count > 1 )
                        keySet.Type = keySet.IsInterpolated
                            ? KeySetType.Interpolated
                            : KeySetType.Linear;

                    else
                        keySet.Type = KeySetType.None;

                    b |= ( int ) keySet.Type << ( i % 8 * 2 );

                    if ( i == KeySets.Count - 1 || i % 8 == 7 )
                    {
                        writer.Write( ( ushort ) b );
                        b = 0;
                    }
                }
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var keySet in KeySets )
                    keySet.Write( writer );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( int index in BoneIndices )
                    writer.Write( ( ushort ) index );

                writer.Write( ( ushort ) 0 );
            } );
        }

        public MotionController GetController( SkeletonEntry skeletonEntry, MotionDatabase motionDatabase )
        {
            var controller = new MotionController { FrameCount = FrameCount };

            for ( int i = 0, j = 0; i < BoneIndices.Count; i++ )
            {
                string boneName = motionDatabase.BoneNames[ BoneIndices[ i ] ];

                var boneEntry = skeletonEntry.GetBoneEntry( boneName );
                if ( boneEntry == null )
                {
                    boneName = motionDatabase.BoneNames[ BoneIndices[ ++i ] ];
                    boneEntry = skeletonEntry.GetBoneEntry( boneName );

                    if ( boneEntry == null )
                        break;
                }

                var keyController = new KeyController { Target = boneName };

                if ( boneEntry.Field00 >= 3 )
                    keyController.Position = new KeySetVector
                    {
                        X = KeySets[ j++ ],
                        Y = KeySets[ j++ ],
                        Z = KeySets[ j++ ],
                    };

                keyController.Rotation = new KeySetVector
                {
                    X = KeySets[ j++ ],
                    Y = KeySets[ j++ ],
                    Z = KeySets[ j++ ],
                };

                controller.KeyControllers.Add( keyController );
            }

            return controller;
        }

        public Motion()
        {
            KeySets = new List<KeySet>();
            BoneIndices = new List<int>();
        }
    }
}