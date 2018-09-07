using MikuMikuLibrary.Models;
using MikuMikuLibrary.Textures;
using System.Collections.Generic;
using System.Linq;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLMesh : IGLDraw
    {
        public List<GLSubMesh> SubMeshes { get; }
        public List<GLMaterial> Materials { get; }

        public void Dispose()
        {
            foreach ( var subMesh in SubMeshes )
                subMesh.Dispose();

            foreach ( var material in Materials )
                material.Dispose();
        }

        public void Draw( GLShaderProgram shaderProgram )
        {
            foreach ( var subMesh in SubMeshes )
            {
                subMesh.Draw( shaderProgram );
            }
        }

        public GLMesh( Mesh mesh, Dictionary<int, GLTexture> textures, TextureSet textureSet )
        {
            Materials = new List<GLMaterial>();
            foreach ( var material in mesh.Materials )
                Materials.Add( new GLMaterial( material, textures, textureSet ) );

            SubMeshes = new List<GLSubMesh>();
            foreach ( var subMesh in mesh.SubMeshes.Where( x => x.Vertices != null ) )
                SubMeshes.Add( new GLSubMesh( subMesh, Materials ) );
        }
    }
}
