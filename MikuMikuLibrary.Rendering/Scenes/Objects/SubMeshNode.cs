using System.Numerics;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Rendering.Arrays;
using MikuMikuLibrary.Rendering.Materials;

namespace MikuMikuLibrary.Rendering.Scenes.Objects
{
    public sealed class SubMeshNode : Node
    {
        public VertexArray VertexArray { get; }
        public ElementArray ElementArray { get; }
        public Material Material { get; }
        public BoundingSphere BoundingSphere { get; }

        public override void Render( Scheduler scheduler, Scene scene, Matrix4x4 parentWorldTransformation )
        {
            scheduler.Render( new RenderCommand
            {
                VertexArray = VertexArray, ElementArray = ElementArray, Material = Material,
                Transformation = parentWorldTransformation * GetTransformation(), BoundingSphere = BoundingSphere
            } );

            base.Render( scheduler, scene, parentWorldTransformation );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                ElementArray.Dispose();

            base.Dispose( disposing );
        }

        public SubMeshNode( State state, VertexArray vertexArray, SubMesh subMesh, Material material )
        {
            VertexArray = vertexArray;
            ElementArray = new ElementArray( state, subMesh );
            Material = material;
            BoundingSphere = subMesh.BoundingSphere;
        }
    }
}