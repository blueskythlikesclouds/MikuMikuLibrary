using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Models
{
    public class Skin
    {
        public const int BYTE_SIZE = 0x40;

        public List<Bone> Bones { get; }
        public ExData ExData { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            long boneIDsOffset = reader.ReadOffset();
            long boneMatricesOffset = reader.ReadOffset();
            long boneNamesOffset = reader.ReadOffset();
            long meshExDataOffset = reader.ReadOffset();
            int boneCount = reader.ReadInt32();
            long boneParentIDsOffset = reader.ReadOffset();

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
                    bone.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( meshExDataOffset, () =>
            {
                ExData = new ExData();
                ExData.Read( reader );
            } );

            reader.ReadAtOffset( boneParentIDsOffset, () =>
            {
                foreach ( var bone in Bones )
                    bone.ParentID = reader.ReadInt32();
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.ID );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.Matrix );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.AddStringToStringTable( bone.Name );
            } );
            writer.ScheduleWriteOffsetIf( ExData != null, 16, AlignmentMode.Center, () => ExData.Write( writer ) );
            writer.Write( Bones.Count );
            writer.ScheduleWriteOffsetIf( Bones.Any( x => x.ParentID != -1 ), 16, AlignmentMode.Center, () =>
            {
                foreach ( var bone in Bones )
                    writer.Write( bone.ParentID );
            } );
            writer.WriteNulls( writer.AddressSpace == AddressSpace.Int64 ? 32 : 40 );
        }

        public Skin()
        {
            Bones = new List<Bone>();
        }
    }
}
