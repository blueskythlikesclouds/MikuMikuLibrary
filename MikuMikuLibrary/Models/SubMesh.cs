using MikuMikuLibrary.IO;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.Models
{
    public class SubMesh
    {
        [Flags]
        private enum VertexFormatElement
        {
            Vertex = 1 << 0,
            Normal = 1 << 1,
            Tangent = 1 << 2,
            Flag3 = 1 << 3,
            UVChannel1 = 1 << 4,
            UVChannel2 = 1 << 5,
            Flag6 = 1 << 6,
            Flag7 = 1 << 7,
            Color = 1 << 8,
            Flag9 = 1 << 9,
            BoneWeight = 1 << 10,
            BoneIndex = 1 << 11,
            Flag12 = 1 << 12,
            Flag13 = 1 << 13,
            Flag14 = 1 << 14,
            Flag15 = 1 << 15,
            Flag16 = 1 << 16,
            Flag17 = 1 << 17,
            Flag18 = 1 << 18,
            Flag19 = 1 << 19,
            Flag20 = 1 << 20,
            Flag21 = 1 << 21,
            Flag22 = 1 << 22,
            Flag23 = 1 << 23,
            Flag24 = 1 << 24,
            Flag25 = 1 << 25,
            Flag26 = 1 << 26,
            Flag27 = 1 << 27,
        };

        public const int ByteSize = 0xD8;

        public BoundingSphere BoundingSphere { get; set; }
        public List<IndexTable> IndexTables { get; }
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector4[] Tangents { get; set; }
        public Vector2[] UVChannel1 { get; set; }
        public Vector2[] UVChannel2 { get; set; }
        public Color[] Colors { get; set; }
        public BoneWeight[] BoneWeights { get; set; }
        public string Name { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            BoundingSphere = BoundingSphere.FromReader( reader );
            int indexTableCount = reader.ReadInt32();
            uint indexTablesOffset = reader.ReadUInt32();
            var vfe = ( VertexFormatElement )reader.ReadUInt32();
            int field00 = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            var elemOffsets = reader.ReadUInt32s( 28 );
            Name = reader.ReadString( StringBinaryFormat.FixedLength, 64 );

            IndexTables.Capacity = indexTableCount;
            for ( int i = 0; i < indexTableCount; i++ )
            {
                reader.ReadAtOffset( indexTablesOffset + ( i * IndexTable.ByteSize ), () =>
                {
                    var indexTable = new IndexTable();
                    indexTable.Read( reader );
                    IndexTables.Add( indexTable );
                } );
            }

            Vector4[] boneWeights = null;
            Vector4[] boneIndices = null;

            for ( int i = 0; i < 28; i++ )
            {
                var elem = ( VertexFormatElement )( 1 << i );

                reader.ReadAtOffsetIf( ( vfe & elem ) != 0, elemOffsets[ i ], () =>
                  {
                      switch ( elem )
                      {
                          case VertexFormatElement.Vertex:
                              Vertices = reader.ReadVector3s( vertexCount );
                              break;

                          case VertexFormatElement.Normal:
                              Normals = reader.ReadVector3s( vertexCount );
                              break;

                          case VertexFormatElement.Tangent:
                              Tangents = reader.ReadVector4s( vertexCount );
                              break;

                          case VertexFormatElement.UVChannel1:
                              UVChannel1 = reader.ReadVector2s( vertexCount );
                              break;

                          case VertexFormatElement.UVChannel2:
                              UVChannel2 = reader.ReadVector2s( vertexCount );
                              break;

                          case VertexFormatElement.Color:
                              Colors = reader.ReadColors( vertexCount );
                              break;

                          case VertexFormatElement.BoneWeight:
                              boneWeights = reader.ReadVector4s( vertexCount );
                              break;

                          case VertexFormatElement.BoneIndex:
                              boneIndices = reader.ReadVector4s( vertexCount );
                              break;

                          default:
                              Console.WriteLine( "Unhandled vertex format element: {0}", elem );
                              break;
                      }
                  } );
            }

            if ( boneWeights != null && boneIndices != null )
            {
                BoneWeights = new BoneWeight[ vertexCount ];
                for ( int i = 0; i < vertexCount; i++ )
                {
                    BoneWeights[ i ] = new BoneWeight
                    {
                        Weight1 = boneWeights[ i ].X,
                        Weight2 = boneWeights[ i ].Y,
                        Weight3 = boneWeights[ i ].Z,
                        Weight4 = boneWeights[ i ].W,
                        Index1 = boneIndices[ i ].X == 255.0f ? -1 : ( int )( boneIndices[ i ].X / 3.0f ),
                        Index2 = boneIndices[ i ].Y == 255.0f ? -1 : ( int )( boneIndices[ i ].Y / 3.0f ),
                        Index3 = boneIndices[ i ].Z == 255.0f ? -1 : ( int )( boneIndices[ i ].Z / 3.0f ),
                        Index4 = boneIndices[ i ].W == 255.0f ? -1 : ( int )( boneIndices[ i ].W / 3.0f ),
                    };
                }
            }
        }

        internal void Write( EndianBinaryWriter writer )
        {
            BoundingSphere.Write( writer );
            writer.Write( IndexTables.Count );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                foreach ( var indexTable in IndexTables )
                    indexTable.Write( writer );
            } );

            VertexFormatElement elements = default( VertexFormatElement );

            if ( Vertices != null )
                elements |= VertexFormatElement.Vertex;
            if ( Normals != null )
                elements |= VertexFormatElement.Normal;
            if ( Tangents != null )
                elements |= VertexFormatElement.Tangent;
            if ( UVChannel1 != null )
                elements |= VertexFormatElement.UVChannel1;
            if ( UVChannel2 != null )
                elements |= VertexFormatElement.UVChannel2;
            if ( Colors != null )
                elements |= VertexFormatElement.Color;
            if ( BoneWeights != null )
                elements |=
                    VertexFormatElement.BoneWeight | VertexFormatElement.BoneIndex;

            writer.Write( ( int )elements );
            writer.Write( BoneWeights != null ? 0x40 : 0x20 );
            writer.Write( Vertices.Length );
            for ( int i = 0; i < 28; i++ )
            {
                var elem = ( VertexFormatElement )( 1 << i );

                writer.EnqueueOffsetWriteAlignedIf( ( elements & elem ) != 0, 4, AlignmentKind.Left, () =>
                {
                    switch ( elem )
                    {
                        case VertexFormatElement.Vertex:
                            writer.Write( Vertices );
                            break;

                        case VertexFormatElement.Normal:
                            writer.Write( Normals );
                            break;

                        case VertexFormatElement.Tangent:
                            writer.Write( Tangents );
                            break;

                        case VertexFormatElement.UVChannel1:
                            writer.Write( UVChannel1 );
                            break;

                        case VertexFormatElement.UVChannel2:
                            writer.Write( UVChannel2 );
                            break;

                        case VertexFormatElement.Color:
                            writer.Write( Colors );
                            break;

                        case VertexFormatElement.BoneWeight:
                            foreach ( var weight in BoneWeights )
                            {
                                writer.Write( weight.Weight1 );
                                writer.Write( weight.Weight2 );
                                writer.Write( weight.Weight3 );
                                writer.Write( weight.Weight4 );
                            }
                            break;

                        case VertexFormatElement.BoneIndex:
                            foreach ( var weight in BoneWeights )
                            {
                                writer.Write( weight.Index1 < 0 ? 255.0f : weight.Index1 * 3.0f );
                                writer.Write( weight.Index2 < 0 ? 255.0f : weight.Index2 * 3.0f );
                                writer.Write( weight.Index3 < 0 ? 255.0f : weight.Index3 * 3.0f );
                                writer.Write( weight.Index4 < 0 ? 255.0f : weight.Index4 * 3.0f );
                            }
                            break;
                    }
                } );
            }
            writer.Write( Name, StringBinaryFormat.FixedLength, 64 );
        }

        public SubMesh()
        {
            IndexTables = new List<IndexTable>();
        }
    }
}
