using System.Collections.Generic;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Rendering.Materials;

namespace MikuMikuLibrary.Rendering.Scenes.Objects
{
    public sealed class ObjectNode : Node
    {
        public ObjectNode( State state, Object obj )
        {
            var materials = new List<Material>( obj.Materials.Count );

            foreach ( var material in obj.Materials )
                materials.Add( new Material( material ) );

            foreach ( var mesh in obj.Meshes )
                Nodes.Add( new MeshNode( state, mesh, materials ) );
        }
    }
}