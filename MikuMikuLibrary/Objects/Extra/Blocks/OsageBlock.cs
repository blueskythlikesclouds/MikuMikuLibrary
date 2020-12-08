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

        public List<OsageBone> Bones { get; }
        public string ExternalName { get; set; }

        internal override void ReadBody( EndianBinaryReader reader, StringSet stringSet )
        {
            StartIndex = reader.ReadInt32();
            Count = reader.ReadInt32();
            ExternalName = stringSet.ReadString( reader );
            Name = stringSet.ReadString( reader );

            Bones.Capacity = Count;

            for ( int i = 0; i < Count; i++ )
                Bones.Add( new OsageBone() );

            // Either means rotation info on FT, or integrated SKP on old DT/AC
            reader.ReadOffset( () =>
            {
                long current = reader.Position;
                {
                    if ( reader.ReadUInt32() == 0 ) // Integrated SKP, yet to support.
                        return;
                }

                reader.SeekBegin( current );

                foreach ( var bone in Bones )
                    bone.ReadOsgBlockInfo( reader, stringSet );
            } );

            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SkipNulls( 4 * sizeof( ulong ) );
            else
                reader.SkipNulls( 5 * sizeof( uint ) );
        }

        internal override void WriteBody( EndianBinaryWriter writer, StringSet stringSet )
        {
            writer.Write( StartIndex );
            writer.Write( Bones.Count );
            stringSet.WriteString( writer, ExternalName );
            stringSet.WriteString( writer, Name );

            bool hasAnyRotation = Bones.Any( x =>
                Math.Abs( x.Rotation.X ) > 0 ||
                Math.Abs( x.Rotation.Y ) > 0 ||
                Math.Abs( x.Rotation.Z ) > 0 );

            writer.ScheduleWriteOffsetIf( hasAnyRotation, 4, AlignmentMode.Left, () =>
            {
                foreach ( var bone in Bones )
                    bone.WriteOsgBlockInfo( writer, stringSet );
            } );

            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 4 * sizeof( ulong ) );
            else
                writer.WriteNulls( 5 * sizeof( uint ) );
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