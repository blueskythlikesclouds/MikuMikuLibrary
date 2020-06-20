using System.Collections.Generic;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Objects;
using MikuMikuLibrary.Materials;

namespace MikuMikuLibrary.Objects
{
    public class Object
    {
        public BoundingSphere BoundingSphere { get; set; }
        public List<Mesh> Meshes { get; }
        public List<Material> Materials { get; }
        public uint Flags { get; set; }
        public Skin Skin { get; set; }
        public string Name { get; set; }
        public uint Id { get; set; }

        internal void Read( EndianBinaryReader reader, ObjectSection section = null )
        {
            uint signature = reader.ReadUInt32();
            reader.SeekCurrent( 4 ); // Unused flags

            int meshCount, materialCount;
            long meshesOffset, materialsOffset;

            // X stores mesh/material count before the bounding sphere
            if ( section?.Format == BinaryFormat.X )
            {
                meshCount = reader.ReadInt32();
                materialCount = reader.ReadInt32();
                BoundingSphere = reader.ReadBoundingSphere();
                meshesOffset = reader.ReadOffset();
                materialsOffset = reader.ReadOffset();

                reader.SkipNulls( 4 * sizeof( uint ) );
                Flags = reader.ReadUInt32(); // TODO: Is this available in other games?
                reader.SkipNulls( sizeof( uint ) );
            }

            else
            {
                BoundingSphere = reader.ReadBoundingSphere();
                meshCount = reader.ReadInt32();
                meshesOffset = reader.ReadOffset();
                materialCount = reader.ReadInt32();
                materialsOffset = reader.ReadOffset();
            }

            reader.SkipNulls( 10 * sizeof( uint ) );

            Meshes.Capacity = meshCount;

            for ( int i = 0; i < meshCount; i++ )
            {
                reader.ReadAtOffset( meshesOffset + i * Mesh.GetByteSize( section?.Format ?? BinaryFormat.DT ), () =>
                {
                    var mesh = new Mesh();
                    mesh.Read( reader, section );
                    Meshes.Add( mesh );
                } );
            }

            Materials.Capacity = materialCount;

            for ( int i = 0; i < materialCount; i++ )
            {
                reader.ReadAtOffset( materialsOffset + i * Material.BYTE_SIZE, () =>
                {
                    var material = new Material();
                    material.Read( reader );
                    Materials.Add( material );
                } );
            }
        }

        internal void Write( EndianBinaryWriter writer, ObjectSection section = null )
        {
            writer.Write( 0x10000 );
            writer.Write( 0 );

            if ( section?.Format == BinaryFormat.X )
            {
                writer.Write( Meshes.Count );
                writer.Write( Materials.Count );
                writer.Write( BoundingSphere );
                writer.ScheduleWriteOffset( 8, AlignmentMode.Left, WriteMeshes );
                writer.ScheduleWriteOffset( 8, AlignmentMode.Left, WriteMaterials );
                writer.WriteNulls( 4 * sizeof( uint ) );
                writer.Write( Flags );
                writer.WriteNulls( sizeof( uint ) );
            }
            else
            {
                writer.Write( BoundingSphere );
                writer.Write( Meshes.Count );
                writer.ScheduleWriteOffset( 8, AlignmentMode.Left, WriteMeshes );
                writer.Write( Materials.Count );
                writer.ScheduleWriteOffset( 8, AlignmentMode.Left, WriteMaterials );
            }

            writer.WriteNulls( 10 * sizeof( uint ) );

            void WriteMeshes()
            {
                foreach ( var mesh in Meshes )
                    mesh.Write( writer, section );
            }

            void WriteMaterials()
            {
                foreach ( var material in Materials )
                    material.Write( writer );
            }
        }

        public Object()
        {
            Meshes = new List<Mesh>();
            Materials = new List<Material>();
        }
    }
}