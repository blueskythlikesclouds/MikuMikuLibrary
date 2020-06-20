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
        public Color4 DiffuseColor { get; }
        public Color4 AmbientColor { get; }
        public Color4 SpecularColor { get; }
        public float Shininess { get; }
        public AnisoDirection AnisoDirection { get; }

        public void Dispose()
        {
            Diffuse?.Dispose();
            Ambient?.Dispose();
            Normal?.Dispose();
            Specular?.Dispose();
            Reflection?.Dispose();
        }

        public void Bind( GLShaderProgram shaderProgram )
        {
            shaderProgram.SetUniform( "uDiffuseColor", DiffuseColor );
            shaderProgram.SetUniform( "uSpecularColor", SpecularColor );
            shaderProgram.SetUniform( "uAmbientColor", AmbientColor );
            shaderProgram.SetUniform( "uShininess", Shininess );
            shaderProgram.SetUniform( "uAnisoDirection", ( int ) AnisoDirection );

            int textureIndex = 0;

            void ActivateTexture( GLTexture texture, string uniformName, TextureTarget target = TextureTarget.Texture2D )
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
        }

        public GLMaterial( Material material, Dictionary<uint, GLTexture> textures, TextureSet textureSet )
        {
            if ( textureSet == null )
                return;

            Diffuse = GetTexture( MaterialTextureType.Color, 0 );
            Ambient = GetTexture( MaterialTextureType.Color, 1 );
            Normal = GetTexture( MaterialTextureType.Normal, 0 );
            Specular = GetTexture( MaterialTextureType.Specular, 0 );
            Reflection = GetTexture( MaterialTextureType.EnvironmentCube, 0 );

            DiffuseColor = material.Diffuse.ToGL();
            SpecularColor = material.Specular.ToGL();
            AmbientColor = new Color4( 0.25f, 0.25f, 0.25f, 1.0f );
            Shininess = material.Shininess;
            AnisoDirection = material.AnisoDirection;

            GLTexture GetTexture( MaterialTextureType type, uint offset )
            {
                uint index = 0;

                foreach ( var materialTexture in material.MaterialTextures )
                {
                    if ( materialTexture.Type != type )
                        continue;

                    if ( index++ < offset )
                        continue;

                    if ( textures.TryGetValue( materialTexture.TextureId, out var texture ) )
                        return texture;

                    var textureToUpload = textureSet.Textures.FirstOrDefault( x => x.Id.Equals( materialTexture.TextureId ) );

                    if ( textureToUpload == null )
                        return null;

                    texture = new GLTexture( textureToUpload );
                    textures.Add( textureToUpload.Id, texture );
                    return texture;
                }

                return null;
            }
        }
    }
}