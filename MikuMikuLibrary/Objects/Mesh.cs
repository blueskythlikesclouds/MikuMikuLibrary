using System;
using System.Collections.Generic;
using System.Numerics;
using Assimp.Configs;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Objects;
using MikuMikuLibrary.Misc;

namespace MikuMikuLibrary.Objects
{
    [Flags]
    public enum MeshFlags
    {
        FaceCameraPosition = 1 << 1,
        FaceCameraView = 1 << 3
    }

    public class Mesh
    {
        public BoundingSphere BoundingSphere { get; set; }
        public List<SubMesh> SubMeshes { get; }
        public Vector3[] Positions { get; set; }
        public Vector3[] Normals { get; set; }
        public Vector4[] Tangents { get; set; }
        public Vector2[] TexCoords0 { get; set; }
        public Vector2[] TexCoords1 { get; set; }
        public Vector2[] TexCoords2 { get; set; }
        public Vector2[] TexCoords3 { get; set; }
        public Color[] Colors0 { get; set; }
        public Color[] Colors1 { get; set; }
        public BoneWeight[] BoneWeights { get; set; }
        public MeshFlags Flags { get; set; }
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

        public Vector2[] GetTexCoordsChannel( int index )
        {
            switch ( index )
            {
                case 0: return TexCoords0;
                case 1: return TexCoords1;
                case 2: return TexCoords2;
                case 3: return TexCoords3;

                default:
                    throw new ArgumentOutOfRangeException( nameof( index ) );
            }
        }

        public void SetTexCoordsChannel( int index, Vector2[] texCoords )
        {
            switch ( index )
            {
                case 0:
                    TexCoords0 = texCoords;
                    break;

                case 1:
                    TexCoords1 = texCoords;
                    break;

                case 2:
                    TexCoords2 = texCoords;
                    break;

                case 3:
                    TexCoords3 = texCoords;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( index ) );
            }
        }

        public Color[] GetColorsChannel( int index )
        {
            switch ( index )
            {
                case 0: return Colors0;
                case 1: return Colors1;

                default:
                    throw new ArgumentOutOfRangeException( nameof( index ) );
            }
        }

