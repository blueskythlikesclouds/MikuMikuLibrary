using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;
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
            get { return Meshes.Where( x => x.Skin != null ).SelectMany( x => x.Skin.Bones.Select( y => y.ID ) ).Distinct().Count(); }
        }

        internal override void Read( EndianBinaryReader reader, Section section = null )
        {
            uint signature = reader.ReadUInt32();
            if ( signature != 0x5062500 && signature != 0x5062501 )
                throw new InvalidDataException( "Invalid signature (expected 0x5062500 or 0x5062501)" );

            int meshCount = reader.ReadInt32();
            int globalBoneCount = reader.ReadInt32();
            uint meshesOffset = reader.ReadUInt32();
            uint meshSkinningsOffset = reader.ReadUInt32();
            uint meshNamesOffset = reader.ReadUInt32();
            uint meshIDsOffset = reader.ReadUInt32();
            uint textureIDsOffset = reader.ReadUInt32();
            int textureIDCount = reader.ReadInt32();

            // We are reading only if there's no section provided.
            // The MeshSection class already parses the meshes.
            // For classic formats, this won't be done, so we will be parsing them ourselves.

            reader.ReadAtOffsetIf( section == null, meshesOffset, () =>
            {
                Meshes.Capacity = meshCount;
                for ( int i = 0; i < meshCount; i++ )
                {
                    reader.ReadAtOffsetAndSeekBack( reader.ReadUInt32(), () =>
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
                    reader.ReadAtOffsetAndSeekBackIfNotZero( reader.ReadUInt32(), () =>
                    {
                        mesh.Skin = new MeshSkin();
                        mesh.Skin.Read( reader );
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

        internal override void Write( EndianBinaryWriter writer, Section section = null )
        {
            writer.Write( section != null ? 0x5062501 : 0x5062500 );
            writer.Write( Meshes.Count );
            writer.Write( section != null ? -1 : BoneCount );

            // Again the same case as the reader here.
            // Section writers will already write the meshes,
            // so we only write for the classic formats in this method.

            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    writer.EnqueueOffsetWriteAlignedIf( section == null, 4, AlignmentKind.Center, () =>
                    {
                        writer.PushBaseOffset();
                        mesh.Write( writer );
                        writer.PopBaseOffset();
                    } );
                }
            } );
            writer.EnqueueOffsetWriteAligned( 16, AlignmentKind.Center, () =>
            {
                foreach ( var mesh in Meshes )
                {
                    writer.EnqueueOffsetWriteAlignedIf( mesh.Skin != null && section == null, 16, AlignmentKind.Left, () =>
                    {
                        mesh.Skin.Write( writer );
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
        }

        public Model()
        {
            Meshes = new List<Mesh>();
            TextureIDs = new List<int>();
        }
    }
}
