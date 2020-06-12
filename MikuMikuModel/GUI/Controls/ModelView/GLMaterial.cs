using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Textures;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLMaterial : IDisposable
    {
        public GLTexture Diffuse { get; }
        public GLTexture Ambient { get; }
        public GLTexture Normal { get; }
        public GLTexture Specular { get; }
        public GLTexture Reflection { get; }
        public GLTexture Tangent { get; }
        public Color4 DiffuseColor { get; }
        public Color4 SpecularColor { get; }
        public float Shininess { get; }
        public bool UseAniso { get; }

        public void Dispose()
        {
            Diffuse?.Dispose();
            Ambient?.Dispose();
            Normal?.Dispose();
            Specular?.Dispose();
            Reflection?.Dispose();
            Tangent?.Dispose();
        }

        public void Bind( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "uDiffuseColor", DiffuseColor );
            shaderProgram.SetUniform( "uSpecularColor", SpecularColor );
            shaderProgram.SetUniform( "uSpecularPower", Shininess );
            shaderProgram.SetUniform( "uUseAniso", UseAniso );

            int textureIndex = 0;

            void ActivateTexture( GLTexture texture, string uniformName,
                TextureTarget target = TextureTarget.Texture2D )
            {
                GL.ActiveTexture( TextureUnit.Texture0 + textureIndex );
                GL.BindTexture( target, texture?.Id ?? 0 );
                shaderProgram.SetUniform( $"uHas{uniformName}", texture != null );
                shaderProgram.SetUniform( $"u{uniformName}", textureIndex++ );
            }

            ActivateTexture( Diffuse, "DiffuseTexture" );
            ActivateTexture( Ambient, "AmbientTexture" );
            ActivateTexture( Normal, "NormalTexture" );
            ActivateTexture( Specular, "SpecularTexture" );
            ActivateTexture( Reflection, "ReflectionTexture", TextureTarget.TextureCubeMap );
            ActivateTexture( Tangent, "TangentTexture" );
        }

        public GLMaterial( Material material, Dictionary<uint, GLTexture> textures, TextureSet textureSet )
        {
            if ( textureSet == null )
                return;

            Diffuse = GetTexture( material.Diffuse );
            Ambient = GetTexture( material.Ambient );
            Normal = GetTexture( material.Normal );
            Specular = GetTexture( material.Specular );
            Reflection = GetTexture( material.Reflection );
            Tangent = GetTexture( material.Tangent );

            DiffuseColor = material.Shader.Equals( "EYEBALL", StringComparison.OrdinalIgnoreCase )
                ? Color4.White
                : material.DiffuseColor.ToGL();
            SpecularColor = material.SpecularColor.ToGL();
            Shininess = material.Shininess;

            UseAniso = material.Shader.Equals( "HAIR", StringComparison.OrdinalIgnoreCase );

            GLTexture GetTexture( MaterialTexture materialTexture )
            {
                if ( !materialTexture.IsActive )
                    return null;

                if ( textures.TryGetValue( materialTexture.TextureId, out var texture ) ) return texture;

                var textureToUpload =
                    textureSet.Textures.FirstOrDefault( x => x.Id.Equals( materialTexture.TextureId ) );
                if ( textureToUpload == null )
                    return null;

                texture = new GLTexture( textureToUpload );
                textures.Add( textureToUpload.Id, texture );
                return texture;
            }
        }
    }
}