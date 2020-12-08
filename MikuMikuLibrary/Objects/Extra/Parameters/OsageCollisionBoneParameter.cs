using System.Numerics;
using MikuMikuLibrary.Parameters;

namespace MikuMikuLibrary.Objects.Extra.Parameters
{
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
}