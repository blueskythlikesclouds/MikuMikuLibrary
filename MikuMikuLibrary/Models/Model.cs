using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MikuMikuLibrary.Models
{
    public class Model : BinaryFile
    {
        public override BinaryFileFlags Flags
        {
            get { return BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat; }
        }

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
        public List<int> TextureIDs { get; }
        public TextureSet TextureSet { get; set; }

        public int BoneCount
        {
            get { return Meshes.Count != 0 ? 0x39393939 : -1; }
        }

        public override void Read( EndianBinaryReader reader, Section section = null )
        {
            uint signature = reader.ReadUInt32();
            if ( signature != 0x5062500 && signature != 0x5062501 )
                throw new InvalidDataException( "Invalid signature (expected 0x5062500 or 0x5062501)" );

            int meshCount = reader.ReadInt32();
            int globalBoneCount = reader.ReadInt32();
            long meshesOffset = reader.ReadOffset();
            long meshSkinningsOffset = reader.ReadOffset();
            long meshNamesOffset = reader.ReadOffset();
            long meshIDsOffset = reader.ReadOffset();
            long textureIDsOffset = reader.ReadOffset();
            int textureIDCount = reader.ReadInt32();

            // We are reading only if there's no section provided.
            // The MeshSection class already parses the meshes.
            // For classic formats, this won't be done, so we will be parsing them ourselves.

            reader.ReadAtOffsetIf( section == null, meshesOffset, () =>
            {
                Meshes.Capacity = meshCount;
                for ( int i = 0; i < meshCount; i++ )
                {
                    reader.ReadAtOffset( reader.ReadUInt32(), () =>
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
                    reader.ReadAtOffset( reader.ReadUInt32(), () =>
                    {
                        mesh.Skin = new MeshSkin();
                        mesh.Skin.Read( reader );
                    } );
                }
            } );

            reader.ReadAtOffset( meshNamesOffset, () =>
            {
                foreach ( var mesh in Meshes )
                    mesh.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( meshIDsOffset, () =>
            {
                foreach ( var mesh in Meshes )
                    mesh.ID = reader.ReadInt32();
            } );

            reader.ReadAtOffset( textureIDsOffset, () =>
            {
                TextureIDs.Capacity = textureIDCount;
                for ( int i = 0; i < textureIDCount; i++ )
                    TextureIDs.Add( reader.ReadInt32() );
            } );
        }

        public override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( section != null ? 0x5062501 : 0x5062500 );
            writer.Write( Meshes.Count );
            writer.Write( section != null ? -1 : BoneCount );

            // Again the same case as the reader here.
            // Section writers will already write the meshes,
            // so we only write for the classic formats in this method.

            writer.EnqueueOffsetWrite( 16, AlignmentKind.Center, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    writer.EnqueueOffsetWriteIf( section == null, 4, AlignmentKind.Center, () =>
                    {
                        writer.PushBaseOffset();
                        mesh.Write( writer );
                        writer.PopBaseOffset();
                    } );
                }
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Center, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    writer.EnqueueOffsetWriteIf( mesh.Skin != null && section == null, 16, AlignmentKind.Left, () =>
                    {
                        mesh.Skin.Write( writer );
                    } );
                }
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Center, () =>
            {
                foreach ( var mesh in Meshes )
                    writer.EnqueueOffsetWrite( 16, AlignmentKind.Left, () => writer.Write( mesh.Name, StringBinaryFormat.NullTerminated ) );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Center, () =>
            {
                foreach ( var mesh in Meshes )
                    writer.Write( mesh.ID );
            } );
            writer.EnqueueOffsetWrite( 16, AlignmentKind.Center, () =>
            {
                foreach ( var textureID in TextureIDs )
                    writer.Write( textureID );
            } );
            writer.Write( TextureIDs.Count );
        }

        public void Load( Stream source, TextureSet textureSet, TextureDatabase textureDatabase, bool leaveOpen = false )
        {
            Load( source, leaveOpen );

            if ( textureSet != null && TextureIDs.Count <= textureSet.Textures.Count )
            {
                TextureSet = textureSet;
                for ( int i = 0; i < TextureIDs.Count; i++ )
                {
                    TextureSet.Textures[ i ].ID = TextureIDs[ i ];

                    var textureEntry = textureDatabase?.GetTexture( TextureIDs[ i ] );
                    if ( textureEntry != null )
                        TextureSet.Textures[ i ].Name = textureEntry.Name;
                }
            }
        }

        public void Load( string filePath, ObjectDatabase objectDatabase, TextureDatabase textureDatabase )
        {
            string textureFilePath = string.Empty;
            if ( objectDatabase != null )
            {
                var objectEntry = objectDatabase.GetObjectByFileName( Path.GetFileName( filePath ) );
                if ( objectEntry != null )
                    textureFilePath = Path.Combine( Path.GetDirectoryName( filePath ), objectEntry.TextureFileName );
            }

            if ( string.IsNullOrEmpty( textureFilePath ) || !File.Exists( textureFilePath ) )
            {
                // Try to assume the file name
                if ( filePath.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
                    textureFilePath = filePath.Substring( 0, filePath.Length - 8 ) + "_tex.bin";

                else if ( filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                    textureFilePath = Path.ChangeExtension( filePath, "txd" );
            }

            TextureSet textureSet = null;
            if ( !string.IsNullOrEmpty( textureFilePath ) && File.Exists( textureFilePath ) )
                textureSet = Load<TextureSet>( textureFilePath );

            using ( var source = File.OpenRead( filePath ) )
                Load( source, textureSet, textureDatabase, false );
        }

        public void Save( Stream destination, ObjectDatabase objectDatabase, TextureDatabase textureDatabase, BoneDatabase boneDatabase, bool leaveOpen = false )
        {
            if ( objectDatabase != null )
            {
                foreach ( var mesh in Meshes )
                    mesh.ID = objectDatabase.GetMesh( mesh.Name )?.ID ?? mesh.ID;
            }

            if ( boneDatabase != null )
            {
                string fileName = ( destination is FileStream fileStream ) ? Path.GetFileName( fileStream.Name ) : string.Empty;

                // Assume we are exporting in game's style
                var skeleton = boneDatabase.Skeletons.FirstOrDefault( x => fileName.StartsWith( x.Name, StringComparison.OrdinalIgnoreCase ) );

                // If we couldn't find it, default to CMN skeleton
                if ( skeleton == null )
                    skeleton = boneDatabase.Skeletons.FirstOrDefault( x => x.Name.Equals( "CMN", StringComparison.OrdinalIgnoreCase ) );

                // Still?? Then default to the first skeleton (this is unlikely to happen though)
                if ( skeleton == null )
                    skeleton = boneDatabase.Skeletons[ 0 ];

                // Pretty much impossible to miss
                if ( skeleton != null )
                {
                    foreach ( var skin in Meshes.Where( x => x.Skin != null ).Select( x => x.Skin ) )
                    {
                        foreach ( var bone in skin.Bones )
                        {
                            int index = skin.ExData?.BoneNames?.FindIndex( x => x.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) ) ?? -1;
                            if ( index == -1 )
                                index = skeleton.BoneNames1.FindIndex( x => x.Equals( bone.Name, StringComparison.OrdinalIgnoreCase ) );
                            else
                                index = 0x8000 | index;
  
                            if ( index != -1 )
                            {
                                // Before we do this, fix the child bones
                                foreach ( var childBone in skin.Bones.Where( x => x.ParentID.Equals( bone.ID ) ) )
                                    childBone.ParentID = index;

                                // Now replace the ID
                                bone.ID = index;
                            }
                            else
                            {
                                Debug.WriteLine( $"Model.Save: Bone wasn't found in bone database or ex-data: {bone.Name}" );
                            }
                        }
                    }
                }
            }

            if ( textureDatabase != null && TextureSet != null )
            {
                var newIDs = new List<int>( TextureSet.Textures.Count );
                int currentID = textureDatabase.Textures.Max( x => x.ID ) + 1;
                foreach ( var texture in TextureSet.Textures )
                {
                    var textureEntry = !string.IsNullOrEmpty( texture.Name ) ?
                        textureDatabase.GetTexture( texture.Name ) : textureDatabase.GetTexture( texture.ID );

                    if ( textureEntry == null )
                    {
                        textureDatabase.Textures.Add( textureEntry = new TextureEntry
                        {
                            ID = currentID++,
                            Name = texture.Name ?? $"Texture{currentID}",
                        } );
                    }

                    newIDs.Add( textureEntry.ID );
                }

                if ( !newIDs.SequenceEqual( TextureIDs ) )
                {
                    Texture.ReAssignTextureIDs( this, newIDs );
                }
            }

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, ObjectDatabase objectDatabase, TextureDatabase textureDatabase, BoneDatabase boneDatabase )
        {
            // Assume it's being exported for F2nd PS3
            if ( BinaryFormatUtilities.IsClassic( Format ) && filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.F2nd;
                Endianness = Endianness.BigEndian;
            }

            // Or reverse
            else if ( BinaryFormatUtilities.IsModern( Format ) && filePath.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                Format = BinaryFormat.DT;
                Endianness = Endianness.LittleEndian;
            }

            var fileName = Path.GetFileName( filePath );

            bool exported = false;
            if ( objectDatabase != null && TextureSet != null )
            {
                var objectEntry = objectDatabase.GetObjectByFileName( fileName );
                if ( objectEntry != null )
                {
                    var textureOutputPath = Path.Combine( Path.GetDirectoryName( filePath ), objectEntry.TextureFileName );
                    TextureSet.Save( textureOutputPath );
                    exported = true;
                }
            }

            if ( !exported && TextureSet != null )
            {
                var textureOutputPath = string.Empty;

                // Try to assume a texture output name
                if ( filePath.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
                    textureOutputPath = filePath.Substring( 0, filePath.Length - 8 ) + "_tex.bin";

                else if ( filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                    textureOutputPath = Path.ChangeExtension( filePath, "txd" );

                if ( !string.IsNullOrEmpty( textureOutputPath ) )
                    TextureSet.Save( textureOutputPath );
            }

            using ( var destination = File.Create( filePath ) )
                Save( destination, objectDatabase, textureDatabase, boneDatabase, false );
        }

        public Model()
        {
            Meshes = new List<Mesh>();
            TextureIDs = new List<int>();
        }
    }
}
