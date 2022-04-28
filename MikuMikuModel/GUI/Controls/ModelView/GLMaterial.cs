using System;
using System.Collections.Generic;
using System.Linq;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Textures;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public class GLMaterial : IDisposable
    {
        private static readonly int[] sBlendTypeMap = { 0, 1, 0x300, 0x301, 0x302, 0x303, 0x304, 0x305, 0x306, 0x307, 0x308, 0, 0, 0 };

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
        public bool CullFace { get; }
        public bool PunchThrough { get; }
        public bool IsTransparent { get; }
        public BlendingFactorSrc SrcBlendingFactor { get; }
        public BlendingFactorDest DstBlendingFactor { get; }
        public Matrix4 DiffuseTransformation { get; private set; }
        public Matrix4 AmbientTransformation { get; private set; }

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

            if ( CullFace )
                GL.Enable( EnableCap.CullFace );
            else
                GL.Disable( EnableCap.CullFace );

            shaderProgram.SetUniform( "uPunchThrough", PunchThrough );
            GL.BlendFuncSeparate( SrcBlendingFactor, DstBlendingFactor, BlendingFactorSrc.OneMinusDstAlpha, BlendingFactorDest.One );

            shaderProgram.SetUniform( "uDiffuseTransformation", DiffuseTransformation );
            shaderProgram.SetUniform( "uAmbientTransformation", AmbientTransformation );

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
            DiffuseTransformation = Matrix4.Identity;
            AmbientTransformation = Matrix4.Identity;

            if ( textureSet == null )
                return;

            Diffuse = GetTexture( MaterialTextureType.Color, 0 );
            Ambient = GetTexture( MaterialTextureType.Color, 1 );

            if ( Diffuse == Ambient )
                Ambient = null;

            Normal = GetTexture( MaterialTextureType.Normal, 0 );
            Specular = GetTexture( MaterialTextureType.Specular, 0 );
            Reflection = GetTexture( MaterialTextureType.EnvironmentCube, 0 );

            DiffuseColor = material.Diffuse.ToGL();
            SpecularColor = material.Specular.ToGL();
            AmbientColor = new Color4( 0.25f, 0.25f, 0.25f, 1.0f );
            Shininess = material.Shininess;
            AnisoDirection = material.AnisoDirection;

            CullFace = !material.DoubleSided;
            PunchThrough = material.PunchThrough;
            IsTransparent = ( material.AlphaTexture || material.AlphaMaterial ) && !material.PunchThrough;
            SrcBlendingFactor = ( BlendingFactorSrc ) sBlendTypeMap[ ( int ) material.SrcBlendFactor ];
            DstBlendingFactor = ( BlendingFactorDest ) sBlendTypeMap[ ( int ) material.DstBlendFactor ];

            GLTexture GetTexture( MaterialTextureType type, uint offset )
            {
                uint index = 0;

                foreach ( var materialTexture in material.MaterialTextures )
                {
                    if ( materialTexture.Type != type )
                        continue;

                    if ( index++ < offset )
                        continue;

                    if ( type == MaterialTextureType.Color )
                    {
                        if ( offset == 0 )
                            DiffuseTransformation = materialTexture.TextureCoordinateMatrix.ToGL();

                        else if ( offset == 1 )
                            AmbientTransformation = materialTexture.TextureCoordinateMatrix.ToGL();
                    }

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