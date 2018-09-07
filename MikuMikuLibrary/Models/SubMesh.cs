using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Misc;
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
            IsModern = 1 << 31,
        };

        public static int ByteSize( BinaryFormat format )
        {
            switch ( format )
            {
                case BinaryFormat.DT:
                case BinaryFormat.F:
                case BinaryFormat.FT:
                case BinaryFormat.F2nd:
                    return 0xD8;

                case BinaryFormat.X:
                    return 0x130;
            }

            throw new ArgumentException( nameof( format ) );
        }

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

        internal void Read( EndianBinaryReader reader, MeshSection section = null )
        {
            reader.SeekCurrent( 4 );
            BoundingSphere = reader.ReadBoundingSphere();
            int indexTableCount = reader.ReadInt32();
            long indexTablesOffset = reader.ReadOffset();
            var vfe = ( VertexFormatElement )reader.ReadUInt32();
            int stride = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            var elemItems = reader.ReadUInt32s( section?.Format == IO.BinaryFormat.X ? 49 : 28 );
            Name = reader.ReadString( StringBinaryFormat.FixedLength, 64 );

            IndexTables.Capacity = indexTableCount;
            for ( int i = 0; i < indexTableCount; i++ )
            {
                reader.ReadAtOffset( indexTablesOffset + ( i * IndexTable.ByteSize( section?.Format ?? BinaryFormat.DT ) ), () =>
                {
                    var indexTable = new IndexTable();
                    indexTable.Read( reader, section );
                    IndexTables.Add( indexTable );
                } );
            }

            // Modern Format
            if ( section != null )
            {
                uint dataOffset = elemItems[ section.Format == BinaryFormat.X ? 27 : 13 ];
                uint mode = elemItems[ section.Format == BinaryFormat.X ? 42 : 21 ];

                if ( mode == 2 || mode == 4 )
                {
                    Vertices = new Vector3[ vertexCount ];
                    Normals = new Vector3[ vertexCount ];
                    UVChannel1 = new Vector2[ vertexCount ];
                    UVChannel2 = new Vector2[ vertexCount ];
                    Colors = new Color[ vertexCount ];

                    if ( mode == 4 )
                        BoneWeights = new BoneWeight[ vertexCount ];

                    var vertexReader = section.VertexData.Reader;
                    for ( int i = 0; i < vertexCount; i++ )
                    {
                        vertexReader.SeekBegin( dataOffset + ( stride * i ) );
                        Vertices[ i ] = vertexReader.ReadVector3();
                        Normals[ i ] = vertexReader.ReadVector3Int16();
                        vertexReader.SeekCurrent( 10 );
                        UVChannel1[ i ] = vertexReader.ReadVector2Half();
                        UVChannel2[ i ] = vertexReader.ReadVector2Half();
                        Colors[ i ] = vertexReader.ReadColorHalf();

                        if ( mode == 4 )
                        {
                            BoneWeights[ i ].Weight1 = vertexReader.ReadUInt16() / 32768f;
                            BoneWeights[ i ].Weight2 = vertexReader.ReadUInt16() / 32768f;
                            BoneWeights[ i ].Weight3 = vertexReader.ReadUInt16() / 32768f;
                            BoneWeights[ i ].Weight4 = vertexReader.ReadUInt16() / 32768f;
                            BoneWeights[ i ].Index1 = vertexReader.ReadByte() / 3;
                            BoneWeights[ i ].Index2 = vertexReader.ReadByte() / 3;
                            BoneWeights[ i ].Index3 = vertexReader.ReadByte() / 3;
                            BoneWeights[ i ].Index4 = vertexReader.ReadByte() / 3;
                        }
                    }
                }
            }

            else
            {
                Vector4[] boneWeights = null;
                Vector4[] boneIndices = null;

                for ( int i = 0; i < 28; i++ )
                {
                    var elem = ( VertexFormatElement )( 1 << i );

                    reader.ReadAtOffsetIf( ( vfe & elem ) != 0, elemItems[ i ], () =>
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
        }

        internal void Write( EndianBinaryWriter writer, MeshSection section = null )
        {
            writer.Write( 0 );
            writer.Write( BoundingSphere );
            writer.Write( IndexTables.Count );
            writer.EnqueueOffsetWrite( 4, AlignmentKind.Left, () =>
            {
                foreach ( var indexTable in IndexTables )
                    indexTable.Write( writer, section );
            } );

            int stride = 0;
            VertexFormatElement elements = default( VertexFormatElement );

            if ( section != null )
            {
                elements = VertexFormatElement.IsModern;
                if ( BoneWeights != null )
                    stride = 56;
                else
                    stride = 44;
            }

            else
            {
                if ( Vertices != null )
                {
                    elements |= VertexFormatElement.Vertex;
                    stride += 12;
                }

                if ( Normals != null )
                {
                    elements |= VertexFormatElement.Normal;
                    stride += 12;
                }

                if ( Tangents != null )
                {
                    elements |= VertexFormatElement.Tangent;
                    stride += 16;
                }

                if ( UVChannel1 != null )
                {
                    elements |= VertexFormatElement.UVChannel1;
                    stride += 8;
                }

                if ( UVChannel2 != null )
                {
                    elements |= VertexFormatElement.UVChannel2;
                    stride += 8;
                }

                if ( Colors != null )
                {
                    elements |= VertexFormatElement.Color;
                    stride += 16;
                }

                if ( BoneWeights != null )
                {
                    elements |= VertexFormatElement.BoneWeight | VertexFormatElement.BoneIndex;
                    stride += 32;
                }
            }

            writer.Write( ( int )elements );
            writer.Write( stride );
            writer.Write( Vertices.Length );

            if ( section != null )
            {
                long vertexPosition = section.VertexData.Data.Position;
                var vertexWriter = section.VertexData.Writer;

                for ( int i = 0; i < Vertices.Length; i++ )
                {
                    // Should I even do it like this? lol
                    vertexWriter.Write( Vertices?[ i ] ?? Vector3.Zero );
                    vertexWriter.WriteVector3Int16( Normals?[ i ] ?? Vector3.Zero );
                    vertexWriter.WriteNulls( 10 );
                    vertexWriter.WriteVector2Half( UVChannel1?[ i ] ?? Vector2.One );
                    vertexWriter.WriteVector2Half( UVChannel2?[ i ] ?? Vector2.One );
                    vertexWriter.WriteColorHalf( Colors?[ i ] ?? Color.One );

                    if ( BoneWeights != null )
                    {
                        vertexWriter.Write( ( ushort )( BoneWeights[ i ].Weight1 * 32768f ) );
                        vertexWriter.Write( ( ushort )( BoneWeights[ i ].Weight2 * 32768f ) );
                        vertexWriter.Write( ( ushort )( BoneWeights[ i ].Weight3 * 32768f ) );
                        vertexWriter.Write( ( ushort )( BoneWeights[ i ].Weight4 * 32768f ) );
                        vertexWriter.Write( ( byte )( BoneWeights[ i ].Index1 * 3 ) );
                        vertexWriter.Write( ( byte )( BoneWeights[ i ].Index2 * 3 ) );
                        vertexWriter.Write( ( byte )( BoneWeights[ i ].Index3 * 3 ) );
                        vertexWriter.Write( ( byte )( BoneWeights[ i ].Index4 * 3 ) );
                    }
                }

                writer.WriteNulls( 0x34 );
                writer.Write( ( uint )vertexPosition );
                writer.WriteNulls( 0x1C );
                writer.Write( BoneWeights != null ? 4 : 2 );
                writer.WriteNulls( 0x18 );
            }

            else
            {
                for ( int i = 0; i < 28; i++ )
                {
                    var elem = ( VertexFormatElement )( 1 << i );

                    writer.EnqueueOffsetWriteIf( ( elements & elem ) != 0, 4, AlignmentKind.Left, () =>
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
            }

            writer.Write( Name, StringBinaryFormat.FixedLength, 64 );
        }

        public SubMesh()
        {
            IndexTables = new List<IndexTable>();
        }
    }
}
