using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.Objects
{
    public class ObjectSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public override Endianness Endianness
        {
            get => BinaryFormatUtilities.IsModern( Format ) ? base.Endianness : Endianness.Little;
            set => base.Endianness = value;
        }

        public List<Object> Objects { get; }
        public List<uint> TextureIds { get; }
        public TextureSet TextureSet { get; set; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            uint signature = reader.ReadUInt32();
            if ( signature != 0x5062500 && signature != 0x5062501 )
                throw new InvalidDataException( "Invalid signature (expected 0x5062500 or 0x5062501)" );

            int objectCount = reader.ReadInt32();
            int globalBoneCount = reader.ReadInt32();
            long objectsOffset = reader.ReadOffset();
            long objectSkinsOffset = reader.ReadOffset();
            long objectNamesOffset = reader.ReadOffset();
            long objectIdsOffset = reader.ReadOffset();
            long textureIdsOffset = reader.ReadOffset();
            int textureIdCount = reader.ReadInt32();

            reader.ReadAtOffsetIf( section == null, objectsOffset, () =>
            {
                Objects.Capacity = objectCount;

                for ( int i = 0; i < objectCount; i++ )
                {
                    reader.ReadOffset( () =>
                    {
                        reader.PushBaseOffset();
                        {
                            var obj = new Object();
                            obj.Read( reader );
                            Objects.Add( obj );
                        }
                        reader.PopBaseOffset();
                    } );
                }
            } );

            reader.ReadAtOffsetIf( section == null, objectSkinsOffset, () =>
            {
                foreach ( var obj in Objects )
                {
                    reader.ReadOffset( () =>
                    {
                        obj.Skin = new Skin();
                        obj.Skin.Read( reader );
                    } );
                }
            } );

            reader.ReadAtOffset( objectNamesOffset, () =>
            {
                foreach ( var obj in Objects )
                    obj.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( objectIdsOffset, () =>
            {
                foreach ( var obj in Objects )
                    obj.Id = reader.ReadUInt32();
            } );

            reader.ReadAtOffset( textureIdsOffset, () =>
            {
                TextureIds.Capacity = textureIdCount;

                for ( int i = 0; i < textureIdCount; i++ )
                    TextureIds.Add( reader.ReadUInt32() );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( section != null ? 0x5062501 : 0x5062500 );
            writer.Write( Objects.Count );
            writer.Write( section != null ? -1 : 0x39393939 );

            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var obj in Objects )
                {
                    writer.ScheduleWriteOffsetIf( section == null, 4, AlignmentMode.Center, () =>
                    {
                        writer.PushBaseOffset();
                        obj.Write( writer );
                        writer.PopBaseOffset();
                    } );
                }
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var obj in Objects )
                    writer.ScheduleWriteOffsetIf( obj.Skin != null && section == null, 16, AlignmentMode.Left, () => { obj.Skin.Write( writer ); } );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var obj in Objects )
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () => writer.Write( obj.Name, StringBinaryFormat.NullTerminated ) );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var obj in Objects )
                    writer.Write( obj.Id );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( uint textureId in TextureIds )
                    writer.Write( textureId );
            } );
            writer.Write( TextureIds.Count );
        }

        public void Load( Stream source, TextureSet textureSet, TextureDatabase textureDatabase, bool leaveOpen = false )
        {
            Load( source, leaveOpen );

            if ( textureSet == null || TextureIds.Count != textureSet.Textures.Count )
                return;

            TextureSet = textureSet;
            for ( int i = 0; i < TextureIds.Count; i++ )
            {
                TextureSet.Textures[ i ].Id = TextureIds[ i ];

                var textureInfo = textureDatabase?.GetTextureInfo( TextureIds[ i ] );

                if ( textureInfo != null )
                    TextureSet.Textures[ i ].Name = textureInfo.Name;
            }
        }

        public void Load( string filePath, ObjectDatabase objectDatabase, TextureDatabase textureDatabase )
        {
            string textureSetFilePath = string.Empty;

            var objectSetInfo = objectDatabase?.GetObjectSetInfoByFileName( Path.GetFileName( filePath ) );

            if ( objectSetInfo != null )
                textureSetFilePath = Path.Combine( Path.GetDirectoryName( filePath ), objectSetInfo.TextureFileName );

            if ( string.IsNullOrEmpty( textureSetFilePath ) || !File.Exists( textureSetFilePath ) )
            {
                if ( filePath.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
                    textureSetFilePath = $"{filePath.Substring( 0, filePath.Length - 8 )}_tex.bin";

                else if ( filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                    textureSetFilePath = Path.ChangeExtension( filePath, "txd" );
            }

            TextureSet textureSet = null;

            if ( File.Exists( textureSetFilePath ) )
                textureSet = Load<TextureSet>( textureSetFilePath );

            using ( var source = File.OpenRead( filePath ) ) 
                Load( source, textureSet, textureDatabase );
        }

        public override void Load( string filePath )
        {
            Load( filePath, null, null );
        }

        public void Save( Stream destination, ObjectDatabase objectDatabase, TextureDatabase textureDatabase, BoneDatabase boneDatabase, bool leaveOpen = false )
        {
            foreach ( var obj in Objects )
                obj.Id = Format.IsModern() ? MurmurHash.Calculate( obj.Name ) : objectDatabase?.GetObjectInfo( obj.Name )?.Id ?? obj.Id;

            if ( boneDatabase != null )
            {
                foreach ( var obj in Objects.Where( x => x.Skin != null ) )
                {
                    var skeleton = boneDatabase.Skeletons.FirstOrDefault( x =>
                        obj.Name.StartsWith( x.Name, StringComparison.OrdinalIgnoreCase ) );

                    if ( skeleton == null )
                        continue;

                    foreach ( var boneInfo in obj.Skin.Bones )
                    {
                        int index = skeleton.ObjectBoneNames.FindIndex( x =>
                            x.Equals( boneInfo.Name, StringComparison.OrdinalIgnoreCase ) );

                        boneInfo.IsEx = index < 0;

                        if ( !boneInfo.IsEx )
                            boneInfo.Id = ( uint ) index;
                    }
                }
            }

            if ( TextureSet != null )
            {
                var idDictionary = new Dictionary<uint, uint>( TextureSet.Textures.Count );
                uint baseId = textureDatabase != null && textureDatabase.Textures.Count > 0 ? textureDatabase.Textures.Max( x => x.Id ) + 1 : 0;

                foreach ( var texture in TextureSet.Textures )
                {
                    if ( string.IsNullOrEmpty( texture.Name ) )
                        texture.Name = Guid.NewGuid().ToString();

                    uint newId = MurmurHash.Calculate( texture.Name );

                    if ( textureDatabase != null && !Format.IsModern() )
                    {
                        var textureInfo = textureDatabase.GetTextureInfo( texture.Name );

                        if ( textureInfo == null )
                        {
                            newId = baseId++;

                            textureDatabase.Textures.Add( new TextureInfo
                            {
                                Id = newId,
                                Name = texture.Name
                            } );
                        }

                        else
                        {
                            newId = textureInfo.Id;
                        }
                    }

                    else if ( !Format.IsModern() )
                        continue;

                    idDictionary[ texture.Id ] = newId;
                    texture.Id = newId;
                }

                foreach ( var materialTexture in Objects.SelectMany( x => x.Materials )
                    .SelectMany( x => x.MaterialTextures ) )
                {
                    if ( !idDictionary.TryGetValue( materialTexture.TextureId, out uint id ) )
                        continue;

                    materialTexture.TextureId = id;
                }

                TextureIds.Clear();
                TextureIds.AddRange( TextureSet.Textures.Select( x => x.Id ) );
            }

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, ObjectDatabase objectDatabase, TextureDatabase textureDatabase, BoneDatabase boneDatabase )
        {
            // Assume it's being exported for F2nd PS3
            if ( BinaryFormatUtilities.IsClassic( Format ) &&
                 filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.F2nd;
                Endianness = Endianness.Big;
            }

            // Or vice versa
            else if ( BinaryFormatUtilities.IsModern( Format ) &&
                      filePath.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.DT;
                Endianness = Endianness.Little;
            }

            using ( var destination = File.Create( filePath ) ) 
                Save( destination, objectDatabase, textureDatabase, boneDatabase );

            string fileName = Path.GetFileName( filePath );

            // Save on OSI if we are modern
            if ( filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
            {
                var objSetInfo = new ObjectSetInfo();
                objSetInfo.Name = Path.GetFileNameWithoutExtension( filePath ).ToUpperInvariant();
                objSetInfo.Id = MurmurHash.Calculate( objSetInfo.Name );
                objSetInfo.FileName = fileName;
                objSetInfo.TextureFileName = Path.ChangeExtension( fileName, "txd" );
                objSetInfo.ArchiveFileName = Path.ChangeExtension( fileName, "farc" );

                foreach ( var obj in Objects )
                {
                    objSetInfo.Objects.Add( new ObjectInfo
                    {
                        Id = obj.Id,
                        Name = obj.Name.ToUpperInvariant()
                    } );
                }

                var modernObjDatabase = new ObjectDatabase();
                modernObjDatabase.ObjectSets.Add( objSetInfo );
                modernObjDatabase.Format = Format;
                modernObjDatabase.Endianness = Endianness;
                modernObjDatabase.Save( Path.ChangeExtension( filePath, "osi" ) );
            }

            bool exported = false;

            if ( objectDatabase != null && TextureSet != null )
            {
                var objectSetInfo = objectDatabase.GetObjectSetInfoByFileName( fileName );

                if ( objectSetInfo != null )
                {
                    string textureOutputPath = Path.Combine( Path.GetDirectoryName( filePath ),
                        objectSetInfo.TextureFileName );

                    TextureSet.Endianness = Endianness;
                    TextureSet.Format = Format;
                    TextureSet.Save( textureOutputPath );
                    exported = true;
                }
            }

            if ( !exported && TextureSet != null )
            {
                string textureOutputPath = string.Empty;

                // Try to assume a texture output name
                if ( filePath.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
                    textureOutputPath = $"{filePath.Substring( 0, filePath.Length - 8 )}_tex.bin";

                else if ( filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                    textureOutputPath = Path.ChangeExtension( filePath, "txd" );

                if ( !string.IsNullOrEmpty( textureOutputPath ) )
                {
                    TextureSet.Endianness = Endianness;
                    TextureSet.Format = Format;
                    TextureSet.Save( textureOutputPath );
                }
            }
        }

        public override void Save( string filePath )
        {
            Save( filePath, null, null, null );
        }

        public void TryFixParentBoneInfos( BoneDatabase boneDatabase = null )
        {
            foreach ( var obj in Objects )
                obj.Skin?.TryFixParentBoneInfos( boneDatabase?.Skeletons.FirstOrDefault( x => obj.Name.StartsWith( x.Name, StringComparison.OrdinalIgnoreCase ) ) );
        }

        public ObjectSet()
        {
            Objects = new List<Object>();
            TextureIds = new List<uint>();
        }
    }
}