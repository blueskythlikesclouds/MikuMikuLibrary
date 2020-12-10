using MikuMikuLibrary.Objects;

namespace MikuMikuLibrary.Rendering.Scenes.Objects
{
    public sealed class ObjectSetNode : Node
    {
        public ObjectSetNode( State state, ObjectSet objectSet )
        {
            foreach ( var obj in objectSet.Objects )
                Nodes.Add( new ObjectNode( state, obj ) );
        }
    }
}