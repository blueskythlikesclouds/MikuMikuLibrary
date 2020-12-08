using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters
{
    public class OsageBocParameter
    {
        public uint StNode { get; set; }
        public uint EdNode { get; set; }
        public string EdRoot { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            StNode = reader.ReadUInt32();
            EdNode = reader.ReadUInt32();
            EdRoot = reader.ReadString( StringBinaryFormat.FixedLength, 64 );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( StNode );
            writer.Write( EdNode );
            writer.Write( EdRoot, StringBinaryFormat.FixedLength, 64 );
        }

        internal void Read( ParameterTree tree )
        {
            StNode = tree.Get<uint>( "st_node" );
            EdNode = tree.Get<uint>( "ed_node" );
            EdRoot = tree.Get<string>( "ed_root" );
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.Write( "st_node", StNode );
            writer.Write( "ed_node", EdNode );
            writer.Write( "ed_root", EdRoot );
        }
    }
}