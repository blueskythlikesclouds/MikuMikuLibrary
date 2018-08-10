using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
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
        public const int ByteSizeClassic = 0x5C;
        public const int ByteSizeModern = 0x70;

        public BoundingSphere BoundingSphere { get; set; }
        public ushort[] Indices { get; set; }
        public ushort[] BoneIndices { get; set; }
        public int MaterialIndex { get; set; }
        public IndexTableType Type { get; set; }

        // Modern Formats
        public float Field00 { get; set; }
        public float Field01 { get; set; }
        public float Field02 { get; set; }
        public float Field03 { get; set; }
        public float Field04 { get; set; }
        public float Field05 { get; set; }
        public int Field06 { get; set; }

        internal void Read( EndianBinaryReader reader, MeshSection section = null )
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

            if ( section != null )
            {
                reader.SeekCurrent( 0x14 );
                Field00 = reader.ReadSingle();
                Field01 = reader.ReadSingle();
                Field02 = reader.ReadSingle();
                Field03 = reader.ReadSingle();
                Field04 = reader.ReadSingle();
                Field05 = reader.ReadSingle();
                Field06 = reader.ReadInt32();
            }

            reader.ReadAtOffsetIf( field00 == 4, boneIndicesOffset, () =>
            {
                BoneIndices = reader.ReadUInt16s( boneIndexCount );
            } );

            if ( section == null )
            {
                reader.ReadAtOffset( indicesOffset, () =>
                {
                    Indices = reader.ReadUInt16s( indexCount );
                } );
            }
            else
            {
                section.IndexData.Reader.SeekBegin( indicesOffset );
                Indices = section.IndexData.Reader.ReadUInt16s( indexCount );
            }
        }

        internal void Write( EndianBinaryWriter writer, MeshSection section = null )
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

            // Modern Format
            if ( section != null )
            {
                section.IndexData.Writer.WriteAlignmentPadding( 4 );
                writer.Write( ( uint )section.IndexData.Data.Position );

                // Write the indices to the index data
                section.IndexData.Writer.Write( Indices );

                // Unknown data that I newly discovered
                writer.WriteNulls( 20 );
                writer.Write( Field00 );
                writer.Write( Field01 );
                writer.Write( Field02 );
                writer.Write( Field03 );
                writer.Write( Field04 );
                writer.Write( Field05 );
                writer.Write( Field06 );
                writer.Write( 0 );
            }

            else
            {
                writer.EnqueueOffsetWriteAligned( 4, AlignmentKind.Left, () =>
                {
                    writer.Write( Indices );
                } );

                writer.WriteNulls( 32 );
            }
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
