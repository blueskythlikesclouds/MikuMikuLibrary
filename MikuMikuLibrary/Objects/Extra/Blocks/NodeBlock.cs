using System.Numerics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public abstract class NodeBlock : IBlock
    {
        public abstract string Signature { get; }

        public string ParentName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string Name { get; set; }

        public virtual void Read( EndianBinaryReader reader, StringSet stringSet )
        {
            ParentName = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            Position = reader.ReadVector3();
            Rotation = reader.ReadVector3();
            Scale = reader.ReadVector3();

            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SeekCurrent( 4 );

            ReadBody( reader, stringSet );
        }

        public virtual void Write( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.AddStringToStringTable( ParentName );
            writer.Write( Position );
            writer.Write( Rotation );
            writer.Write( Scale );

            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( sizeof( uint ) );

            WriteBody( writer, stringSet );
        }

        internal abstract void ReadBody( EndianBinaryReader reader, StringSet stringSet );
        internal abstract void WriteBody( EndianBinaryWriter writer, StringSet stringSet );
    }
}