﻿using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MikuMikuLibrary.Objects
{
    public class ObjectSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public override Endianness Endianness
        {
            get
            {
                if ( BinaryFormatUtilities.IsModern( Format ) )
                    return base.Endianness;
                else
                    return Endianness.LittleEndian;
            }

            set
            {
                if ( BinaryFormatUtilities.IsModern( Format ) )
                    base.Endianness = value;
            }
        }

        public List<Object> Objects { get; }
        public List<int> TextureIds { get; }
        public TextureSet TextureSet { get; set; }

        public int BoneCount => Objects.Count != 0 ? 0x39393939 : -1;

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            uint signature = reader.ReadUInt32();
            if ( signature != 0x5062500 && signature != 0x5062501 )
                throw new InvalidDataException( "Invalid signature (expected 0x5062500 or 0x5062501)" );

            int objectCount = reader.ReadInt32();
            int globalBoneCount = reader.ReadInt32();
            long objectsOffset = reader.ReadOffset();
            long objectSkinningsOffset = reader.ReadOffset();
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

            reader.ReadAtOffsetIf( section == null, objectSkinningsOffset, () =>
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
                    obj.Id = reader.ReadInt32();
            } );

            reader.ReadAtOffset( textureIdsOffset, () =>
            {
                TextureIds.Capacity = textureIdCount;
                for ( int i = 0; i < textureIdCount; i++ )
                    TextureIds.Add( reader.ReadInt32() );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( section != null ? 0x5062501 : 0x5062500 );
            writer.Write( Objects.Count );
            writer.Write( section != null ? -1 : BoneCount );

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
                {
                    writer.ScheduleWriteOffsetIf( obj.Skin != null && section == null, 16, AlignmentMode.Left,
                        () => { obj.Skin.Write( writer ); } );
                }
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var obj in Objects )
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left,
                        () => writer.Write( obj.Name, StringBinaryFormat.NullTerminated ) );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var obj in Objects )
                    writer.Write( obj.Id );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var textureId in TextureIds )
                    writer.Write( textureId );
            } );
            writer.Write( TextureIds.Count );
        }

        public void Load( Stream source, TextureSet textureSet, TextureDatabase textureDatabase,
            bool leaveOpen = false )
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

        public void Save( Stream destination, ObjectDatabase objectDatabase, TextureDatabase textureDatabase,
            BoneDatabase boneDatabase, bool leaveOpen = false )
        {
            if ( objectDatabase != null )
            {
                foreach ( var obj in Objects )
                    obj.Id = objectDatabase.GetObjectInfo( obj.Name )?.Id ?? obj.Id;
            }

            if ( boneDatabase != null )
            {
                string fileName = destination is FileStream fileStream
                    ? Path.GetFileName( fileStream.Name )
                    : string.Empty;

                var skeleton =
                    boneDatabase.Skeletons.FirstOrDefault( x =>
                        fileName.StartsWith( x.Name, StringComparison.OrdinalIgnoreCase ) ) ??
                    boneDatabase.Skeletons.FirstOrDefault( x =>
                        x.Name.Equals( "CMN", StringComparison.OrdinalIgnoreCase ) ) ??
                    null;

                if ( skeleton != null )
                {
                    foreach ( var skin in Objects.Where( x => x.Skin != null ).Select( x => x.Skin ) )
                    {
                        foreach ( var boneInfo in skin.Bones )
                        {
                            int index = skin.ExData?.BoneNames?.FindIndex( x =>
                                            x.Equals( boneInfo.Name, StringComparison.OrdinalIgnoreCase ) ) ?? -1;

                            if ( index == -1 )
                                index = skeleton.ObjectBoneNames.FindIndex( x =>
                                    x.Equals( boneInfo.Name, StringComparison.OrdinalIgnoreCase ) );
                            else
                                index = 0x8000 | index;

                            if ( index == -1 )
                                continue;

                            foreach ( var childBone in skin.Bones.Where( x => x.ParentId == boneInfo.Id ) )
                                childBone.ParentId = index;

                            boneInfo.Id = index;
                        }
                    }
                }
            }

            if ( textureDatabase != null && TextureSet != null )
            {
                int id = textureDatabase.Textures.Max( x => x.Id ) + 1;
                var idDictionary = new Dictionary<int, int>( TextureSet.Textures.Count );

                foreach ( var texture in TextureSet.Textures )
                {
                    var textureInfo = !string.IsNullOrEmpty( texture.Name )
                        ? textureDatabase.GetTextureInfo( texture.Name )
                        : textureDatabase.GetTextureInfo( texture.Id );

                    if ( textureInfo == null )
                    {
                        textureDatabase.Textures.Add(
                            textureInfo = new TextureInfo
                            {
                                Name = texture.Name,
                                Id = id++
                            } );

                        if ( string.IsNullOrEmpty( textureInfo.Name ) )
                            textureInfo.Name = $"Unnamed {textureInfo.Id}";
                    }

                    idDictionary[ texture.Id ] = textureInfo.Id;

                    texture.Name = textureInfo.Name;
                    texture.Id = textureInfo.Id;
                }

                foreach ( var materialTexture in Objects.SelectMany( x => x.Materials )
                    .SelectMany( x => x.MaterialTextures ) )
                {
                    if ( idDictionary.TryGetValue( materialTexture.TextureId, out id ) )
                        materialTexture.TextureId = id;
                }

                TextureIds.Clear();
                TextureIds.AddRange( TextureSet.Textures.Select( x => x.Id ) );
            }

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, ObjectDatabase objectDatabase, TextureDatabase textureDatabase,
            BoneDatabase boneDatabase )
        {
            // Assume it's being exported for F2nd PS3
            if ( BinaryFormatUtilities.IsClassic( Format ) &&
                 filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.F2nd;
                Endianness = Endianness.BigEndian;
            }

            // Or reverse
            else if ( BinaryFormatUtilities.IsModern( Format ) &&
                      filePath.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.DT;
                Endianness = Endianness.LittleEndian;
            }

            string fileName = Path.GetFileName( filePath );

            bool exported = false;
            if ( objectDatabase != null && TextureSet != null )
            {
                var objectSetInfo = objectDatabase.GetObjectSetInfoByFileName( fileName );
                if ( objectSetInfo != null )
                {
                    string textureOutputPath =
                        Path.Combine( Path.GetDirectoryName( filePath ), objectSetInfo.TextureFileName );

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
                    TextureSet.Save( textureOutputPath );
            }

            using ( var destination = File.Create( filePath ) )
                Save( destination, objectDatabase, textureDatabase, boneDatabase );
        }

        public ObjectSet()
        {
            Objects = new List<Object>();
            TextureIds = new List<int>();
        }
    }
}
