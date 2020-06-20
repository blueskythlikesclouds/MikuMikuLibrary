using System;
using System.Collections.Generic;
using System.Diagnostics;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class OsageBlock : NodeBlock
    {
        public override string Signature => "OSG";

        internal int StartIndex { get; set; }
        internal int Count { get; set; }

        public List<OsageBone> Bones { get; }
        public string ExternalName { get; set; }
        public string InternalName { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            StartIndex = reader.ReadInt32();
            Count = reader.ReadInt32();
            ExternalName = stringSet.ReadString( reader );
            InternalName = stringSet.ReadString( reader );

            reader.SeekCurrent( reader.AddressSpace.GetByteSize() ); // Integrated skin parameter
            reader.SkipNulls( 3 * sizeof( uint ) );

            Bones.Capacity = Count;
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.Write( StartIndex );
            writer.Write( Bones.Count );
            stringSet.WriteString( writer, ExternalName );
            stringSet.WriteString( writer, InternalName );
            writer.WriteNulls( writer.AddressSpace.GetByteSize() );
            writer.WriteNulls( 3 * sizeof( uint ) );
        }

        public OsageBlock()
        {
            Bones = new List<OsageBone>();
        }
    }
}