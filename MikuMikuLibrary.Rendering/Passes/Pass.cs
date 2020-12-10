using System;
using MikuMikuLibrary.Rendering.Cameras;
using MikuMikuLibrary.Rendering.Scenes;

namespace MikuMikuLibrary.Rendering.Passes
{
    public abstract class Pass : IDisposable
    {
        public abstract void Initialize( Renderer renderer );
        public abstract void Render( Renderer renderer, Camera camera, Scene scene, Effect effect );

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {

        }

        ~Pass()
        {
            Dispose( false );
        }
    }
}