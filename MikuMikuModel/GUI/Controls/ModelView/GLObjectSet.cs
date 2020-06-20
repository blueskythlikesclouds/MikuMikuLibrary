using System.Collections.Generic;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLObjectSet : IDrawable
    {
        public List<GLObject> Objects { get; }

        public void Dispose()
        {
            foreach ( var mesh in Objects )
                mesh.Dispose();
        }

        public void Draw( GLShaderProgram shaderProgram )
        {
            foreach ( var mesh in Objects ) mesh.Draw( shaderProgram );
        }

        public GLObjectSet( ObjectSet objectSet, TextureSet textureSet )
        {
            Objects = new List<GLObject>();

            var textures = new Dictionary<uint, GLTexture>();

            foreach ( var obj in objectSet.Objects )
                Objects.Add( new GLObject( obj, textures, textureSet ) );
        }
    }
}