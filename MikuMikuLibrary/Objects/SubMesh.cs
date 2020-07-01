using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections.Objects;

namespace MikuMikuLibrary.Objects
{
    public enum PrimitiveType
    {
        Points = 0,
        Lines = 1,
        LineStrip = 2,
        LineLoop = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadStrip = 8,
        Polygon = 9
    }

    public enum IndexFormat
    {
        UInt8 = 0,
        UInt16 = 1,
        UInt32 = 2
    }

    public struct Triangle
    {
        public uint A, B, C;

        public Triangle( uint a, uint b, uint c )
        {
            A = a;
            B = b;
            C = c;
        }
    }

    [Flags]
    public enum SubMeshFlags
    {
        ReceiveShadow = 1 << 0,
        CastShadow = 1 << 1,
        Transparent = 1 << 2
    }

    public class SubMesh
    {
        public BoundingSphere BoundingSphere { get; set; }
        public uint MaterialIndex { get; set; }
        public byte[] TexCoordIndices { get; set; }
        public ushort[] BoneIndices { get; set; }
        public uint BonesPerVertex { get; set; }
        public PrimitiveType PrimitiveType { get; set; }
        public IndexFormat IndexFormat { get; set; }
        public uint[] Indices { get; set; }
        public SubMeshFlags Flags { get; set; }
        public uint IndexOffset { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public static int GetByteSize( BinaryFormat format )
        {
            switch ( format )
            {
                case BinaryFormat.DT:
                case BinaryFormat.F:
                case BinaryFormat.FT:
                    return 0x5C;

                case BinaryFormat.F2nd:
                    return 0x70;

                case BinaryFormat.X:
                    return 0x80;
            }

            throw new ArgumentException( nameof( format ) );
        }

        internal void Read( EndianBinaryReader reader, ObjectSection section = null )
        {
            reader.SeekCurrent( 4 ); // Unused flags

            BoundingSphere = reader.ReadBoundingSphere();

            MaterialIndex = reader.ReadUInt32();
            TexCoordIndices = reader.ReadBytes( 8 );

            int boneIndexCount = reader.ReadInt32();
            long boneIndicesOffset = reader.ReadOffset();
            BonesPerVertex = reader.ReadUInt32();

            PrimitiveType = ( PrimitiveType ) reader.ReadUInt32();
            IndexFormat = ( IndexFormat ) reader.ReadInt32();
            int indexCount = reader.ReadInt32();
            long indicesOffset = reader.ReadOffset();
            Flags = ( SubMeshFlags ) reader.ReadInt32();

            if ( section != null )
            {
                reader.SkipNulls( 4 * sizeof( uint ) );
                BoundingBox = reader.ReadBoundingBox();
                reader.SeekCurrent( sizeof( uint ) ); // Max Index
            }

            else
            {
                BoundingBox = BoundingSphere.ToBoundingBox();
                reader.SkipNulls( 6 * sizeof( uint ) );
            }

            IndexOffset = reader.ReadUInt32();

            reader.ReadAtOffsetIf( BonesPerVertex == 4, boneIndicesOffset, 
                () => { BoneIndices = reader.ReadUInt16s( boneIndexCount ); } );

            if ( section == null )
            {
                reader.ReadAtOffset( indicesOffset, () => { ReadIndices( reader ); } );
            }

            else
            {
                var indexReader = section.IndexData.Reader;

                indexReader.SeekBegin( section.IndexData.DataOffset + indicesOffset );
                ReadIndices( indexReader );
            }

            void ReadIndices( EndianBinaryReader r )
            {
                Indices = new uint[ indexCount ];

                switch ( IndexFormat )
                {
                    case IndexFormat.UInt8:
                        for ( int i = 0; i < Indices.Length; i++ )
                        {
                            byte index = r.ReadByte();
                            Indices[ i ] = index == 0xFF ? 0xFFFFFFFF : index;
                        }

                        break;

                    case IndexFormat.UInt16:
                        for ( int i = 0; i < Indices.Length; i++ )
                        {
                            ushort index = r.ReadUInt16();
                            Indices[ i ] = index == 0xFFFF ? 0xFFFFFFFF : index;
                        }

                        break;

                    case IndexFormat.UInt32:
                        for ( int i = 0; i < Indices.Length; i++ )
                            Indices[ i ] = r.ReadUInt32();

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal void Write( EndianBinaryWriter writer, ObjectSection section = null )
        {
            writer.Write( 0 );
            writer.Write( BoundingSphere );
            writer.Write( MaterialIndex );

            if ( TexCoordIndices?.Length == 8 )
                writer.Write( TexCoordIndices );

            else
                writer.WriteNulls( 8 );

            writer.Write( BoneIndices?.Length ?? 0 );
            writer.ScheduleWriteOffsetIf( BoneIndices != null, 4, AlignmentMode.Left,
                () => { writer.Write( BoneIndices ); } );
            writer.Write( BonesPerVertex );

            writer.Write( ( int ) PrimitiveType );
            writer.Write( ( int ) IndexFormat );
            writer.Write( Indices.Length );

            // Modern Format
            if ( section != null )
            {
                writer.WriteOffset( section.IndexData.AddSubMesh( this ) );
                writer.Write( ( int ) Flags );

                writer.WriteNulls( 4 * sizeof( uint ) );

                writer.Write( BoundingBox );

                switch ( IndexFormat )
                {
                    case IndexFormat.UInt8:
                        writer.WriteNulls( 3 );
                        writer.Write( ( byte ) Indices.Max() );
                        break;

                    case IndexFormat.UInt16:
                        writer.WriteNulls( 2 );
                        writer.Write( ( ushort ) Indices.Max() );
                        break;

                    case IndexFormat.UInt32:
                        writer.Write( Indices.Max() );
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            else
            {
                writer.ScheduleWriteOffset( 8, AlignmentMode.Left, () =>
                {
                    switch ( IndexFormat )
                    {
                        case IndexFormat.UInt8:
                            foreach ( uint index in Indices )
                                writer.Write( ( byte ) index );

                            break;

                        case IndexFormat.UInt16:
                            foreach ( uint index in Indices )
                                writer.Write( ( ushort ) index );

                            break;

                        case IndexFormat.UInt32:
                            foreach ( uint index in Indices )
                                writer.Write( index );

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } );
                writer.Write( ( int ) Flags );
                writer.WriteNulls( 6 * sizeof( uint ) );
            }

            writer.Write( IndexOffset );

            if ( section?.Format == BinaryFormat.X )
                writer.WriteNulls( sizeof( uint ) );
        }

        public unsafe List<Triangle> GetTriangles()
        {
            var triangles = new List<Triangle>();
            if ( Indices == null || Indices.Length == 0 )
                return triangles;

            fixed ( uint* indicesPtr = Indices )
            {
                var start = indicesPtr;
                var end = start + Indices.Length;

                if ( PrimitiveType == PrimitiveType.Triangles )
                {
                    triangles.Capacity = Indices.Length / 3;

                    while ( start < end )
                        triangles.Add( new Triangle( *start++, *start++, *start++ ) );
                }

                else if ( PrimitiveType == PrimitiveType.TriangleStrip )
                {
                    uint a = *start++;
                    uint b = *start++;
                    int direction = -1;

                    while ( start < end )
                    {
                        uint c = *start++;

                        if ( c == 0xFFFFFFFF )
                        {
                            a = *start++;
                            b = *start++;
                            direction = -1;
                        }

                        else
                        {
                            direction *= -1;
                            if ( a != b && b != c && c != a )
                                triangles.Add( direction > 0 ? new Triangle( a, b, c ) : new Triangle( a, c, b ) );

                            a = b;
                            b = c;
                        }
                    }
                }
            }

            return triangles;
        }

        public SubMesh()
        {
            TexCoordIndices = new byte[ 8 ];
            PrimitiveType = PrimitiveType.Triangles;
        }
    }
}