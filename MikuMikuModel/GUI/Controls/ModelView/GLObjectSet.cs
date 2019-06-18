using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLObjectSet : IGLDraw
    {
        public List<GLObject> Objects { get; }

        public void Dispose()
        {
            foreach ( var mesh in Objects )
                mesh.Dispose();
        }

        public void Draw( GLShaderProgram shaderProgram )
        {
            foreach ( var mesh in Objects )
            {
                mesh.Draw( shaderProgram );
            }
        }

        public GLObjectSet( ObjectSet objectSet, TextureSet textureSet )
        {
            Objects = new List<GLObject>();

            var textures = new Dictionary<int, GLTexture>();
            foreach ( var obj in objectSet.Objects )
                Objects.Add( new GLObject( obj, textures, textureSet ) );
        }
    }
}
