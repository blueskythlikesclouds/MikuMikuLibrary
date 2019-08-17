using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "OVTX" )]
    public class MeshVertexDataSection : Section<object>
    {
        private readonly List<Mesh> mMeshes;
        private long mCurrentOffset;

        public override SectionFlags Flags => SectionFlags.None;

        public long AddSubMesh( Mesh mesh, int stride )
        {
            long current = mCurrentOffset;
            {
                mMeshes.Add( mesh );
                mCurrentOffset += mesh.Vertices.Length * stride;
            }

            return current;
        }

        protected override void Read( object data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object data, EndianBinaryWriter writer )
        {
            foreach ( var mesh in mMeshes )
                for ( int i = 0; i < mesh.Vertices.Length; i++ )
                {
                    writer.Write( mesh.Vertices?[ i ] ?? Vector3.Zero );
                    writer.Write( mesh.Normals?[ i ] ?? Vector3.Zero, VectorBinaryFormat.Int16 );
                    writer.WriteNulls( 2 );
                    writer.Write( mesh.Tangents?[ i ] ?? Vector4.Zero, VectorBinaryFormat.Int16 );
                    writer.Write( mesh.UVChannel1?[ i ] ?? Vector2.Zero, VectorBinaryFormat.Half );
                    writer.Write( mesh.UVChannel2?[ i ] ?? mesh.UVChannel1?[ i ] ?? Vector2.Zero,
                        VectorBinaryFormat.Half );
                    writer.Write( mesh.Colors?[ i ] ?? Color.White, VectorBinaryFormat.Half );

                    if ( mesh.BoneWeights != null )
                    {
                        writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight1 * 32768f ) );
                        writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight2 * 32768f ) );
                        writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight3 * 32768f ) );
                        writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight4 * 32768f ) );
                        writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index1 * 3 ) );
                        writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index2 * 3 ) );
                        writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index3 * 3 ) );
                        writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index4 * 3 ) );
                    }
                }
        }

        public MeshVertexDataSection( SectionMode mode, object data = null ) : base( mode, data )
        {
            if ( mode == SectionMode.Write )
                mMeshes = new List<Mesh>();
        }
    }
}