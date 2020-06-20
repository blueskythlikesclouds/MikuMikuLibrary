using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Skeletons;

namespace MikuMikuLibrary.Motions
{
    public class Motion : BinaryFile
    {
        private MotionBinding mBinding;

        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public uint Id { get; set; }
        public string Name { get; set; }
        public byte HighBits { get; set; }
        public ushort FrameCount { get; set; }
        public List<KeySet> KeySets { get; }
        public List<BoneInfo> BoneInfos { get; }

        public bool HasBinding => mBinding != null;

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            if ( section != null )
            {
                if ( section.Format == BinaryFormat.F2nd )
                    reader.SeekCurrent( 4 );

                Id = reader.ReadUInt32();
                Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            }

            long keySetCountOffset = reader.ReadOffset();
            long keySetTypesOffset = reader.ReadOffset();
            long keySetsOffset = reader.ReadOffset();
            long boneInfosOffset = reader.ReadOffset();

            if ( section != null )
            {
                long boneIdsOffset = reader.ReadOffset();
                int boneInfoCount = reader.ReadInt32();

                BoneInfos.Capacity = boneInfoCount;
                reader.ReadAtOffset( boneInfosOffset, () =>
                {
                    for ( int i = 0; i < boneInfoCount; i++ )
                        BoneInfos.Add( new BoneInfo { Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) } );
                } );

                reader.ReadAtOffset( boneIdsOffset, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        boneInfo.Id = ( uint ) reader.ReadUInt64();
                } );
            }
            else
            {
                reader.ReadAtOffset( boneInfosOffset, () =>
                {
                    uint index = reader.ReadUInt16();

                    do
                    {
                        BoneInfos.Add( new BoneInfo { Id = index } );
                        index = reader.ReadUInt16();
                    } while ( index != 0 && reader.Position < reader.Length );
                } );
            }

            reader.ReadAtOffset( keySetCountOffset, () =>
            {
                int info = reader.ReadUInt16();
                int keySetCount = info & 0x3FFF;
                HighBits = ( byte ) ( info >> 14 );
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
                            keySet.Read( reader, section != null );
                    } );
                } );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
            {
                if ( section.Format == BinaryFormat.F2nd )
                    writer.Write( 0 );

                writer.Write( Id );
                writer.AddStringToStringTable( Name );
            }

            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                writer.Write( ( ushort ) ( ( HighBits << 14 ) | KeySets.Count ) );
                writer.Write( FrameCount );
            } );

            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                for ( int i = 0, b = 0; i < KeySets.Count; i++ )
                {
                    var keySet = KeySets[ i ];

                    if ( keySet.Keys.Count == 1 )
                        keySet.Type = KeySetType.Static;

                    else if ( keySet.Keys.Count > 1 )
                        keySet.Type = keySet.HasTangents
                            ? KeySetType.Tangent
                            : KeySetType.Linear;

                    else
                        keySet.Type = KeySetType.None;

                    b |= ( int ) keySet.Type << ( i % 8 * 2 );

                    if ( i != KeySets.Count - 1 && i % 8 != 7 )
                        continue;

                    writer.Write( ( ushort ) b );
                    b = 0;
                }
            } );

            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var keySet in KeySets )
                    keySet.Write( writer, section != null );
            } );

            if ( section != null )
            {
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        writer.AddStringToStringTable( boneInfo.Name );
                } );

                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        writer.Write( ( long ) boneInfo.Id );
                } );

                writer.Write( BoneInfos.Count );
                writer.Align( 16 );
            }
            else
            {
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        writer.Write( ( ushort ) boneInfo.Id );

                    writer.Write( ( ushort ) 0 );
                } );
            }
        }

        public MotionBinding Bind( Skeleton skeleton = null,
            MotionDatabase motionDatabase = null )
        {
            if ( mBinding != null )
                return mBinding;

            if ( skeleton == null )
                throw new ArgumentNullException( nameof( skeleton ) );

            var binding = new MotionBinding( this );

            int index = 0;

            foreach ( var boneInfo in BoneInfos )
            {
                if ( motionDatabase != null && boneInfo.Id >= motionDatabase.BoneNames.Count )
                    break;

                boneInfo.Name = boneInfo.Name ?? motionDatabase?.BoneNames[ ( int ) boneInfo.Id ] ??
                    throw new ArgumentNullException( nameof( motionDatabase ) );

                var bone = skeleton.GetBone( boneInfo.Name );
                var boneBinding = new BoneBinding { Name = boneInfo.Name };

                if ( bone != null )
                {
                    if ( bone.Type != BoneType.Rotation )
                        boneBinding.Position = BindNext();

                    if ( bone.Type != BoneType.Position )
                        boneBinding.Rotation = BindNext();

                    binding.BoneBindings.Add( boneBinding );
                }

                else if ( boneInfo.Name.Equals( "gblctr", StringComparison.OrdinalIgnoreCase ) )
                {
                    binding.GlobalTransformation.Position = BindNext();
                }

                else if ( boneInfo.Name.Equals( "kg_ya_ex", StringComparison.OrdinalIgnoreCase ) )
                {
                    binding.GlobalTransformation.Rotation = BindNext();
                }

                KeyBinding BindNext()
                {
                    return new KeyBinding
                    {
                        X = KeySets[ index++ ],
                        Y = KeySets[ index++ ],
                        Z = KeySets[ index++ ]
                    };
                }
            }

            return mBinding = binding;
        }

        public void Load( Stream source, Skeleton skeleton, bool leaveOpen = false )
        {
            Load( source, leaveOpen );

            if ( skeleton != null )
                Bind( skeleton );
        }

        public void Load( string filePath, Skeleton skeleton )
        {
            using ( var stream = File.OpenRead( filePath ) ) 
                Load( stream, skeleton );
        }

        public void Save( Stream destination, Skeleton skeleton, bool leaveOpen = false )
        {
            if ( skeleton != null )
                mBinding?.Unbind( skeleton );

            // Force modern format if we are saving motions individually
            if ( !Format.IsModern() )
                Format = BinaryFormat.F2nd;

            if ( !string.IsNullOrEmpty( Name ) )
                Id = MurmurHash.Calculate( Name );

            foreach ( var boneInfo in BoneInfos.Where( boneInfo => !string.IsNullOrEmpty( boneInfo.Name ) ) )
                boneInfo.Id = MurmurHash.Calculate( boneInfo.Name );

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, Skeleton skeleton )
        {
            using ( var stream = File.Create( filePath ) ) 
                Save( stream, skeleton );
        }

        public Motion()
        {
            KeySets = new List<KeySet>();
            BoneInfos = new List<BoneInfo>();
        }
    }

    public class BoneInfo
    {
        public string Name { get; set; }
        public uint Id { get; set; }
    }
}