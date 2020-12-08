using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra
{
    public class OsageNodeParameter
    {
        public float HingeYMin { get; set; }
        public float HingeYMax { get; set; }
        public float HingeZMin { get; set; }
        public float HingeZMax { get; set; }
        public float Radius { get; set; }
        public float Weight { get; set; }
        public float InertialCancel { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            HingeYMin = reader.ReadSingle();
            HingeYMax = reader.ReadSingle();
            HingeZMin = reader.ReadSingle();
            HingeZMax = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Weight = reader.ReadSingle();
            InertialCancel = reader.ReadSingle();
            reader.SkipNulls( 4 ); // Padding?
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( HingeYMin );
            writer.Write( HingeYMax );
            writer.Write( HingeZMin );
            writer.Write( HingeZMax );
            writer.Write( Radius );
            writer.Write( Weight );
            writer.Write( InertialCancel );
            writer.WriteNulls( 4 );
        }

        internal void Read( ParameterTree tree )
        {
            HingeYMin = tree.Get<float>( "hinge_ymin", -180 );
            HingeYMax = tree.Get<float>( "hinge_ymax", 180 );         
            HingeZMin = tree.Get<float>( "hinge_zmin", -180 );
            HingeZMax = tree.Get<float>( "hinge_zmax", 180 );
            Radius = tree.Get<float>( "coli_r" );
            Weight = tree.Get<float>( "weight", 1 );
            InertialCancel = tree.Get<float>( "inertial_cancel" );
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.Write( "hinge_ymin", HingeYMin );
            writer.Write( "hinge_ymax", HingeYMax );          
            writer.Write( "hinge_zmin", HingeZMin );
            writer.Write( "hinge_zmax", HingeZMax );
            writer.Write( "coli_r", Radius );
            writer.Write( "inertial_cancel", InertialCancel );
            writer.Write( "weight", Weight );
        }
    }

    public class OsageCollisionBoneParameter
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }

        internal void Read( ParameterTree tree )
        {
            Name = tree.Get<string>( "name" );
            Position = new Vector3(
                tree.Get<float>( "posx" ),
                tree.Get<float>( "posy" ),
                tree.Get<float>( "posz" )
            );
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.Write( "name", Name );
            writer.Write( "posx", Position.X );
            writer.Write( "posy", Position.Y );
            writer.Write( "posz", Position.Z );
        }
    }

    public class OsageCollisionParameter
    {
        public uint Type { get; set; }
        public float Radius { get; set; }
        public OsageCollisionBoneParameter Bone0 { get; }
        public OsageCollisionBoneParameter Bone1 { get; }

        internal void Read( EndianBinaryReader reader )
        {
            // X
            if ( reader.AddressSpace == AddressSpace.Int64 )
            {
                Type = reader.ReadUInt32();
                Radius = reader.ReadSingle();

                // Murmur hashes of bone names
                reader.SeekCurrent( 2 * sizeof( ulong ) );
                reader.SkipNulls( 8 );

                Bone0.Position = reader.ReadVector3();
                reader.SkipNulls( 4 );

                Bone1.Position = reader.ReadVector3();
                reader.SkipNulls( 4 );

                Bone0.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Bone1.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            }

            // F 2nd
            else
            {
                reader.SkipNulls( 4 );

                // Murmur hashes of bone names
                reader.SeekCurrent( 2 * sizeof( ulong ) );

                Radius = reader.ReadSingle();

                Bone0.Position = reader.ReadVector3();
                Bone1.Position = reader.ReadVector3();

                reader.SkipNulls( 4 );

                Bone0.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Bone1.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );

                Type = reader.ReadUInt32();
            }
        }

        internal void Write( EndianBinaryWriter writer )
        {
            // X
            if ( writer.AddressSpace == AddressSpace.Int64 )
            {
                writer.Write( Type );
                writer.Write( Radius );

                writer.Write( ( ulong ) MurmurHash.Calculate( Bone0.Name ) );
                writer.Write( ( ulong ) MurmurHash.Calculate( Bone1.Name ) );

                writer.WriteNulls( 4 );

                writer.Write( Bone0.Position );
                writer.WriteNulls( 4 );            
                
                writer.Write( Bone1.Position );
                writer.WriteNulls( 4 );

                writer.AddStringToStringTable( Bone0.Name );
                writer.AddStringToStringTable( Bone1.Name );
            }

            // F 2nd
            else
            {
                writer.Write( ( byte ) 0xD );
                writer.Align( 4 );

                writer.Write( ( ulong ) MurmurHash.Calculate( Bone0.Name ) );
                writer.Write( ( ulong ) MurmurHash.Calculate( Bone1.Name ) );

                writer.Write( Radius );

                writer.Write( Bone0.Position );
                writer.Write( Bone1.Position );

                writer.Write( ( byte ) 0xD );
                writer.Align( 4 );

                writer.AddStringToStringTable( Bone0.Name );
                writer.AddStringToStringTable( Bone1.Name );

                writer.Write( Type );
            }
        }

        internal void Read( ParameterTree tree )
        {
            Type = tree.Get<uint>( "type" );
            Radius = tree.Get<float>( "radius" );

            if ( tree.OpenScope( "bone" ) )
            {
                if ( tree.OpenScope( 0 ) )
                {
                    Bone0.Read( tree );
                    tree.CloseScope();
                }

                if ( tree.OpenScope( 1 ) )
                {
                    Bone1.Read( tree );
                    tree.CloseScope();
                }

                tree.CloseScope();
            }
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.Write( "type", Type );
            writer.Write( "radius", Radius );

            writer.PushScope( "bone" );
            {
                writer.PushScope( 0 );
                {
                    Bone0.Write( writer );
                }
                writer.PopScope();

                writer.PushScope( 1 );
                {
                    Bone1.Write( writer );
                }
                writer.PopScope();
            }
            writer.PopScope();
        }

        internal static void WriteNull( EndianBinaryWriter writer )
        {
            // X
            if ( writer.AddressSpace == AddressSpace.Int64 )
            {
                writer.Write( 0 );
                writer.Write( 0.2f );

                writer.Write( ( ulong ) 0xCAD3078 );
                writer.Write( ( ulong ) 0xCAD3078 );
                writer.WriteNulls( 36 );

                writer.AddStringToStringTable( "" );
                writer.AddStringToStringTable( "" );
            }

            // F 2nd
            else
            {
                writer.WriteNulls( 4 );

                writer.Write( ( ulong ) 0xCAD3078 );
                writer.Write( ( ulong ) 0xCAD3078 );

                writer.Write( 0.2f );

                writer.WriteNulls( 28 );

                writer.AddStringToStringTable( "" );
                writer.AddStringToStringTable( "" );

                writer.Write( 0 );
            }
        }

        public OsageCollisionParameter()
        {
            Bone0 = new OsageCollisionBoneParameter();
            Bone1 = new OsageCollisionBoneParameter();
        }
    }

    public class OsageBocParameter
    {
        public uint StNode { get; set; }
        public uint EdNode { get; set; }
        public string EdRootName { get; set; }

        internal void Read( EndianBinaryReader reader )
        {
            EdNode = reader.ReadUInt32();
            StNode = reader.ReadUInt32();
            EdRootName = reader.ReadString( StringBinaryFormat.FixedLength, 64 );
        }

        internal void Write( EndianBinaryWriter writer )
        {
            writer.Write( EdNode );
            writer.Write( StNode );
            writer.Write( EdRootName, StringBinaryFormat.FixedLength, 64 );
        }

        internal void Read( ParameterTree tree )
        {
            StNode = tree.Get<uint>( "st_node" );
            EdNode = tree.Get<uint>( "ed_node" );
            EdRootName = tree.Get<string>( "ed_root" );
        }

        internal void Write( ParameterTreeWriter writer )
        {
            writer.Write( "st_node", StNode );
            writer.Write( "ed_node", EdNode );
            writer.Write( "ed_root", EdRootName );
        }
    }

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
        public uint Field00 { get; set; } = 2;

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

            // F 2nd
            if ( reader.AddressSpace == AddressSpace.Int32 )
                Field00 = reader.ReadUInt32();

            for ( int i = 0; i < 12; i++ )
            {
                var collision = new OsageCollisionParameter();
                collision.Read( reader );

                if ( !string.IsNullOrEmpty( collision.Bone0.Name ) && !string.IsNullOrEmpty( collision.Bone1.Name ) )
                    Collisions.Add( collision );
            }

            // X
            if ( reader.AddressSpace == AddressSpace.Int64 )
                Field00 = reader.ReadUInt32();

            Friction = reader.ReadSingle();
            WindAffection = reader.ReadSingle();
            CollisionRadius = reader.ReadSingle(); // not sure

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

            // F 2nd
            if ( writer.AddressSpace == AddressSpace.Int32 )
                writer.Write( Field00 );

            int collisionCount = Math.Min( 12, Collisions.Count );
            for ( int i = 0; i < collisionCount; i++ )
                Collisions[ i ].Write( writer );

            for ( int i = 0; i < 12 - collisionCount; i++ )
                OsageCollisionParameter.WriteNull( writer );

            // X
            if ( writer.AddressSpace == AddressSpace.Int64 )
                writer.Write( Field00 );

            writer.Write( Friction );
            writer.Write( WindAffection );
            writer.Write( CollisionRadius );

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

    public class OsageSkinParameterSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<OsageSkinParameter> Parameters { get; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            if ( section != null )
                ReadModern();
            else
                ReadClassic();

            void ReadClassic()
            {
                var paramTree = new ParameterTree( reader );

                foreach ( string key in paramTree.Keys )
                {
                    var param = new OsageSkinParameter();

                    param.Name = key;

                    paramTree.OpenScope( key );
                    {
                        param.Read( paramTree );
                    }
                    paramTree.CloseScope();

                    Parameters.Add( param );
                }
            }

            void ReadModern()
            {
                int count0, count1, count2;
                long offset0, offset1, offset2;

                if ( section.Format == BinaryFormat.X )
                {
                    count0 = reader.ReadInt32();
                    count1 = reader.ReadInt32();
                    count2 = reader.ReadInt32();
                    offset0 = reader.ReadOffset();
                    offset1 = reader.ReadOffset();
                    offset2 = reader.ReadOffset();
                }
                else
                {
                    count0 = reader.ReadInt32();
                    offset0 = reader.ReadOffset();             
                    count1 = reader.ReadInt32();
                    offset1 = reader.ReadOffset();    
                    count2 = reader.ReadInt32();
                    offset2 = reader.ReadOffset();
                }

                reader.ReadAtOffset( offset0, () =>
                {
                    Parameters.Capacity = count0;

                    for ( int i = 0; i < count0; i++ )
                    {
                        var parameter = new OsageSkinParameter();
                        parameter.Read( reader );
                        Parameters.Add( parameter );
                    }
                } );
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
                WriteModern();
            else
                WriteClassic();

            void WriteClassic()
            {
                var paramWriter = new ParameterTreeWriter();

                writer.WriteLine( "# This file was generated automatically. DO NOT EDIT." );

                foreach ( var parameter in Parameters )
                    parameter.Write( paramWriter );

                paramWriter.Flush( writer.BaseStream );
            }

            void WriteModern()
            {
                if ( section.Format == BinaryFormat.X )
                {
                    writer.Write( Parameters.Count );
                    writer.WriteNulls( 2 * sizeof( uint ) );

                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, WriteParameters );
                    writer.WriteNulls( 2 * sizeof( ulong ) );
                }
                else
                {
                    writer.Write( Parameters.Count );
                    writer.ScheduleWriteOffset( 16, AlignmentMode.Left, WriteParameters );
                    writer.WriteNulls( 4 * sizeof( uint ) );
                }
            }

            void WriteParameters()
            {
                foreach ( var parameter in Parameters )
                    parameter.Write( writer );
            }
        }

        public override void Save( string filePath )
        {
            // Assume it's being exported for F 2nd PS3
            if ( Format.IsClassic() && filePath.EndsWith( ".osd", StringComparison.OrdinalIgnoreCase ) )
                Format = BinaryFormat.F2nd;

            // And vice versa
            else if ( Format.IsModern() && filePath.EndsWith( ".txt", StringComparison.OrdinalIgnoreCase ) )
                Format = BinaryFormat.DT;

            base.Save( filePath );
        }

        public OsageSkinParameterSet()
        {
            Parameters = new List<OsageSkinParameter>();
        }
    }
}