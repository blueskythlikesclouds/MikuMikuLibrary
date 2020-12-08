using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters
{
    public class OsageSkinParameter
    {
        public string Name { get; set; }
        public float Force { get; set; }
        public float ForceGain { get; set; }
        public float AirResistance { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public float Friction { get; set; }
        public float WindAffection { get; set; }
        public uint CollisionType { get; set; }
        public float InitRotationY { get; set; }
        public float InitRotationZ { get; set; }
        public float HingeY { get; set; }
        public float HingeZ { get; set; }
        public float CollisionRadius { get; set; }
        public float Stiffness { get; set; }
        public float MoveCancel { get; set; }
        public List<OsageCollisionParameter> Collisions { get; }
        public List<OsageBocParameter> Bocs { get; }
        public List<OsageNodeParameter> Nodes { get; }

        internal void Read( EndianBinaryReader reader )
        {
            Force = reader.ReadSingle();
            ForceGain = reader.ReadSingle();
            AirResistance = reader.ReadSingle();

            if ( reader.AddressSpace == AddressSpace.Int64 )
                reader.SkipNulls( 4 );

            RotationY = reader.ReadSingle();
            RotationZ = reader.ReadSingle();
            InitRotationY = reader.ReadSingle();
            InitRotationZ = reader.ReadSingle();
            HingeY = reader.ReadSingle();
            HingeZ = reader.ReadSingle();
            Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

            for ( int i = 0; i < 12; i++ )
            {
                var collision = new OsageCollisionParameter();
                collision.Read( reader );

                if ( !string.IsNullOrEmpty( collision.Bone0.Name ) && !string.IsNullOrEmpty( collision.Bone1.Name ) )
                    Collisions.Add( collision );
            }

            reader.SkipNulls( 4 ); // padding?

            Friction = reader.ReadSingle();
            WindAffection = reader.ReadSingle();
            
            reader.SkipNulls( 4 ); // padding?

            int bocCount, nodeCount;

            // X
            if ( reader.AddressSpace == AddressSpace.Int64 )
            {
                bocCount = reader.ReadInt32();
                CollisionType = reader.ReadUInt32();
                reader.ReadOffset( ReadBocs );

                nodeCount = reader.ReadInt32();

                reader.SkipNulls( 8 );

                // Murmur hash of name
                reader.SeekCurrent( sizeof( ulong ) );

                reader.ReadOffset( ReadNodes );
            }

            // F 2nd
            else
            {
                bocCount = reader.ReadInt32();
                reader.ReadOffset( ReadBocs );
                CollisionType = reader.ReadUInt32();

                reader.SkipNulls( 4 );

                // Murmur hash of name
                reader.SeekCurrent( sizeof( ulong ) );

                nodeCount = reader.ReadInt32();
                reader.ReadOffset( ReadNodes );
            }

            void ReadBocs()
            {
                Bocs.Capacity = bocCount;

                for ( int i = 0; i < bocCount; i++ )
                {
                    var boc = new OsageBocParameter();
                    boc.Read( reader );
                    Bocs.Add( boc );
                }
            }

            void ReadNodes()
            {
                Nodes.Capacity = nodeCount;

                for ( int i = 0; i < nodeCount; i++ )
                {
                    var node = new OsageNodeParameter();
                    node.Read( reader );
                    Nodes.Add( node );
                }
            }
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( Force );
            writer.Write( ForceGain );
            writer.Write( AirResistance );

            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.WriteNulls( 4 );

            writer.Write( RotationY );
            writer.Write( RotationZ );
            writer.Write( InitRotationY );
            writer.Write( InitRotationZ );
            writer.Write( HingeY );
            writer.Write( HingeZ );
            writer.AddStringToStringTable( Name );

            int collisionCount = Math.Min( 12, Collisions.Count );
            for ( int i = 0; i < collisionCount; i++ )
                Collisions[ i ].Write( writer );

            for ( int i = 0; i < 12 - collisionCount; i++ )
                OsageCollisionParameter.WriteNull( writer );

            writer.WriteNulls( 4 );

            writer.Write( Friction );
            writer.Write( WindAffection );

            writer.WriteNulls( 4 );

            // X
            if ( writer.AddressSpace == AddressSpace.Int64 )
            {
                writer.Write( Bocs.Count );
                writer.Write( CollisionType );
                writer.ScheduleWriteOffsetIf( Bocs.Count > 0, 16, AlignmentMode.Left, WriteBocs );
                writer.Write( Nodes.Count );
                writer.WriteNulls( 8 );
                writer.Write( ( ulong ) MurmurHash.Calculate( Name ) );
                writer.ScheduleWriteOffsetIf( Nodes.Count > 0, 16, AlignmentMode.Left, WriteNodes );
            }

            // F 2nd
            else
            {
                writer.Write( Bocs.Count );
                writer.ScheduleWriteOffsetIf( Bocs.Count > 0, 16, AlignmentMode.Left, WriteBocs );
                writer.Write( CollisionType );
                writer.WriteNulls( 4 );
                writer.Write( ( ulong ) MurmurHash.Calculate( Name ) );
                writer.Write( Nodes.Count );
                writer.ScheduleWriteOffsetIf( Nodes.Count > 0, 16, AlignmentMode.Left, WriteNodes );
            }

            void WriteBocs()
            {
                foreach ( var boc in Bocs )
                    boc.Write( writer );
            }

            void WriteNodes()
            {
                foreach ( var node in Nodes )
                    node.Write( writer );
            }
        }

        internal void Read( ParameterTree tree )
        {
            if ( tree.OpenScope( "root" ) )
            {
                Force = tree.Get<float>( "force" );
                ForceGain = tree.Get<float>( "force_gain" );
                AirResistance = tree.Get<float>( "air_res" );
                RotationY = tree.Get<float>( "rot_y" );
                RotationZ = tree.Get<float>( "rot_z" );
                Friction = tree.Get<float>( "friction" );
                WindAffection = tree.Get<float>( "wind_afc" );
                InitRotationY = tree.Get<float>( "init_rot_y" );
                InitRotationZ = tree.Get<float>( "init_rot_z" );             
                HingeY = tree.Get<float>( "hinge_y", 90 );
                HingeZ = tree.Get<float>( "hinge_z", 90 );
                CollisionRadius = tree.Get<float>( "coli_r" );
                Stiffness = tree.Get<float>( "stiffness" );
                MoveCancel = tree.Get<float>( "move_cancel" );

                tree.Enumerate( "coli", i =>
                {
                    var collision = new OsageCollisionParameter();
                    collision.Read( tree );
                    Collisions.Add( collision );
                } );

                tree.Enumerate( "boc", i =>
                {
                    var boc = new OsageBocParameter();
                    boc.Read( tree );
                    Bocs.Add( boc );
                } );

                tree.CloseScope();
            }

            tree.Enumerate( "node", i =>
            {
                var node = new OsageNodeParameter();
                node.Read( tree );
                Nodes.Add( node );
            } );
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.PushScope( Name );
            {
                writer.PushScope( "root" );
                {
                    writer.Write( "force", Force );
                    writer.Write( "force_gain", ForceGain );
                    writer.Write( "air_res", AirResistance );
                    writer.Write( "rot_y", RotationY );
                    writer.Write( "rot_z", RotationZ );
                    writer.Write( "friction", Friction );
                    writer.Write( "wind_afc", WindAffection );
                    writer.Write( "coli_type", CollisionType );
                    writer.Write( "init_rot_y", InitRotationY );
                    writer.Write( "init_rot_z", InitRotationZ );
                    writer.Write( "hinge_y", HingeY );
                    writer.Write( "hinge_z", HingeZ );
                    writer.Write( "coli_r", CollisionRadius );
                    writer.Write( "stiffness", Stiffness );
                    writer.Write( "move_cancel", MoveCancel );

                    writer.Write( "coli", Collisions, x => x.Write( writer ) );
                    writer.Write( "boc", Bocs, x => x.Write( writer ) );
                }
                writer.PopScope();

                writer.Write( "node", Nodes, x => x.Write( writer ) );
            }
            writer.PopScope();
        }

        public OsageSkinParameter()
        {
            Collisions = new List<OsageCollisionParameter>( 12 );
            Bocs = new List<OsageBocParameter>();
            Nodes = new List<OsageNodeParameter>();
        }
    }
}