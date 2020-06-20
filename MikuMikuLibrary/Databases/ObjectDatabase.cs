using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Databases
{
    public class ObjectInfo
    {
        public string Name { get; set; }
        public uint Id { get; set; }
    }

    public class ObjectSetInfo
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        public string FileName { get; set; }
        public string TextureFileName { get; set; }
        public string ArchiveFileName { get; set; }
        public List<ObjectInfo> Objects { get; }

        public ObjectInfo GetObjectInfo( string meshName ) => 
            Objects.FirstOrDefault( x => x.Name.Equals( meshName, StringComparison.OrdinalIgnoreCase ) );

        public ObjectInfo GetObjectInfo( uint meshId ) => 
            Objects.FirstOrDefault( x => x.Id.Equals( meshId ) );

        public ObjectSetInfo()
        {
            Objects = new List<ObjectInfo>();
        }
    }

    public class ObjectDatabase : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<ObjectSetInfo> ObjectSets { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            if ( section != null )
                reader.SeekCurrent( 4 );

            int objectCount = reader.ReadInt32();
            uint maxId = reader.ReadUInt32();
            long objectsOffset = reader.ReadOffset();
            int meshCount = reader.ReadInt32();
            long meshesOffset = reader.ReadOffset();

            reader.ReadAtOffset( objectsOffset, () =>
            {
                ObjectSets.Capacity = objectCount;

                for ( int i = 0; i < objectCount; i++ )
                {
                    ObjectSets.Add( new ObjectSetInfo
                    {
                        Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated ),
                        Id = reader.ReadUInt32(),
                        FileName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated ),
                        TextureFileName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated ),
                        ArchiveFileName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated )
                    } );

                    reader.SkipNulls( 4 * sizeof( uint ) );
                }
            } );

            reader.ReadAtOffset( meshesOffset, () =>
            {
                for ( int i = 0; i < meshCount; i++ )
                {
                    uint id, parentId;

                    if ( section != null )
                    {
                        id = reader.ReadUInt32();
                        parentId = reader.ReadUInt32();
                    }
                    else
                    {
                        id = reader.ReadUInt16();
                        parentId = reader.ReadUInt16();
                    }

                    ObjectSets.First( x => x.Id == parentId ).Objects.Add(
                        new ObjectInfo
                        {
                            Id = id,
                            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated )
                        } );
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
                writer.Write( 0 );

            writer.Write( ObjectSets.Count );
            writer.Write( section == null ? ObjectSets.Max( x => x.Id ) : 0 );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var objectSetInfo in ObjectSets )
                {
                    writer.AddStringToStringTable( objectSetInfo.Name );
                    writer.Write( objectSetInfo.Id );
                    writer.AddStringToStringTable( objectSetInfo.FileName );
                    writer.AddStringToStringTable( objectSetInfo.TextureFileName );
                    writer.AddStringToStringTable( objectSetInfo.ArchiveFileName );
                    writer.WriteNulls( 4 * sizeof( uint ) );
                }
            } );
            writer.Write( ObjectSets.Sum( x => x.Objects.Count ) );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var objectSetInfo in ObjectSets )
                foreach ( var objectInfo in objectSetInfo.Objects )
                {
                    if ( section != null )
                    {
                        writer.Write( objectInfo.Id );
                        writer.Write( objectSetInfo.Id );
                    }

                    else
                    {
                        writer.Write( ( ushort ) objectInfo.Id );
                        writer.Write( ( ushort ) objectSetInfo.Id );
                    }

                    writer.AddStringToStringTable( objectInfo.Name );
                }
            } );
        }

        public ObjectSetInfo GetObjectSetInfo( string objectName ) => 
            ObjectSets.FirstOrDefault( x => x.Name.Equals( objectName, StringComparison.OrdinalIgnoreCase ) );

        public ObjectSetInfo GetObjectSetInfo( uint objectId ) => 
            ObjectSets.FirstOrDefault( x => x.Id.Equals( objectId ) );

        public ObjectSetInfo GetObjectSetInfoByFileName( string fileName ) => 
            ObjectSets.FirstOrDefault( x => x.FileName.Equals( fileName, StringComparison.OrdinalIgnoreCase ) );

        public ObjectInfo GetObjectInfo( string meshName ) => 
            ObjectSets.SelectMany( x => x.Objects ).FirstOrDefault( x => x.Name.Equals( meshName, StringComparison.OrdinalIgnoreCase ) );

        public ObjectInfo GetObjectInfo( uint meshId ) =>
            ObjectSets.SelectMany( x => x.Objects ).FirstOrDefault( x => x.Id.Equals( meshId ) );

        public ObjectDatabase()
        {
            ObjectSets = new List<ObjectSetInfo>();
        }
    }
}