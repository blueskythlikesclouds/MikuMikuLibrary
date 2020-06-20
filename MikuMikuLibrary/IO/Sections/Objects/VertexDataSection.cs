using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Misc;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "OVTX" )]
    public class VertexDataSection : Section<object>
    {
        private readonly List<Mesh> mMeshes;
        private long mCurrentOffset;

        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;

        public long AddSubMesh( Mesh mesh, int stride )
        {
            long current = mCurrentOffset;
            {
                mMeshes.Add( mesh );
                mCurrentOffset += mesh.Positions.Length * stride;
            }

            return current;
        }

        protected override void Read( object data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object data, EndianBinaryWriter writer )
        {
            foreach ( var mesh in mMeshes )
            {
                for ( int i = 0; i < mesh.Positions.Length; i++ )
                {
                    writer.Write( ( Vector3 ) mesh.Positions?[ i ] );
                    writer.Write( mesh.Normals?[ i ] ?? Vector3.Zero, VectorBinaryFormat.Int16 );
                    writer.WriteNulls( sizeof( ushort ) );
                    writer.Write( mesh.Tangents?[ i ] ?? Vector4.Zero, VectorBinaryFormat.Int16 );
                    writer.Write( mesh.TexCoords0?[ i ] ?? Vector2.Zero, VectorBinaryFormat.Half );
                    writer.Write( mesh.TexCoords1?[ i ] ?? mesh.TexCoords0?[ i ] ?? Vector2.Zero, VectorBinaryFormat.Half );
                    writer.Write( mesh.Colors0?[ i ] ?? Color.White, VectorBinaryFormat.Half );

                    if ( mesh.BoneWeights == null ) 
                        continue;

                    writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight1 * 32767f ) );
                    writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight2 * 32767f ) );
                    writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight3 * 32767f ) );
                    writer.Write( ( ushort ) ( mesh.BoneWeights[ i ].Weight4 * 32767f ) );

                    writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index1 * 3 ) );
                    writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index2 * 3 ) );
                    writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index3 * 3 ) );
                    writer.Write( ( byte ) ( mesh.BoneWeights[ i ].Index4 * 3 ) );
                }
            }
        }

        public VertexDataSection( SectionMode mode, object data = null ) : base( mode, data )
        {
            if ( mode == SectionMode.Write )
                mMeshes = new List<Mesh>();
        }
    }
}