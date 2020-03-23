using System;
using System.Collections.Generic;
using System.Numerics;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Objects;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Objects
{
    public class Mesh
    {
        public BoundingSphere BoundingSphere { get; set; }
        public List<SubMesh> SubMeshes { get; }
        public Vector3[] Vertices { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector4[] Tangents { get; set; }
        public Vector2[] UVChannel1 { get; set; }
        public Vector2[] UVChannel2 { get; set; }
        public Color[] Colors { get; set; }
        public BoneWeight[] BoneWeights { get; set; }
        public string Name { get; set; }

        public static int GetByteSize( BinaryFormat format )
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

        internal void Read( EndianBinaryReader reader, ObjectSection section = null )
        {
            reader.SeekCurrent( 4 );
            BoundingSphere = reader.ReadBoundingSphere();
            int subMeshCount = reader.ReadInt32();
            long subMeshesOffset = reader.ReadOffset();
            var attributes = ( VertexFormatAttribute ) reader.ReadUInt32();
            int stride = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            var elemItems = reader.ReadUInt32s( section?.Format == BinaryFormat.X ? 49 : 28 );
            Name = reader.ReadString( StringBinaryFormat.FixedLength, 64 );

            SubMeshes.Capacity = subMeshCount;
            for ( int i = 0; i < subMeshCount; i++ )
                reader.ReadAtOffset( subMeshesOffset + i * SubMesh.GetByteSize( section?.Format ?? BinaryFormat.DT ),
                    () =>
                    {
                        var subMesh = new SubMesh();
                        subMesh.Read( reader, section );
                        SubMeshes.Add( subMesh );
                    } );

            // Modern Format
            if ( section != null )
                ReadVertexAttributesModern();
            else
                ReadVertexAttributesClassic();

            void ReadVertexAttributesClassic()
            {
                Vector4[] boneWeights = null;
                Vector4[] boneIndices = null;

                for ( int i = 0; i < 28; i++ )
                {
                    var attribute = ( VertexFormatAttribute ) ( 1 << i );

                    reader.ReadAtOffsetIf( ( attributes & attribute ) != 0, elemItems[ i ], () =>
                    {
                        switch ( attribute )
                        {
                            case VertexFormatAttribute.Vertex:
                                Vertices = reader.ReadVector3s( vertexCount );
                                break;

                            case VertexFormatAttribute.Normal:
                                Normals = reader.ReadVector3s( vertexCount );
                                break;

                            case VertexFormatAttribute.Tangent:
                                Tangents = reader.ReadVector4s( vertexCount );
                                break;

                            case VertexFormatAttribute.UVChannel1:
                                UVChannel1 = reader.ReadVector2s( vertexCount );
                                break;

                            case VertexFormatAttribute.UVChannel2:
                                UVChannel2 = reader.ReadVector2s( vertexCount );
                                break;

                            case VertexFormatAttribute.Color:
                                Colors = reader.ReadColors( vertexCount );
                                break;

                            case VertexFormatAttribute.BoneWeight:
                                boneWeights = reader.ReadVector4s( vertexCount );
                                break;

                            case VertexFormatAttribute.BoneIndex:
                                boneIndices = reader.ReadVector4s( vertexCount );
                                break;

                            default:
                                Console.WriteLine( "Unhandled vertex format element: {0}", attribute );
                                break;
                        }
                    } );
                }

                if ( boneWeights != null && boneIndices != null )
                {
                    BoneWeights = new BoneWeight[ vertexCount ];
                    for ( int i = 0; i < vertexCount; i++ )
                    {
                        var weight4 = boneWeights[ i ];
                        var index4 = Vector4.Divide( boneIndices[ i ], 3 );

                        var boneWeight = new BoneWeight
                        {
                            Weight1 = weight4.X,
                            Weight2 = weight4.Y,
                            Weight3 = weight4.Z,
                            Weight4 = weight4.W,
                            Index1 = ( int ) index4.X,
                            Index2 = ( int ) index4.Y,
                            Index3 = ( int ) index4.Z,
                            Index4 = ( int ) index4.W
                        };
                        boneWeight.Validate();

                        BoneWeights[ i ] = boneWeight;
                    }
                }
            }

            void ReadVertexAttributesModern()
            {
                uint dataOffset = elemItems[ section.Format == BinaryFormat.X ? 27 : 13 ];
                uint attributeFlags = elemItems[ section.Format == BinaryFormat.X ? 42 : 21 ];

                if ( attributeFlags == 2 || attributeFlags == 4 )
                {
                    Vertices = new Vector3[ vertexCount ];
                    Normals = new Vector3[ vertexCount ];
                    Tangents = new Vector4[ vertexCount ];
                    UVChannel1 = new Vector2[ vertexCount ];
                    UVChannel2 = new Vector2[ vertexCount ];
                    Colors = new Color[ vertexCount ];

                    if ( attributeFlags == 4 )
                        BoneWeights = new BoneWeight[ vertexCount ];

                    bool hasTangents = false;
                    bool hasUVChannel2 = false;
                    bool hasColors = false;

                    var vertexReader = section.VertexData.Reader;
                    for ( int i = 0; i < vertexCount; i++ )
                    {
                        vertexReader.SeekBegin( section.VertexData.DataOffset + dataOffset + stride * i );
                        Vertices[ i ] = vertexReader.ReadVector3();
                        Normals[ i ] = vertexReader.ReadVector3( VectorBinaryFormat.Int16 );
                        vertexReader.SeekCurrent( 2 );
                        Tangents[ i ] = vertexReader.ReadVector4( VectorBinaryFormat.Int16 );
                        UVChannel1[ i ] = vertexReader.ReadVector2( VectorBinaryFormat.Half );
                        UVChannel2[ i ] = vertexReader.ReadVector2( VectorBinaryFormat.Half );
                        Colors[ i ] = vertexReader.ReadColor( VectorBinaryFormat.Half );

                        if ( attributeFlags == 4 )
                        {
                            var boneWeight = new BoneWeight
                            {
                                Weight1 = vertexReader.ReadUInt16() / 32767f,
                                Weight2 = vertexReader.ReadUInt16() / 32767f,
                                Weight3 = vertexReader.ReadUInt16() / 32767f,
                                Weight4 = vertexReader.ReadUInt16() / 32767f,
                                Index1 = vertexReader.ReadByte() / 3,
                                Index2 = vertexReader.ReadByte() / 3,
                                Index3 = vertexReader.ReadByte() / 3,
                                Index4 = vertexReader.ReadByte() / 3
                            };
                            boneWeight.Validate();

                            BoneWeights[ i ] = boneWeight;
                        }

                        // Normalize normal because precision
                        Normals[ i ] = Vector3.Normalize( Normals[ i ] );

                        // Checks to get rid of useless data after reading
                        if ( Tangents[ i ] != Vector4.Zero ) hasTangents = true;
                        if ( UVChannel1[ i ] != UVChannel2[ i ] ) hasUVChannel2 = true;
                        if ( !Colors[ i ].Equals( Color.White ) ) hasColors = true;
                    }

                    if ( !hasTangents ) Tangents = null;
                    if ( !hasUVChannel2 ) UVChannel2 = null;
                    if ( !hasColors ) Colors = null;
                }

                if ( Tangents != null )
                    for ( int i = 0; i < Tangents.Length; i++ )
                    {
                        int direction = Math.Sign( Tangents[ i ].W );
                        var tangent =
                            Vector3.Normalize( new Vector3( Tangents[ i ].X, Tangents[ i ].Y, Tangents[ i ].Z ) );

                        Tangents[ i ] = new Vector4( tangent, direction );
                    }
            }
        }

        internal void Write( EndianBinaryWriter writer, ObjectSection section = null )
        {
            writer.Write( 0 );
            writer.Write( BoundingSphere );
            writer.Write( SubMeshes.Count );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var subMesh in SubMeshes )
                    subMesh.Write( writer, section );
            } );

            int stride = 0;
            VertexFormatAttribute attributes = default;

            if ( section != null )
            {
                attributes = VertexFormatAttribute.UsesModernStorage;
                if ( BoneWeights != null )
                    stride = 56;
                else
                    stride = 44;
            }

            else
            {
                if ( Vertices != null )
                {
                    attributes |= VertexFormatAttribute.Vertex;
                    stride += 12;
                }

                if ( Normals != null )
                {
                    attributes |= VertexFormatAttribute.Normal;
                    stride += 12;
                }

                if ( Tangents != null )
                {
                    attributes |= VertexFormatAttribute.Tangent;
                    stride += 16;
                }

                if ( UVChannel1 != null )
                {
                    attributes |= VertexFormatAttribute.UVChannel1;
                    stride += 8;
                }

                if ( UVChannel2 != null )
                {
                    attributes |= VertexFormatAttribute.UVChannel2;
                    stride += 8;
                }

                if ( Colors != null )
                {
                    attributes |= VertexFormatAttribute.Color;
                    stride += 16;
                }

                if ( BoneWeights != null )
                {
                    attributes |= VertexFormatAttribute.BoneWeight | VertexFormatAttribute.BoneIndex;
                    stride += 32;
                }
            }

            writer.Write( ( int ) attributes );
            writer.Write( stride );
            writer.Write( Vertices.Length );

            if ( section != null )
                WriteVertexAttributesModern();
            else
                WriteVertexAttributesClassic();

            writer.Write( Name, StringBinaryFormat.FixedLength, 64 );

            void WriteVertexAttributesClassic()
            {
                for ( int i = 0; i < 28; i++ )
                {
                    var attribute = ( VertexFormatAttribute ) ( 1 << i );

                    writer.ScheduleWriteOffsetIf( ( attributes & attribute ) != 0, 4, AlignmentMode.Left, () =>
                    {
                        switch ( attribute )
                        {
                            case VertexFormatAttribute.Vertex:
                                writer.Write( Vertices );
                                break;

                            case VertexFormatAttribute.Normal:
                                writer.Write( Normals );
                                break;

                            case VertexFormatAttribute.Tangent:
                                writer.Write( Tangents );
                                break;

                            case VertexFormatAttribute.UVChannel1:
                                writer.Write( UVChannel1 );
                                break;

                            case VertexFormatAttribute.UVChannel2:
                                writer.Write( UVChannel2 );
                                break;

                            case VertexFormatAttribute.Color:
                                writer.Write( Colors );
                                break;

                            case VertexFormatAttribute.BoneWeight:
                                foreach ( var weight in BoneWeights )
                                {
                                    writer.Write( weight.Weight1 );
                                    writer.Write( weight.Weight2 );
                                    writer.Write( weight.Weight3 );
                                    writer.Write( weight.Weight4 );
                                }

                                break;

                            case VertexFormatAttribute.BoneIndex:
                                foreach ( var weight in BoneWeights )
                                {
                                    writer.Write( weight.Index1 < 0 ? -1f : weight.Index1 * 3.0f );
                                    writer.Write( weight.Index2 < 0 ? -1f : weight.Index2 * 3.0f );
                                    writer.Write( weight.Index3 < 0 ? -1f : weight.Index3 * 3.0f );
                                    writer.Write( weight.Index4 < 0 ? -1f : weight.Index4 * 3.0f );
                                }

                                break;
                        }
                    } );
                }
            }

            void WriteVertexAttributesModern()
            {
                writer.WriteNulls( section.Format == BinaryFormat.X ? 0x6C : 0x34 );
                writer.Write( ( uint ) section.VertexData.AddSubMesh( this, stride ) );
                writer.WriteNulls( section.Format == BinaryFormat.X ? 0x38 : 0x1C );
                writer.Write( BoneWeights != null ? 4 : 2 );
                writer.WriteNulls( 0x18 );
            }
        }

        public Mesh()
        {
            SubMeshes = new List<SubMesh>();
        }

        [Flags]
        private enum VertexFormatAttribute
        {
            Vertex = 1 << 0,
            Normal = 1 << 1,
            Tangent = 1 << 2,
            UVChannel1 = 1 << 4,
            UVChannel2 = 1 << 5,
            Color = 1 << 8,
            BoneWeight = 1 << 10,
            BoneIndex = 1 << 11,
            UsesModernStorage = 1 << 31
        }
    }
}