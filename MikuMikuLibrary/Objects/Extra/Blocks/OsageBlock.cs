using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Objects.Extra.Blocks
{
    public class OsageBlock : NodeBlock
    {
        public override string Signature => "OSG";

        internal int StartIndex { get; set; }
        internal int Count { get; set; }

        public List<OsageNode> Nodes { get; }
        public string ExternalName { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            StartIndex = reader.ReadInt32();
            Count = reader.ReadInt32();
            ExternalName = stringSet.ReadString( reader );
            Name = stringSet.ReadString( reader );

            Nodes.Capacity = Count;

            for ( int i = 0; i < Count; i++ )
                Nodes.Add( new OsageNode() );

            // Either means rotation info on FT, or integrated SKP on old DT/AC
            reader.ReadOffset( () =>
            {
                long current = reader.Position;
                {
                    if ( reader.ReadUInt32() == 0 ) // Integrated SKP, yet to support.
                        return;
                }

                reader.SeekBegin( current );

                foreach ( var bone in Nodes )
                    bone.ReadOsgBlockInfo( reader, stringSet );
            } );

            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SkipNulls( 4 * sizeof( ulong ) );
            else
                reader.SkipNulls( 5 * sizeof( uint ) );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet, BinaryFormat format )
        {
            writer.Write( StartIndex );
            writer.Write( Nodes.Count );
            stringSet.WriteString( writer, ExternalName );
            stringSet.WriteString( writer, Name );

            bool shouldWriteRotation = format == BinaryFormat.FT && Nodes.Any( x =>
                Math.Abs( x.Rotation.X ) > 0 ||
                Math.Abs( x.Rotation.Y ) > 0 ||
                Math.Abs( x.Rotation.Z ) > 0 );

            writer.ScheduleWriteOffsetIf( shouldWriteRotation, 4, AlignmentMode.Left, () =>
            {
                foreach ( var bone in Nodes )
                    bone.WriteOsgBlockInfo( writer, stringSet );
            } );

            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 4 * sizeof( ulong ) );
            else
                writer.WriteNulls( 5 * sizeof( uint ) );
        }

        public OsageBlock()
        {
            Nodes = new List<OsageNode>();
        }
    }
}