using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Models;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "MOSD" )]
    public class ModelSection : BinaryFileSection<Model>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        [SubSection( typeof( MeshSection ) )]
        public List<Mesh> Meshes => DataObject.Meshes;

        public ModelSection( SectionMode mode, Model dataObject = null ) : base( mode, dataObject )
        {
        }
    }

    [Section( "OMDL" )]
    public class MeshSection : Section<Mesh>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        [SubSection( typeof( SkinSection ) )]
        public Skin Skin
        {
            get => DataObject.Skin;
            set => DataObject.Skin = value;
        }

        [SubSection]
        public MeshIndexDataSection IndexData { get; set; }

        [SubSection]
        public MeshVertexDataSection VertexData { get; set; }

        protected override void Read( Mesh dataObject, EndianBinaryReader reader, long length ) =>
            dataObject.Read( reader, this );

        protected override void Write( Mesh dataObject, EndianBinaryWriter writer ) =>
            dataObject.Write( writer, this );

        public MeshSection( SectionMode mode, Mesh dataObject = null ) : base( mode, dataObject )
        {
            IndexData = new MeshIndexDataSection( mode, this );
            VertexData = new MeshVertexDataSection( mode, this );
        }
    }


    [Section( "OSKN" )]
    public class SkinSection : Section<Skin>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        protected override void Read( Skin dataObject, EndianBinaryReader reader, long length ) => dataObject.Read( reader );

        protected override void Write( Skin dataObject, EndianBinaryWriter writer ) => dataObject.Write( writer );

        public SkinSection( SectionMode mode, Skin dataObject = null ) : base( mode, dataObject )
        {
        }
    }

    [Section( "OVTX" )]
    public class MeshVertexDataSection : Section<object>
    {
        private readonly List<SubMesh> mSubMeshes;
        private long mCurrentOffset;

        public override SectionFlags Flags => SectionFlags.None;

        public long AddSubMesh( SubMesh subMesh, int stride )
        {
            long current = mCurrentOffset;
            {
                mSubMeshes.Add( subMesh );
                mCurrentOffset += subMesh.Vertices.Length * stride;
            }

            return current;
        }

        protected override void Read( object dataObject, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object dataObject, EndianBinaryWriter writer )
        {
            foreach ( var subMesh in mSubMeshes )
            {
                for ( int i = 0; i < subMesh.Vertices.Length; i++ )
                {
                    writer.Write( subMesh.Vertices?[ i ] ?? Vector3.Zero );
                    writer.Write( subMesh.Normals?[ i ] ?? Vector3.Zero, VectorBinaryFormat.Int16 );
                    writer.WriteNulls( 2 );
                    writer.Write( subMesh.Tangents?[ i ] ?? Vector4.Zero, VectorBinaryFormat.Int16 );
                    writer.Write( subMesh.UVChannel1?[ i ] ?? Vector2.Zero, VectorBinaryFormat.Half );
                    writer.Write( subMesh.UVChannel2?[ i ] ?? subMesh.UVChannel1?[ i ] ?? Vector2.Zero, VectorBinaryFormat.Half );
                    writer.Write( subMesh.Colors?[ i ] ?? Color.White, VectorBinaryFormat.Half );

                    if ( subMesh.BoneWeights != null )
                    {
                        writer.Write( ( ushort )( subMesh.BoneWeights[ i ].Weight1 * 32768f ) );
                        writer.Write( ( ushort )( subMesh.BoneWeights[ i ].Weight2 * 32768f ) );
                        writer.Write( ( ushort )( subMesh.BoneWeights[ i ].Weight3 * 32768f ) );
                        writer.Write( ( ushort )( subMesh.BoneWeights[ i ].Weight4 * 32768f ) );
                        writer.Write( ( byte )( subMesh.BoneWeights[ i ].Index1 * 3 ) );
                        writer.Write( ( byte )( subMesh.BoneWeights[ i ].Index2 * 3 ) );
                        writer.Write( ( byte )( subMesh.BoneWeights[ i ].Index3 * 3 ) );
                        writer.Write( ( byte )( subMesh.BoneWeights[ i ].Index4 * 3 ) );
                    }
                }
            }
        }

        public MeshVertexDataSection( SectionMode mode, object dataObject = null ) : base( mode, dataObject )
        {
            if ( mode == SectionMode.Write )
                mSubMeshes = new List<SubMesh>();
        }
    }

    [Section( "OIDX" )]
    public class MeshIndexDataSection : Section<object>
    {
        private readonly List<ushort[]> mIndices;
        private long mCurrentOffset;

        public override SectionFlags Flags => SectionFlags.None;

        public long AddIndices( ushort[] indices )
        {
            long current = mCurrentOffset;
            {
                mIndices.Add( indices );
                mCurrentOffset += indices.Length * 2;
                mCurrentOffset = AlignmentUtilities.Align( mCurrentOffset, 4 );
            }

            return current;
        }

        protected override void Read( object dataObject, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object dataObject, EndianBinaryWriter writer )
        {
            foreach ( var indices in mIndices )
            {
                writer.Write( indices );
                writer.WriteAlignmentPadding( 4 );
            }
        }

        public MeshIndexDataSection( SectionMode mode, object dataObject = null ) : base( mode, dataObject )
        {
            if ( mode == SectionMode.Write )
                mIndices = new List<ushort[]>();
        }
    }
}