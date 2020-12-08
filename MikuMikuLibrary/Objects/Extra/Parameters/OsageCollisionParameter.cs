using MikuMikuLibrary.Hashes;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters
{
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
                Type = reader.ReadUInt32();
                reader.SkipNulls( 4 );

                // Murmur hashes of bone names
                reader.SeekCurrent( 2 * sizeof( ulong ) );

                Radius = reader.ReadSingle();

                Bone0.Position = reader.ReadVector3();
                Bone1.Position = reader.ReadVector3();

                reader.SkipNulls( 4 );

                Bone0.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
                Bone1.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
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

                writer.WriteNulls( 8 );

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
                writer.Write( Type );
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
                writer.WriteNulls( 8 );

                writer.Write( ( ulong ) 0xCAD3078 );
                writer.Write( ( ulong ) 0xCAD3078 );

                writer.Write( 0.2f );

                writer.WriteNulls( 28 );

                writer.AddStringToStringTable( "" );
                writer.AddStringToStringTable( "" );
            }
        }

        public OsageCollisionParameter()
        {
            Bone0 = new OsageCollisionBoneParameter();
            Bone1 = new OsageCollisionBoneParameter();
        }
    }
}