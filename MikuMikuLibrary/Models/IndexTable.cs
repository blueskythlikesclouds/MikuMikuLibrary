using MikuMikuLibrary.IO;
using System.Collections.Generic;

namespace MikuMikuLibrary.Models
{
    public enum IndexTableType
    {
        Triangles = 4,
        TriangleStrip = 5,
    };

    public struct Triangle
    {
        public ushort A, B, C;

        public Triangle( ushort a, ushort b, ushort c )
        {
            A = a; B = b; C = c;
        }
    }

    public class IndexTable
    {
        public const int ByteSize = 0x5C;

        public BoundingSphere BoundingSphere { get; set; }
        public ushort[] Indices { get; set; }
        public ushort[] BoneIndices { get; set; }
        public int MaterialIndex { get; set; }
        public IndexTableType Type { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            BoundingSphere = BoundingSphere.FromReader( reader );
            MaterialIndex = reader.ReadInt32();
            reader.SeekCurrent( 8 );
            int boneIndexCount = reader.ReadInt32();
            uint boneIndicesOffset = reader.ReadUInt32();
            uint field00 = reader.ReadUInt32();
            Type = ( IndexTableType )reader.ReadUInt32();
            int field01 = reader.ReadInt32();
            int indexCount = reader.ReadInt32();
            uint indicesOffset = reader.ReadUInt32();

            reader.ReadAtOffsetIf( field00 == 4, boneIndicesOffset, () =>
            {
                BoneIndices = reader.ReadUInt16s( boneIndexCount );
            } );

            reader.ReadAtOffset( indicesOffset, () =>
            {
                Indices = reader.ReadUInt16s( indexCount );
            } );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            BoundingSphere.Write( writer );
            writer.Write( MaterialIndex );
            writer.WriteNulls( 8 );
            writer.Write( BoneIndices != null ? BoneIndices.Length : 0 );
            writer.EnqueueOffsetWriteAlignedIf( BoneIndices != null, 4, AlignmentKind.Left, () =>
            {
                writer.Write( BoneIndices );
            } );
            writer.Write( BoneIndices != null ? 4 : 0 );
            writer.Write( ( int )Type );
            writer.Write( 1 );
            writer.Write( Indices.Length );
            writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
            {
                writer.Write( Indices );
            } );
            writer.WriteNulls( 32 );
        }

        public unsafe List<Triangle> GetTriangles()
        {
            var triangles = new List<Triangle>();
            if ( Indices == null || Indices.Length == 0 )
                return triangles;

            fixed ( ushort* indicesPtr = Indices )
            {
                ushort* start = indicesPtr;
                ushort* end = start + Indices.Length;

                if ( Type == IndexTableType.Triangles )
                {
                    while ( start < end )
                        triangles.Add( new Triangle( *start++, *start++, *start++ ) );
                }
                else if ( Type == IndexTableType.TriangleStrip )
                {
                    ushort a = *start++; ushort b = *start++; ushort c = 0;
                    int direction = -1;

                    while ( start < end )
                    {
                        c = *start++;

                        if ( c == 0xFFFF )
                        {
                            a = *start++;
                            b = *start++;
                            direction = -1;
                        }

                        else
                        {
                            direction *= -1;
                            if ( a != b && b != c && c != a )
                            {
                                if ( direction > 0 )
                                    triangles.Add( new Triangle( a, b, c ) );
                                else
                                    triangles.Add( new Triangle( a, c, b ) );
                            }

                            a = b;
                            b = c;
                        }
                    }
                }
            }

            return triangles;
        }

        public IndexTable()
        {
            Type = IndexTableType.Triangles;
        }
    }
}
