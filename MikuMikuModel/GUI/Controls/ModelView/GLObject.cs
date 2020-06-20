using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Objects;
using MikuMikuLibrary.Textures;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLObject : IDrawable
    {
        public List<GLMesh> Meshes { get; }
        public List<GLMaterial> Materials { get; }

        public void Dispose()
        {
            foreach ( var mesh in Meshes )
                mesh.Dispose();

            foreach ( var material in Materials )
                material.Dispose();
        }

        public void Draw( GLShaderProgram shaderProgram )
        {
            foreach ( var mesh in Meshes ) mesh.Draw( shaderProgram );
        }

        public GLObject( Object obj, Dictionary<uint, GLTexture> textures, TextureSet textureSet )
        {
            Materials = new List<GLMaterial>();

            foreach ( var material in obj.Materials )
                Materials.Add( new GLMaterial( material, textures, textureSet ) );

            Meshes = new List<GLMesh>();

            foreach ( var mesh in obj.Meshes.Where( x => x.Positions != null ) )
                Meshes.Add( new GLMesh( mesh, Materials ) );
        }
    }
}