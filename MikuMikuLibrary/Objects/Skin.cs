using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects.Extra;
using MikuMikuLibrary.Objects.Extra.Blocks;

namespace MikuMikuLibrary.Objects
{
    public class Skin
    {
        public const int BYTE_SIZE = 0x40;

        public List<BoneInfo> Bones { get; }
        public List<Block> Blocks { get; }

        internal void Read( EndianBinaryReader reader )
        {
            long boneIdsOffset = reader.ReadOffset();
            long boneMatricesOffset = reader.ReadOffset();
            long boneNamesOffset = reader.ReadOffset();
            long meshExDataOffset = reader.ReadOffset();
            int boneCount = reader.ReadInt32();
            long boneParentIdsOffset = reader.ReadOffset();

            reader.ReadAtOffset( boneIdsOffset, () =>
            {
                Bones.Capacity = boneCount;
                for ( int i = 0; i < boneCount; i++ )
                {
                    uint id = reader.ReadUInt32();
                    Bones.Add( new BoneInfo { Id = id, IsEx = ( id & 0x8000 ) != 0 } );
                }
            } );

            reader.ReadAtOffset( boneMatricesOffset, () =>
            {
                foreach ( var bone in Bones )
                    bone.InverseBindPoseMatrix = reader.ReadMatrix4x4();
            } );

            reader.ReadAtOffset( boneNamesOffset, () =>
            {
                foreach ( var bone in Bones )
                    bone.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( meshExDataOffset, () =>
            {
                int osageNameCount = reader.ReadInt32();
                int osageBoneCount = reader.ReadInt32();
                reader.SeekCurrent( 4 );
                long osageBonesOffset = reader.ReadOffset();
                long osageNamesOffset = reader.ReadOffset();
                long blocksOffset = reader.ReadOffset();
                int stringCount = reader.ReadInt32();
                long stringsOffset = reader.ReadOffset();
                long osageSiblingInfosOffset = reader.ReadOffset();

                var stringSet = new StringSet( reader, stringsOffset, stringCount );
                var osageBones = new List<OsageBone>( osageBoneCount );

                reader.ReadAtOffset( osageBonesOffset, () =>
                {
                    for ( int i = 0; i < osageBoneCount; i++ )
                    {
                        var osageBone = new OsageBone();
                        osageBone.Read( reader, stringSet );
                        osageBones.Add( osageBone );
                    }
                } );

                reader.ReadAtOffset( blocksOffset, () =>
                {
                    while ( true )
                    {
                        string blockSignature = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                        long blockOffset = reader.ReadOffset();

                        if ( blockOffset == 0 )
                            break;

                        reader.ReadAtOffset( blockOffset, () =>
                        {
                            if ( !Block.BlockFactory.TryGetValue( blockSignature, out var blockConstructor ) )
                                return;

                            var block = blockConstructor();
                            block.Read( reader, stringSet );

                            if ( block is OsageBlock osageNode )
                                for ( int i = 0; i < osageNode.Count; i++ )
                                    osageNode.Bones.Add( osageBones[ osageNode.StartIndex + i ] );

                            Blocks.Add( block );
                        } );
                    }
                } );

                reader.ReadAtOffset( osageSiblingInfosOffset, () =>
                {
                    while ( true )
                    {
                        string boneName = stringSet.ReadString( reader );
                        if ( boneName == null )
                            break;

                        string siblingName = stringSet.ReadString( reader );
                        float siblingDistance = reader.ReadSingle();

                        var osageBone = osageBones.FirstOrDefault( x => x.Name.Equals( boneName ) );
                        if ( osageBone == null )
                            continue;

                        osageBone.SiblingName = siblingName;
                        osageBone.SiblingDistance = siblingDistance;
                    }
                } );
            } );

            reader.ReadAtOffset( boneParentIdsOffset, () =>
            {
                foreach ( var bone in Bones )
                {
                    uint parentId = reader.ReadUInt32();
                    if ( parentId != 0xFFFFFFFF )
                        bone.Parent = Bones.FirstOrDefault( x => x.Id == parentId );
                }
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            var stringSet = new StringSet( this );

            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                {
                    if ( bone.IsEx )
                        bone.Id = stringSet.GetStringId( bone.Name );

                    writer.Write( bone.Id );
                }
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.InverseBindPoseMatrix );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.AddStringToStringTable( bone.Name );
            } );
            writer.ScheduleWriteOffsetIf( Blocks.Count > 0, 16, AlignmentMode.Center, () =>
            {
                var osageNames = new List<string>( Blocks.Count / 2 );
                var osageBones = new List<OsageBone>( Blocks.Count / 2 );

                foreach ( var block in Blocks )
                {
                    if ( !( block is OsageBlock osageBlock ) )
                        continue;

                    osageBlock.StartIndex = osageBones.Count;
                    osageBones.AddRange( osageBlock.Bones );
                    osageNames.Add( osageBlock.ExternalName );
                }

                writer.Write( osageNames.Count );
                writer.Write( osageBones.Count );

                writer.WriteNulls( 4 );

                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
                {
                    foreach ( var osageBone in osageBones )
                        osageBone.Write( writer, stringSet );
                } );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( string value in osageNames )
                        writer.AddStringToStringTable( value );
                } );
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
                {
                    foreach ( var block in Blocks )
                    {
                        writer.AddStringToStringTable( block.Signature );
                        writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () => block.Write( writer, stringSet ) );
                    }

                    writer.WriteNulls( writer.AddressSpace.GetByteSize() * 2 );
                } );
                writer.Write( stringSet.Strings.Count );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( string value in stringSet.Strings )
                        writer.AddStringToStringTable( value );
                } );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var osageBone in osageBones )
                    {
                        if ( string.IsNullOrEmpty( osageBone.SiblingName ) )
                            continue;

                        stringSet.WriteString( writer, osageBone.Name );
                        stringSet.WriteString( writer, osageBone.SiblingName );
                        writer.Write( osageBone.SiblingDistance );
                    }

                    writer.WriteNulls( 12 );
                } );
                writer.WriteNulls( 32 );
            } );
            writer.Write( Bones.Count );
            writer.ScheduleWriteOffsetIf( Bones.Any( x => x.Parent != null ), 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.Parent?.Id ?? 0xFFFFFFFF );
            } );
            writer.WriteNulls( writer.AddressSpace == AddressSpace.Int64 ? 32 : 40 );
        }

        public Skin()
        {
            Bones = new List<BoneInfo>();
            Blocks = new List<Block>();
        }
    }
}