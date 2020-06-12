using System.Collections.Generic;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "OIDX" )]
    public class IndexDataSection : Section<object>
    {
        private readonly List<ushort[]> mIndices;
        private long mCurrentOffset;

        public override SectionFlags Flags => SectionFlags.HasNoRelocationTable;

        public long AddIndices( ushort[] indices )
        {
            long current = mCurrentOffset;
            {
                mIndices.Add( indices );
                mCurrentOffset += indices.Length * 2;
                mCurrentOffset = AlignmentHelper.Align( mCurrentOffset, 4 );
            }

            return current;
        }

        protected override void Read( object data, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object data, EndianBinaryWriter writer )
        {
            foreach ( var indices in mIndices )
            {
                writer.Write( indices );
                writer.WriteAlignmentPadding( 4 );
            }
        }

        public IndexDataSection( SectionMode mode, object data = null ) : base( mode, data )
        {
            if ( mode == SectionMode.Write )
                mIndices = new List<ushort[]>();
        }
    }
}