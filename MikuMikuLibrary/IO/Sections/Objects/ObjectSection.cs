﻿using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.IO.Sections.Objects
{
    [Section( "OMDL" )]
    public class ObjectSection : Section<Object>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        [SubSection( typeof( SkinSection ) )]
        public Skin Skin
        {
            get => Data.Skin;
            set => Data.Skin = value;
        }

        [SubSection] public MeshIndexDataSection IndexData { get; set; }

        [SubSection] public MeshVertexDataSection VertexData { get; set; }

        protected override void Read( Object data, EndianBinaryReader reader, long length ) =>
            data.Read( reader, this );

        protected override void Write( Object data, EndianBinaryWriter writer ) =>
            data.Write( writer, this );

        public ObjectSection( SectionMode mode, Object data = null ) : base( mode, data )
        {
            IndexData = new MeshIndexDataSection( mode, this );
            VertexData = new MeshVertexDataSection( mode, this );
        }
    }
}