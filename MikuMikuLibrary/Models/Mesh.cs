using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Maths;
using System.Collections.Generic;

namespace MikuMikuLibrary.Models
{
    public class Mesh
    {
        public const int BYTE_SIZE = 0x50;

        public List<SubMesh> SubMeshes { get; }
        public List<Material> Materials { get; }
        public Skin Skin { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
        public BoundingSphere BoundingSphere { get; set; }

        internal void Read( EndianBinaryReader reader, MeshSection section = null )
        {
            uint signature = reader.ReadUInt32();
            reader.SeekCurrent( 4 );

            int subMeshCount, materialCount;
            long subMeshesOffset, materialsOffset;

            // X stores submesh/material count before the bounding sphere
            if ( section?.Format == BinaryFormat.X )
            {
                subMeshCount = reader.ReadInt32();
                materialCount = reader.ReadInt32();
                BoundingSphere = reader.ReadBoundingSphere();
                subMeshesOffset = reader.ReadOffset();
                materialsOffset = reader.ReadOffset();
            }

            else
            {
                BoundingSphere = reader.ReadBoundingSphere();
                subMeshCount = reader.ReadInt32();
                subMeshesOffset = reader.ReadOffset();
                materialCount = reader.ReadInt32();
                materialsOffset = reader.ReadOffset();
            }

            SubMeshes.Capacity = subMeshCount;
            for ( int i = 0; i < subMeshCount; i++ )
            {
                reader.ReadAtOffset( subMeshesOffset + ( i * SubMesh.GetByteSize( section?.Format ?? BinaryFormat.DT ) ), () =>
                {
                    var submesh = new SubMesh();
                    submesh.Read( reader, section );
                    SubMeshes.Add( submesh );
                } );
            }

            Materials.Capacity = materialCount;
            for ( int i = 0; i < materialCount; i++ )
            {
                reader.ReadAtOffset( materialsOffset + ( i * Material.BYTE_SIZE ), () =>
                {
                    var material = new Material();
                    material.Read( reader );
                    Materials.Add( material );
                } );
            }
        }

        internal void Write( EndianBinaryWriter writer, MeshSection section = null )
        {
            writer.Write( 0x10000 );
            writer.Write( 0 );

            if ( section?.Format == BinaryFormat.X )
            {
                writer.Write( SubMeshes.Count );
                writer.Write( Materials.Count );
                writer.Write( BoundingSphere );
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, WriteSubMeshes );
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, WriteMaterials );
            }
            else
            {
                writer.Write( BoundingSphere );
                writer.Write( SubMeshes.Count );
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, WriteSubMeshes );
                writer.Write( Materials.Count );
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, WriteMaterials );
            }

            writer.WriteNulls( section?.Format == BinaryFormat.X ? 0x40 : 0x28 );

            void WriteSubMeshes()
            {
                foreach ( var subMesh in SubMeshes )
                    subMesh.Write( writer, section );
            }

            void WriteMaterials()
            {
                foreach ( var material in Materials )
                    material.Write( writer );
            }
        }

        public Mesh()
        {
            SubMeshes = new List<SubMesh>();
            Materials = new List<Material>();
        }
    }
}
