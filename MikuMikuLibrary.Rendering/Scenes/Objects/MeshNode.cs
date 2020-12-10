using System.Collections.Generic;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Rendering.Arrays;
using MikuMikuLibrary.Rendering.Materials;

namespace MikuMikuLibrary.Rendering.Scenes.Objects
{
    public sealed class MeshNode : Node
    {
        public VertexArray VertexArray { get; }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                VertexArray.Dispose();

            base.Dispose( disposing );
        }

        public MeshNode( State state, Mesh mesh, IReadOnlyList<Material> materials )
        {
            VertexArray = new VertexArray( state, mesh );

            foreach ( var subMesh in mesh.SubMeshes )
                Nodes.Add( new SubMeshNode( state, VertexArray, subMesh, materials[ ( int ) subMesh.MaterialIndex ] ) );
        }
    }
}