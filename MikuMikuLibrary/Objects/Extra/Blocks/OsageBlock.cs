using System.Collections.Generic;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class OsageBlock : Block
    {
        public override string Signature => "OSG";

        internal int StartIndex { get; set; }
        internal int Count { get; set; }

        public List<OsageBone> Bones { get; }
        public string ExternalName { get; set; }
        public string InternalName { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SeekCurrent( 4 );

            StartIndex = reader.ReadInt32();
            Count = reader.ReadInt32();
            ExternalName = stringSet.ReadString( reader );
            InternalName = stringSet.ReadString( reader );

            Bones.Capacity = Count;
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 4 );

            writer.Write( StartIndex );
            writer.Write( Bones.Count );
            stringSet.WriteString( writer, ExternalName );
            stringSet.WriteString( writer, InternalName );
            writer.WriteNulls( 40 );
        }

        public OsageBlock()
        {
            Bones = new List<OsageBone>();
        }
    }
}