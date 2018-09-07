using MikuMikuLibrary.Models;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLModel : IGLDraw
    {
        public List<GLMesh> Meshes { get; }

        public void Dispose()
        {
            foreach ( var mesh in Meshes )
                mesh.Dispose();
        }

        public void Draw( GLShaderProgram shaderProgram )
        {
            foreach ( var mesh in Meshes )
            {
                mesh.Draw( shaderProgram );
            }
        }

        public GLModel( Model model, TextureSet textureSet )
        {
            Meshes = new List<GLMesh>();

            var textures = new Dictionary<int, GLTexture>();
            foreach ( var mesh in model.Meshes )
                Meshes.Add( new GLMesh( mesh, textures, textureSet ) );
        }
    }
}
