using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MikuMikuLibrary.IO.Sections
{
    [Section( "MOSD", typeof( Model ) )]
    public class ModelSection : BinaryFileSection<Model>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        [SubSection( typeof( MeshSection ) )]
        public List<Mesh> Meshes => Data.Meshes;

        public ModelSection( Stream source, Model dataToRead = null ) : base( source, dataToRead )
        {
        }

        public ModelSection( Model dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "OMDL", typeof( Mesh ) )]
    public class MeshSection : Section<Mesh>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        [SubSection( typeof( MeshSkinSection ) )]
        public MeshSkin Skin
        {
            get => Data.Skin;
            set => Data.Skin = value;
        }

        [SubSection]
        public MeshIndexDataSection IndexData { get; set; }

        [SubSection]
        public MeshVertexDataSection VertexData { get; set; }

        protected override void Read( EndianBinaryReader reader, long length )
        {
            Data.Read( reader, this );
        }

        protected override void Write( EndianBinaryWriter writer )
        {
            Data.Write( writer, this );
        }

        public MeshSection( Stream source, Mesh dataToRead = null ) : base( source, dataToRead )
        {
        }

        public MeshSection( Mesh dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
            IndexData = new MeshIndexDataSection( new MemoryStream(), endianness, addressSpace );
            VertexData = new MeshVertexDataSection( new MemoryStream(), endianness, addressSpace );
        }
    }

    [Section( "OSKN", typeof( MeshSkin ) )]
    public class MeshSkinSection : Section<MeshSkin>
    {
        public override SectionFlags Flags => SectionFlags.RelocationTableSection;

        protected override void Read( EndianBinaryReader reader, long length )
        {
            Data.Read( reader );
        }

        protected override void Write( EndianBinaryWriter writer )
        {
            Data.Write( writer );
        }

        public MeshSkinSection( Stream source, MeshSkin dataToRead = null ) : base( source, dataToRead )
        {
        }

        public MeshSkinSection( MeshSkin dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    // TODO: Can I do this in a better way perhaps?

    public abstract class MemoryStreamSection : Section<MemoryStream>
    {
        /// <summary>
        /// Used when section is open for reading.
        /// Will be null otherwise
        /// </summary>
        public EndianBinaryReader Reader { get; }

        /// <summary>
        /// Used when section is open for writing.
        /// Will be null otherwise
        /// </summary>
        public EndianBinaryWriter Writer { get; }

        protected override void Read( EndianBinaryReader reader, long length )
        {
            // Copy the data to the memory stream
            var bytes = reader.ReadBytes( ( int )length );
            Data.Write( bytes, 0, bytes.Length );
            Data.Seek( 0, SeekOrigin.Begin );
        }

        protected override void Write( EndianBinaryWriter writer )
        {
            // Copy the contents to the section
            Data.Seek( 0, SeekOrigin.Begin );
            Data.CopyTo( writer.BaseStream );
        }

        public MemoryStreamSection( Stream source, MemoryStream dataToRead = null ) : base( source, dataToRead )
        {
            Reader = new EndianBinaryReader( Data, Encoding.UTF8, true, Endianness );
        }

        public MemoryStreamSection( MemoryStream dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
            Writer = new EndianBinaryWriter( Data, Encoding.UTF8, true, endianness );
        }
    }

    [Section( "OVTX", typeof( MemoryStream ) )]
    public class MeshVertexDataSection : MemoryStreamSection
    {
        public override SectionFlags Flags => SectionFlags.None;

        public MeshVertexDataSection( Stream source, MemoryStream dataToRead = null ) : base( source, dataToRead )
        {
        }

        public MeshVertexDataSection( MemoryStream dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }

    [Section( "OIDX", typeof( MemoryStream ) )]
    public class MeshIndexDataSection : MemoryStreamSection
    {
        public override SectionFlags Flags => SectionFlags.None;

        public MeshIndexDataSection( Stream source, MemoryStream dataToRead = null ) : base( source, dataToRead )
        {
        }

        public MeshIndexDataSection( MemoryStream dataToWrite, Endianness endianness, AddressSpace addressSpace ) : base( dataToWrite, endianness, addressSpace )
        {
        }
    }
}
