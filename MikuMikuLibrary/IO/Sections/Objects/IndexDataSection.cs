using System;
using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "OIDX" )]
    public class IndexDataSection : Section<object>
    {
        private readonly List<SubMesh> mSubMeshes;
        private long mCurrentOffset;

        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;

        public long AddSubMesh( SubMesh subMesh )
        {
            long current = mCurrentOffset;
            {
                mSubMeshes.Add( subMesh );
                mCurrentOffset += subMesh.Indices.Length * ( 1 << ( int ) subMesh.IndexFormat );
                mCurrentOffset = AlignmentHelper.Align( mCurrentOffset, 4 );
            }

            return current;
        }

        protected override void Read( object data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object data, EndianBinaryWriter writer )
        {
            foreach ( var subMesh in mSubMeshes )
            {
                switch ( subMesh.IndexFormat )
                {
                    case IndexFormat.UInt8:
                        foreach ( uint index in subMesh.Indices )
                            writer.Write( ( byte ) index );

                        break;                   
                    
                    case IndexFormat.UInt16:
                        foreach ( uint index in subMesh.Indices )
                            writer.Write( ( ushort ) index );

                        break;                   
                    
                    case IndexFormat.UInt32:
                        foreach ( uint index in subMesh.Indices )
                            writer.Write( index );

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }


                writer.Align( 4 );
            }
        }

        public IndexDataSection( SectionMode mode, object data = null ) : base( mode, data )
        {
            if ( mode == SectionMode.Write )
                mSubMeshes = new List<SubMesh>();
        }
    }
}