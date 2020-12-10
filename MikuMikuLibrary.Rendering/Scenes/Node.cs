using System;
using System.Collections.Generic;
using System.Numerics;

namespace MikuMikuLibrary.Rendering.Scenes
{
    public class Node : IDisposable
    {
        private bool mDisposed;

        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public List<Node> Nodes { get; }

        public Matrix4x4 GetTransformation()
        {
            return Matrix4x4.CreateTranslation( Translation ) *
                   Matrix4x4.CreateFromYawPitchRoll( Rotation.Y, Rotation.X, Rotation.Z ) *
                   Matrix4x4.CreateScale( Scale );
        }

        public virtual void Render( Scheduler scheduler, Scene scene, Matrix4x4 parentWorldTransformation )
        {
            if ( Nodes.Count == 0 )
                return;

            var worldTransformation = parentWorldTransformation * GetTransformation();

            foreach ( var node in Nodes )
                node.Render( scheduler, scene, worldTransformation );
        }

        public void Dispose()
        {
            if ( mDisposed )
                return;

            mDisposed = true;

            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposing )
                return;

            foreach ( var node in Nodes )
                node.Dispose();
        }

        public Node()
        {
            Scale = Vector3.One;
            Nodes = new List<Node>();
        }

        ~Node()
        {
            Dispose( false );
        }
    }
}