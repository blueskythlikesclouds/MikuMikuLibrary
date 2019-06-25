using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLObject : IGLDraw
    {
        public List<GLMesh> Meshes { get; }
        public List<GLMaterial> Materials { get; }

        public void Dispose()
        {
            foreach ( var subMesh in Meshes )
                subMesh.Dispose();

            foreach ( var material in Materials )
                material.Dispose();
        }

        public void Draw( GLShaderProgram shaderProgram )
        {
            foreach ( var subMesh in Meshes )
            {
                subMesh.Draw( shaderProgram );
            }
        }

        public GLObject( Object obj, Dictionary<int, GLTexture> textures, TextureSet textureSet )
        {
            Materials = new List<GLMaterial>();
            foreach ( var material in obj.Materials )
                Materials.Add( new GLMaterial( material, textures, textureSet ) );

            Meshes = new List<GLMesh>();
            foreach ( var mesh in obj.Meshes.Where( x => x.Vertices != null ) )
                Meshes.Add( new GLMesh( mesh, Materials ) );
        }
    }
}
