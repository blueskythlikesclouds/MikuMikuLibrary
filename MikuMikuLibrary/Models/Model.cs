using MikuMikuLibrary.IO;
using MikuMikuLibrary.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MikuMikuLibrary.Models
{
    public class Model : BinaryFile
    {
        public List<Mesh> Meshes { get; }
        public List<int> TextureIDs { get; }
        public TextureSet TextureSet { get; }

        public override bool CanLoad
        {
            get { return true; }
        }

        public override bool CanSave
        {
            get { return true; }
        }

        public int BoneCount
        {
            get { return Meshes.SelectMany( x => x.Bones.Select( y => y.ID ) ).Distinct().Count(); }
        }

        protected override void Read( Stream source )
        {
            using ( var reader = new EndianBinaryReader( source, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                uint signature = reader.ReadUInt32();
                if ( signature != 0x5062500 )
                    throw new InvalidDataException( "Invalid signature (expected 0x5062500)" );

                int meshCount = reader.ReadInt32();
                int globalBoneCount = reader.ReadInt32();
                uint meshesOffset = reader.ReadUInt32();
                uint meshSkinningsOffset = reader.ReadUInt32();
                uint meshNamesOffset = reader.ReadUInt32();
                uint meshIDsOffset = reader.ReadUInt32();
                uint textureIDsOffset = reader.ReadUInt32();
                int textureIDCount = reader.ReadInt32();

                reader.ReadAtOffset( meshesOffset, () =>
                {
                    Meshes.Capacity = meshCount;
                    for ( int i = 0; i < meshCount; i++ )
                    {
                        reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
                        {
                            var mesh = new Mesh();
                            mesh.Read( reader );
                            Meshes.Add( mesh );
                        } );
                    }
                } );

                reader.ReadAtOffset( meshSkinningsOffset, () =>
                {
                    foreach ( var mesh in Meshes )
                    {
                        reader.ReadAtOffsetAndSeekBackIfNotZero( reader.ReadUInt32(), () =>
                        {
                            uint boneIDsOffset = reader.ReadUInt32();
                            uint boneMatricesOffset = reader.ReadUInt32();
                            uint boneNamesOffset = reader.ReadUInt32();
                            uint meshExDataOffset = reader.ReadUInt32();
                            int boneCount = reader.ReadInt32();
                            uint boneParentIDsOffset = reader.ReadUInt32();

                            reader.ReadAtOffset( boneIDsOffset, () =>
                            {
                                mesh.Bones.Capacity = boneCount;
                                for ( int i = 0; i < boneCount; i++ )
                                {
                                    var bone = new Bone();
                                    bone.ID = reader.ReadInt32();
                                    mesh.Bones.Add( bone );
                                }
                            } );

                            reader.ReadAtOffset( boneMatricesOffset, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    bone.Matrix = reader.ReadMatrix4x4();
                            } );

                            reader.ReadAtOffset( boneNamesOffset, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    bone.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
                            } );

                            reader.ReadAtOffsetIfNotZero( meshExDataOffset, () =>
                            {
                                mesh.ExData = new MeshExData();
                                mesh.ExData.Read( reader );
                            } );

                            reader.ReadAtOffsetIfNotZero( boneParentIDsOffset, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    bone.ParentID = reader.ReadInt32();
                            } );
                        } );
                    }
                } );

                reader.ReadAtOffset( meshNamesOffset, () =>
                {
                    foreach ( var mesh in Meshes )
                        mesh.Name = reader.ReadStringPtr( StringBinaryFormat.NullTerminated );
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
        }

        protected override void Write( Stream destination )
        {
            using ( var writer = new EndianBinaryWriter( destination, Encoding.UTF8, true, Endianness.LittleEndian ) )
            {
                writer.Write( 0x5062500 );
                writer.Write( Meshes.Count );
                writer.Write( BoneCount );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                {
                    foreach ( var mesh in Meshes )
                        writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Center, () => mesh.Write( writer ) );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                {
                    foreach ( var mesh in Meshes )
                    {
                        writer.EnqueueOffsetWriteAlignedIf( mesh.Bones.Count > 0, 16, AlignmentKind.Left, () =>
                        {
                            writer.PushStringTableAligned( 16, AlignmentKind.Center, StringBinaryFormat.NullTerminated );
                            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    writer.Write( bone.ID );
                            } );
                            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    writer.Write( bone.Matrix );
                            } );
                            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    writer.AddStringToStringTable( bone.Name );
                            } );
                            writer.EnqueueOffsetWriteAlignedIf( mesh.ExData != null, 16, AlignmentKind.Center, () => mesh.ExData.Write( writer ) );
                            writer.Write( mesh.Bones.Count );
                            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                            {
                                foreach ( var bone in mesh.Bones )
                                    writer.Write( bone.ParentID );

                                writer.PopStringTable();
                            } );
                            writer.WriteNulls( 40 );
                        } );
                    }
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                {
                    foreach ( var mesh in Meshes )
                        writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Left, () => writer.Write( mesh.Name, StringBinaryFormat.NullTerminated ) );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                {
                    foreach ( var mesh in Meshes )
                        writer.Write( mesh.ID );
                } );
                writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
                {
                    foreach ( var textureID in TextureIDs )
                        writer.Write( textureID );
                } );
                writer.Write( TextureIDs.Count );
                writer.DoEnqueuedOffsetWrites();
                writer.PopStringTablesReversed();
            }
        }

        public override void Load( string fileName )
        {
            base.Load( fileName );

            if ( fileName.EndsWith( "_obj.bin", StringComparison.OrdinalIgnoreCase ) )
            {
                var textureFileName =
                    fileName.Substring( 0, fileName.Length - 8 ) + "_tex.bin";

                if ( File.Exists( textureFileName ) )
                {
                    TextureSet.Load( textureFileName );

                    for ( int i = 0; i < TextureSet.Textures.Count; i++ )
                        TextureSet.Textures[ i ].ID = TextureIDs[ i ];
                }
            }
        }

        public Model()
        {
            Meshes = new List<Mesh>();
            TextureSet = new TextureSet();
            TextureIDs = new List<int>();
        }
    }
}
