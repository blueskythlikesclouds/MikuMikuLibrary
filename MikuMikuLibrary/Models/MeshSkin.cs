using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;

namespace MikuMikuLibrary.Models
{
    public class MeshSkin
    {
        public const int ByteSize = 0x40;

        public List<Bone> Bones { get; }
        public MeshExData ExData { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            uint boneIDsOffset = reader.ReadUInt32();
            uint boneMatricesOffset = reader.ReadUInt32();
            uint boneNamesOffset = reader.ReadUInt32();
            uint meshExDataOffset = reader.ReadUInt32();
            int boneCount = reader.ReadInt32();
            uint boneParentIDsOffset = reader.ReadUInt32();

            reader.ReadAtOffset( boneIDsOffset, () =>
            {
                Bones.Capacity = boneCount;
                for ( int i = 0; i < boneCount; i++ )
                {
                    var bone = new Bone();
                    bone.ID = reader.ReadInt32();
                    Bones.Add( bone );
                }
            } );

            reader.ReadAtOffset( boneMatricesOffset, () =>
            {
                foreach ( var bone in Bones )
                    bone.Matrix = reader.ReadMatrix4x4();
            } );

            reader.ReadAtOffset( boneNamesOffset, () =>
            {
                foreach ( var bone in Bones )
                    bone.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffsetIfNotZero( meshExDataOffset, () =>
            {
                ExData = new MeshExData();
                ExData.Read( reader );
            } );

            reader.ReadAtOffsetIfNotZero( boneParentIDsOffset, () =>
            {
                foreach ( var bone in Bones )
                    bone.ParentID = reader.ReadInt32();
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.ID );
            } );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.Matrix );
            } );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.AddStringToStringTable( bone.Name );
            } );
            writer.EnqueueOffsetWriteAlignedIf( ExData != null, 16, AlignmentKind.Center, () => ExData.Write( writer ) );
            writer.Write( Bones.Count );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.ParentID );

                writer.PopStringTable();
            } );
            writer.WriteNulls( 40 );
        }

        public MeshSkin()
        {
            Bones = new List<Bone>();
        }
    }
}
