using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuLibrary.Databases
{
    public class MeshEntry
    {
        public string Name { get; set; }
        public ushort ID { get; set; }
    }

    public class ObjectEntry
    {
        public string Name { get; set; }
        public ushort ID { get; set; }
        public string FileName { get; set; }
        public string TextureFileName { get; set; }
        public string ArchiveFileName { get; set; }
        public List<MeshEntry> Meshes { get; }

        public ObjectEntry()
        {
            Meshes = new List<MeshEntry>();
        }
    }

    public class ObjectDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat; }
        }

        public List<ObjectEntry> Objects { get; }
        public int Unknown { get; set; }

        internal override void Read( EndianBinaryReader reader, Section section = null )
        {
            int objectCount = reader.ReadInt32();
            Unknown = reader.ReadInt32();
            uint objectsOffset = reader.ReadUInt32();
            int meshCount = reader.ReadInt32();
            uint meshesOffset = reader.ReadUInt32();

            reader.ReadAtOffset( objectsOffset, () =>
            {
                Objects.Capacity = objectCount;
                for ( int i = 0; i < objectCount; i++ )
                {
                    uint nameOffset = reader.ReadUInt32();
                    ushort id = reader.ReadUInt16();
                    reader.SeekCurrent( 2 );
                    uint fileNameOffset = reader.ReadUInt32();
                    uint textureFileNameOffset = reader.ReadUInt32();
                    uint archiveFileNameOffset = reader.ReadUInt32();
                    reader.SeekCurrent( 16 );

                    var objectEntry = new ObjectEntry();
                    objectEntry.ID = id;
                    objectEntry.Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );
                    objectEntry.FileName = reader.ReadStringAtOffset( fileNameOffset, StringBinaryFormat.NullTerminated );
                    objectEntry.TextureFileName = reader.ReadStringAtOffset( textureFileNameOffset, StringBinaryFormat.NullTerminated );
                    objectEntry.ArchiveFileName = reader.ReadStringAtOffset( archiveFileNameOffset, StringBinaryFormat.NullTerminated );
                    Objects.Add( objectEntry );
                }
            } );

            reader.ReadAtOffset( meshesOffset, () =>
            {
                for ( int i = 0; i < meshCount; i++ )
                {
                    ushort meshID = reader.ReadUInt16();
                    ushort parentObjectID = reader.ReadUInt16();
                    uint nameOffset = reader.ReadUInt32();

                    var meshEntry = new MeshEntry();
                    meshEntry.ID = meshID;
                    meshEntry.Name = reader.ReadStringAtOffset( nameOffset, StringBinaryFormat.NullTerminated );

                    var obj = Objects.First( x => x.ID == parentObjectID );
                    obj.Meshes.Add( meshEntry );
                }
            } );
        }

        internal override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( Objects.Count );
            writer.Write( Unknown );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var objectEntry in Objects )
                {
                    writer.AddStringToStringTable( objectEntry.Name );
                    writer.Write( objectEntry.ID );
                    writer.WriteNulls( 2 );
                    writer.AddStringToStringTable( objectEntry.FileName );
                    writer.AddStringToStringTable( objectEntry.TextureFileName );
                    writer.AddStringToStringTable( objectEntry.ArchiveFileName );
                    writer.WriteNulls( 16 );
                }
                writer.PopStringTable();
            } );
            writer.Write( Objects.Sum( x => x.Meshes.Count ) );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () =>
            {
                foreach ( var objectEntry in Objects )
                {
                    foreach ( var meshEntry in objectEntry.Meshes )
                    {
                        writer.Write( meshEntry.ID );
                        writer.Write( objectEntry.ID );
                        writer.AddStringToStringTable( meshEntry.Name );
                    }
                }
                writer.PopStringTablesReversed();
            } );
        }

        public ObjectDatabase()
        {
            Objects = new List<ObjectEntry>();
        }
    }
}
