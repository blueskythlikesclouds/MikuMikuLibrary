using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            StartIndex = reader.ReadInt32();
            Count = reader.ReadInt32();
            ExternalName = stringSet.ReadString( reader );
            Name = stringSet.ReadString( reader );

            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SkipNulls( 5 * sizeof( ulong ) );
            else
                reader.SkipNulls( 6 * sizeof( uint ) );

            Bones.Capacity = Count;
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.Write( StartIndex );
            writer.Write( Bones.Count );
            stringSet.WriteString( writer, ExternalName );
            stringSet.WriteString( writer, Name );

            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 5 * sizeof( ulong ) );
            else
                writer.WriteNulls( 6 * sizeof( uint ) );
        }

        public OsageBlock()
        {
            Bones = new List<OsageBone>();
        }

        // Obsolete properties
        
        [Obsolete( "This property has been renamed. Please use Name instead." ), Browsable( false )]
        public string InternalName
        {
            get => Name;
            set => Name = value;
        }
    }
}