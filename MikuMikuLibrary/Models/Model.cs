using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MikuMikuLibrary.Models
{
    public class Model : BinaryFile
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

        public List<Mesh> Meshes { get; }
        public List<int> TextureIds { get; }
        public TextureSet TextureSet { get; set; }

        public int BoneCount => Meshes.Count != 0 ? 0x39393939 : -1;

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            uint signature = reader.ReadUInt32();
            if ( signature != 0x5062500 && signature != 0x5062501 )
                throw new InvalidDataException( "Invalid signature (expected 0x5062500 or 0x5062501)" );

            int meshCount = reader.ReadInt32();
            int globalBoneCount = reader.ReadInt32();
            long meshesOffset = reader.ReadOffset();
            long meshSkinningsOffset = reader.ReadOffset();
            long meshNamesOffset = reader.ReadOffset();
            long meshIdsOffset = reader.ReadOffset();
            long textureIdsOffset = reader.ReadOffset();
            int textureIdCount = reader.ReadInt32();

            reader.ReadAtOffsetIf( section == null, meshesOffset, () =>
            {
                Meshes.Capacity = meshCount;
                for ( int i = 0; i < meshCount; i++ )
                {
                    reader.ReadOffset( () =>
                    {
                        reader.PushBaseOffset();
                        {
                            var mesh = new Mesh();
                            mesh.Read( reader );
                            Meshes.Add( mesh );
                        }
                        reader.PopBaseOffset();
                    } );
                }
            } );

            reader.ReadAtOffsetIf( section == null, meshSkinningsOffset, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    reader.ReadOffset( () =>
                    {
                        mesh.Skin = new Skin();
                        mesh.Skin.Read( reader );
                    } );
                }
            } );

            reader.ReadAtOffset( meshNamesOffset, () =>
            {
                foreach ( var mesh in Meshes )
                    mesh.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( meshIdsOffset, () =>
            {
                foreach ( var mesh in Meshes )
                    mesh.Id = reader.ReadInt32();
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
            writer.Write( Meshes.Count );
            writer.Write( section != null ? -1 : BoneCount );

            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    writer.ScheduleWriteOffsetIf( section == null, 4, AlignmentMode.Center, () =>
                    {
                        writer.PushBaseOffset();
                        mesh.Write( writer );
                        writer.PopBaseOffset();
                    } );
                }
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    writer.ScheduleWriteOffsetIf( mesh.Skin != null && section == null, 16, AlignmentMode.Left,
                        () => { mesh.Skin.Write( writer ); } );
                }
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var mesh in Meshes )
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left,
                        () => writer.Write( mesh.Name, StringBinaryFormat.NullTerminated ) );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Center, () =>
            {
                foreach ( var mesh in Meshes )
                    writer.Write( mesh.Id );
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

                var textureEntry = textureDatabase?.GetTexture( TextureIds[ i ] );
                if ( textureEntry != null )
                    TextureSet.Textures[ i ].Name = textureEntry.Name;
            }
        }

        public void Load( string filePath, ObjectDatabase objectDatabase, TextureDatabase textureDatabase )
        {
            string textureSetFilePath = string.Empty;

            var objectEntry = objectDatabase?.GetObjectByFileName( Path.GetFileName( filePath ) );
            if ( objectEntry != null )
                textureSetFilePath = Path.Combine( Path.GetDirectoryName( filePath ), objectEntry.TextureFileName );

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
                foreach ( var mesh in Meshes )
                    mesh.Id = objectDatabase.GetMesh( mesh.Name )?.Id ?? mesh.Id;
            }

            if ( boneDatabase != null )
            {
                string fileName = destination is FileStream fileStream
                    ? Path.GetFileName( fileStream.Name )
                    : string.Empty;

                var skeletonEntry =
                    boneDatabase.Skeletons.FirstOrDefault( x =>
                        fileName.StartsWith( x.Name, StringComparison.OrdinalIgnoreCase ) ) ??
                    boneDatabase.Skeletons.FirstOrDefault( x =>
                        x.Name.Equals( "CMN", StringComparison.OrdinalIgnoreCase ) ) ??
                    boneDatabase.Skeletons[ 0 ];

                if ( skeletonEntry != null )
                {
                    foreach ( var skin in Meshes.Where( x => x.Skin != null ).Select( x => x.Skin ) )
                    {
                        foreach ( var bone in skin.Bones )
                        {
                            int index = skin.ExData?.BoneNames?.FindIndex( x =>
                                            x.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) ) ?? -1;

                            if ( index == -1 )
                                index = skeletonEntry.BoneNames1.FindIndex( x =>
                                    x.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) );
                            else
                                index = 0x8000 | index;

                            if ( index == -1 )
                                continue;

                            foreach ( var childBone in skin.Bones.Where( x => x.ParentId == bone.Id ) )
                                childBone.ParentId = index;

                            bone.Id = index;
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
                    var textureEntry = !string.IsNullOrEmpty( texture.Name )
                        ? textureDatabase.GetTexture( texture.Name )
                        : textureDatabase.GetTexture( texture.Id );

                    if ( textureEntry == null )
                    {
                        textureDatabase.Textures.Add(
                            textureEntry = new TextureEntry
                            {
                                Name = texture.Name,
                                Id = id++
                            } );
                            
                        if ( string.IsNullOrEmpty( textureEntry.Name ) )
                            textureEntry.Name = $"Unnamed {textureEntry.Id}";
                    }
                    idDictionary[ texture.Id ] = textureEntry.Id;

                    texture.Name = textureEntry.Name;
                    texture.Id = textureEntry.Id;
                }

                foreach ( var materialTexture in Meshes.SelectMany( x => x.Materials )
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
                var objectEntry = objectDatabase.GetObjectByFileName( fileName );
                if ( objectEntry != null )
                {
                    string textureOutputPath =
                        Path.Combine( Path.GetDirectoryName( filePath ), objectEntry.TextureFileName );

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

        public Model()
        {
            Meshes = new List<Mesh>();
            TextureIds = new List<int>();
        }
    }
}