        public void SetColorsChannel( int index, Color[] colors )
        {
            switch ( index )
            {
                case 0:
                    Colors0 = colors;
                    break;

                case 1:
                    Colors1 = colors;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( index ) );
            }
        }

        public void GenerateTangents()
        {
            if ( Positions == null || Normals == null || TexCoords0 == null )
                return;

            var tangents = new Vector3[ Positions.Length ];
            var bitangents = new Vector3[ Positions.Length ];

            foreach ( var subMesh in SubMeshes )
            {
                foreach ( var triangle in subMesh.GetTriangles() )
                {
                    var positionA = Positions[ triangle.C ] - Positions[ triangle.A ];
                    var positionB = Positions[ triangle.B ] - Positions[ triangle.A ];

                    var texCoordA = TexCoords0[ triangle.C ] - TexCoords0[ triangle.A ];
                    var texCoordB = TexCoords0[ triangle.B ] - TexCoords0[ triangle.A ];

                    float direction = texCoordA.X * texCoordB.Y - texCoordA.Y * texCoordB.X > 0.0f ? 1.0f : -1.0f;

                    var tangent = ( positionA * texCoordB.Y - positionB * texCoordA.Y ) * direction;
                    var bitangent = ( positionB * texCoordA.X - positionA * texCoordB.X ) * direction;

                    tangents[ triangle.A ] += tangent;
                    tangents[ triangle.B ] += tangent;
                    tangents[ triangle.C ] += tangent;

                    bitangents[ triangle.A ] += bitangent;
                    bitangents[ triangle.B ] += bitangent;
                    bitangents[ triangle.C ] += bitangent;
                }
            }

            Tangents = new Vector4[ Positions.Length ];

            for ( int i = 0; i < tangents.Length; i++ )
            {
                var normal = Normals[ i ];

                var tangent = Vector3.Normalize( tangents[ i ] );
                var bitangent = Vector3.Normalize( bitangents[ i ] );

                tangent = Vector3.Normalize( tangent - normal * Vector3.Dot( tangent, normal ) );
                bitangent = Vector3.Normalize( bitangent - normal * Vector3.Dot( bitangent, normal ) );

                float directionCheck = Vector3.Dot( Vector3.Normalize( Vector3.Cross( normal, tangent ) ), bitangent );
                Tangents[ i ] = new Vector4( tangent, directionCheck > 0.0f ? 1.0f : -1.0f );
            }

            // Look for NaNs
            for ( int i = 0; i < Positions.Length; i++ )
            {
                var position = Positions[ i ];
                var tangent = Tangents[ i ];

                if ( !float.IsNaN( tangent.X ) && !float.IsNaN( tangent.Y ) && !float.IsNaN( tangent.Z ) )
                    continue;

                int nearestVertexIndex = -1;
                float currentDistance = float.PositiveInfinity;

                for ( int j = 0; j < Positions.Length; j++ )
                {
                    var positionToCompare = Positions[ j ];
                    var tangentToCompare = Tangents[ j ];

                    if ( i == j || float.IsNaN( tangentToCompare.X ) || float.IsNaN( tangentToCompare.Y ) || float.IsNaN( tangentToCompare.Z ) )
                        continue;

                    float distance = Vector3.DistanceSquared( position, positionToCompare );

                    if ( distance > currentDistance ) 
                        continue;

                    nearestVertexIndex = j;
                    currentDistance = distance;
                }

                if ( nearestVertexIndex != -1 )
                    Tangents[ i ] = Tangents[ nearestVertexIndex ];
            }
        }

        internal void Read( EndianBinaryReader reader, ObjectSection section = null )
        {
            reader.SeekCurrent( 4 ); // Unused flags

            BoundingSphere = reader.ReadBoundingSphere();

            int subMeshCount = reader.ReadInt32();
            long subMeshesOffset = reader.ReadOffset();

            var vertexFormat = ( VertexFormatAttributes ) reader.ReadUInt32();
            int vertexSize = reader.ReadInt32();
            int vertexCount = reader.ReadInt32();
            var attributeOffsets = reader.ReadOffsets( 20 );

            Flags = ( MeshFlags ) reader.ReadInt32();

            uint attributeFlags = reader.ReadUInt32();

            reader.SkipNulls( 6 * sizeof( uint ) );

            Name = reader.ReadString( StringBinaryFormat.FixedLength, 64 );

            SubMeshes.Capacity = subMeshCount;

            for ( int i = 0; i < subMeshCount; i++ )
            {
                reader.ReadAtOffset( subMeshesOffset + i * SubMesh.GetByteSize( section?.Format ?? BinaryFormat.DT ), () =>
                {
                    var subMesh = new SubMesh();
                    subMesh.Read( reader, section );
                    SubMeshes.Add( subMesh );
                } );
            }

            // Modern Format
            if ( section != null )
                ReadVertexAttributesModern();

            else
                ReadVertexAttributesClassic();

            void ReadVertexAttributesClassic()
            {
                Vector4[] boneWeights = null;
                Vector4[] boneIndices = null;

                for ( int i = 0; i < attributeOffsets.Length; i++ )
                {
                    var attribute = ( VertexFormatAttributes ) ( 1 << i );

                    reader.ReadAtOffsetIf( ( vertexFormat & attribute ) != 0, attributeOffsets[ i ], () =>
                    {
                        switch ( attribute )
                        {
                            case VertexFormatAttributes.Position:
                                Positions = reader.ReadVector3s( vertexCount );
                                break;

                            case VertexFormatAttributes.Normal:
                                Normals = reader.ReadVector3s( vertexCount );
                                break;

                            case VertexFormatAttributes.Tangent:
                                Tangents = reader.ReadVector4s( vertexCount );
                                break;

                            case VertexFormatAttributes.TexCoord0:
                                TexCoords0 = reader.ReadVector2s( vertexCount );
                                break;

                            case VertexFormatAttributes.TexCoord1:
                                TexCoords1 = reader.ReadVector2s( vertexCount );
                                break;

                            case VertexFormatAttributes.TexCoord2:
                                TexCoords2 = reader.ReadVector2s( vertexCount );
                                break;

                            case VertexFormatAttributes.TexCoord3:
                                TexCoords3 = reader.ReadVector2s( vertexCount );
                                break;

                            case VertexFormatAttributes.Color0:
                                Colors0 = reader.ReadColors( vertexCount );
                                break;

                            case VertexFormatAttributes.Color1:
                                Colors1 = reader.ReadColors( vertexCount );
                                break;

                            case VertexFormatAttributes.BoneWeight:
                                boneWeights = reader.ReadVector4s( vertexCount );
                                break;

                            case VertexFormatAttributes.BoneIndex:
                                boneIndices = reader.ReadVector4s( vertexCount );
                                break;

                            default:
                                Console.WriteLine( "Unhandled vertex format element: {0}", attribute );
                                break;
                        }
                    } );
                }

                if ( boneWeights == null || boneIndices == null )
                    return;

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

            void ReadVertexAttributesModern()
            {
                if ( attributeFlags == 2 || attributeFlags == 4 )
                {
                    Positions = new Vector3[ vertexCount ];
                    Normals = new Vector3[ vertexCount ];
                    Tangents = new Vector4[ vertexCount ];
                    TexCoords0 = new Vector2[ vertexCount ];
                    TexCoords1 = new Vector2[ vertexCount ];
                    Colors0 = new Color[ vertexCount ];

                    if ( attributeFlags == 4 )
                        BoneWeights = new BoneWeight[ vertexCount ];

                    bool hasTangents = false;
                    bool hasTexCoord1 = false;
                    bool hasColors = false;

                    var vertexReader = section.VertexData.Reader;

                    for ( int i = 0; i < vertexCount; i++ )
                    {
                        vertexReader.SeekBegin( section.VertexData.DataOffset + attributeOffsets[ 13 ] + vertexSize * i );

                        Positions[ i ] = vertexReader.ReadVector3();
                        Normals[ i ] = vertexReader.ReadVector3( VectorBinaryFormat.Int16 );
                        vertexReader.SeekCurrent( 2 );
                        Tangents[ i ] = vertexReader.ReadVector4( VectorBinaryFormat.Int16 );
                        TexCoords0[ i ] = vertexReader.ReadVector2( VectorBinaryFormat.Half );
                        TexCoords1[ i ] = vertexReader.ReadVector2( VectorBinaryFormat.Half );
                        Colors0[ i ] = vertexReader.ReadColor( VectorBinaryFormat.Half );

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
                        if ( TexCoords0[ i ] != TexCoords1[ i ] ) hasTexCoord1 = true;
                        if ( !Colors0[ i ].Equals( Color.White ) ) hasColors = true;
                    }

                    if ( !hasTangents ) Tangents = null;
                    if ( !hasTexCoord1 ) TexCoords1 = null;
                    if ( !hasColors ) Colors0 = null;
                }

                if ( Tangents == null ) 
                    return;

                for ( int i = 0; i < Tangents.Length; i++ )
                {
                    int direction = Math.Sign( Tangents[ i ].W );
                    var tangent = Vector3.Normalize( new Vector3( Tangents[ i ].X, Tangents[ i ].Y, Tangents[ i ].Z ) );

                    Tangents[ i ] = new Vector4( tangent, direction );
                }
            }
        }

        internal void Write( EndianBinaryWriter writer, ObjectSection section = null )
        {
            writer.Write( 0 );
            writer.Write( BoundingSphere );
            writer.Write( SubMeshes.Count );

            writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
            {
                foreach ( var subMesh in SubMeshes )
                    subMesh.Write( writer, section );
            } );

            int vertexSize = 0;
            VertexFormatAttributes vertexFormat = default;

            if ( section != null )
            {
                vertexFormat = VertexFormatAttributes.UsesModernStorage;
                vertexSize = BoneWeights != null ? 56 : 44;
            }

            else
            {
                if ( Positions != null )
                {
                    vertexFormat |= VertexFormatAttributes.Position;
                    vertexSize += 12;
                }

                if ( Normals != null )
                {
                    vertexFormat |= VertexFormatAttributes.Normal;
                    vertexSize += 12;
                }

                if ( Tangents != null )
                {
                    vertexFormat |= VertexFormatAttributes.Tangent;
                    vertexSize += 16;
                }

                if ( TexCoords0 != null )
                {
                    vertexFormat |= VertexFormatAttributes.TexCoord0;
                    vertexSize += 8;
                }

                if ( TexCoords1 != null )
                {
                    vertexFormat |= VertexFormatAttributes.TexCoord1;
                    vertexSize += 8;
                }                
                
                if ( TexCoords2 != null )
                {
                    vertexFormat |= VertexFormatAttributes.TexCoord2;
                    vertexSize += 8;
                }                
                
                if ( TexCoords3 != null )
                {
                    vertexFormat |= VertexFormatAttributes.TexCoord3;
                    vertexSize += 8;
                }

                if ( Colors0 != null )
                {
                    vertexFormat |= VertexFormatAttributes.Color0;
                    vertexSize += 16;
                }               
                
                if ( Colors1 != null )
                {
                    vertexFormat |= VertexFormatAttributes.Color1;
                    vertexSize += 16;
                }

                if ( BoneWeights != null )
                {
                    vertexFormat |= VertexFormatAttributes.BoneWeight | VertexFormatAttributes.BoneIndex;
                    vertexSize += 32;
                }
            }

            writer.Write( ( int ) vertexFormat );
            writer.Write( vertexSize );
            writer.Write( Positions.Length );

            if ( section != null )
                WriteVertexAttributesModern();

            else
                WriteVertexAttributesClassic();

            writer.Write( ( int ) Flags );

            writer.Write( section != null ? ( BoneWeights != null ? 4 : 2 ) : 0 );

            writer.WriteNulls( 6 * sizeof( uint ) ); // Reserved

            writer.Write( Name, StringBinaryFormat.FixedLength, 64 );

            void WriteVertexAttributesClassic()
            {
                for ( int i = 0; i < 20; i++ )
                {
                    var attribute = ( VertexFormatAttributes ) ( 1 << i );

                    writer.ScheduleWriteOffsetIf( ( vertexFormat & attribute ) != 0, 4, AlignmentMode.Left, () =>
                    {
                        switch ( attribute )
                        {
                            case VertexFormatAttributes.Position:
                                writer.Write( Positions );
                                break;

                            case VertexFormatAttributes.Normal:
                                writer.Write( Normals );
                                break;

                            case VertexFormatAttributes.Tangent:
                                writer.Write( Tangents );
                                break;

                            case VertexFormatAttributes.TexCoord0:
                                writer.Write( TexCoords0 );
                                break;

                            case VertexFormatAttributes.TexCoord1:
                                writer.Write( TexCoords1 );
                                break;                            
                            
                            case VertexFormatAttributes.TexCoord2:
                                writer.Write( TexCoords2 );
                                break;

                            case VertexFormatAttributes.TexCoord3:
                                writer.Write( TexCoords3 );
                                break;

                            case VertexFormatAttributes.Color0:
                                writer.Write( Colors0 );
                                break;                            
                            
                            case VertexFormatAttributes.Color1:
                                writer.Write( Colors1 );
                                break;

                            case VertexFormatAttributes.BoneWeight:
                                foreach ( var weight in BoneWeights )
                                {
                                    writer.Write( weight.Weight1 );
                                    writer.Write( weight.Weight2 );
                                    writer.Write( weight.Weight3 );
                                    writer.Write( weight.Weight4 );
                                }

                                break;

                            case VertexFormatAttributes.BoneIndex:
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
                int byteSize = writer.AddressSpace.GetByteSize();

                writer.Align( byteSize );
                writer.WriteNulls( 13 * byteSize );
                writer.WriteOffset( section.VertexData.AddSubMesh( this, vertexSize ) );
                writer.WriteNulls( 6 * byteSize );
            }
        }

        public Mesh()
        {
            SubMeshes = new List<SubMesh>();
        }

        [Flags]
        private enum VertexFormatAttributes
        {
            Position = 1 << 0,
            Normal = 1 << 1,
            Tangent = 1 << 2,
            TexCoord0 = 1 << 4,
            TexCoord1 = 1 << 5,
            TexCoord2 = 1 << 6,
            TexCoord3 = 1 << 7,
            Color0 = 1 << 8,
            Color1 = 1 << 9,
            BoneWeight = 1 << 10,
            BoneIndex = 1 << 11,
            VertexData = 1 << 13,
            UsesModernStorage = 1 << 31
        }
    }
}