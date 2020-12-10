using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Rendering.Scenes;
using MikuMikuLibrary.Rendering.Shaders;
using OpenTK.Graphics.OpenGL;
using MMLMaterial = MikuMikuLibrary.Materials.Material;

namespace MikuMikuLibrary.Rendering.Materials
{
    public class Material
    {
        private static readonly IReadOnlyList<( MaterialTextureType Type, int Offset, Sampler Sampler)> sSamplerMap = new[]
        {
            ( MaterialTextureType.Color, 0, Sampler.Diffuse ),
            ( MaterialTextureType.Color, 1, Sampler.Ambient ),
            ( MaterialTextureType.Normal, 0, Sampler.Normal ),
            ( MaterialTextureType.Specular, 0, Sampler.Specular ),
            ( MaterialTextureType.EnvironmentCube, 0, EnvironmentCube: Sampler.Environment ),
            ( MaterialTextureType.Transparency, 0, Sampler.Transparency ),
            ( MaterialTextureType.Color, 2, Sampler.ToonCurve )
        };

        private static readonly int[] sBlendTypeMap = { 0, 1, 0x300, 0x301, 0x302, 0x303, 0x304, 0x305, 0x306, 0x307, 0x308, 0, 0, 0 };

        private static readonly ShaderFlags[] sAnisoDirectionMap =
            { ShaderFlags.AnisoDirectionNormal, ShaderFlags.AnisoDirectionU, ShaderFlags.AnisoDirectionV, ShaderFlags.AnisoDirectionRadial };

        public MMLMaterial SourceMaterial { get; }

        public bool IsTransparent()
        {
            return ( SourceMaterial.AlphaTexture || SourceMaterial.AlphaMaterial ) && !SourceMaterial.PunchThrough;
        }

        public void Bind( Renderer renderer, Shader shader, Scene scene )
        {
            renderer.State.CullFace( !SourceMaterial.DoubleSided );
            renderer.State.CullFaceMode( CullFaceMode.Back );

            renderer.State.BlendFunc(
                ( BlendingFactor ) sBlendTypeMap[ ( int ) SourceMaterial.SrcBlendFactor ],
                ( BlendingFactor ) sBlendTypeMap[ ( int ) SourceMaterial.DstBlendFactor ] );

            var data = new MaterialData
            {
                Diffuse = SourceMaterial.Diffuse,
                Ambient = SourceMaterial.Ambient,
                Specular = SourceMaterial.Specular,
                Emission = SourceMaterial.Emission,
                FresnelCoefficientAndShininess = new Vector4(
                    ( SourceMaterial.Fresnel == 0 ? 7.0f : SourceMaterial.Fresnel - 1.0f ) * 0.12f * 0.82f,
                    0.18f,
                    SourceMaterial.LineLight * 0.111f,
                    SourceMaterial.Shininess ),

                ShaderFlags = sAnisoDirectionMap[ ( int ) SourceMaterial.AnisoDirection ] |
                              ( SourceMaterial.PunchThrough ? ShaderFlags.PunchThrough : ShaderFlags.None ),

                ShaderName = ( uint ) MMLMaterial.ShaderNames.IndexOf( SourceMaterial.ShaderName ),
            };

            foreach ( ( var type, int offset, var sampler ) in sSamplerMap )
            {
                var materialTexture = GetMaterialTexture( type, offset );

                if ( materialTexture == null || !scene.Textures.TryGetValue( materialTexture.TextureId, out var texture ) )
                    continue;

                texture.Bind( renderer.State, sampler.TextureUnit );

                GL.TextureParameter( texture.Id, TextureParameterName.TextureWrapS, ( int ) (
                    materialTexture.MirrorU ? TextureWrapMode.MirroredRepeat :
                    materialTexture.RepeatU ? TextureWrapMode.Repeat :
                    TextureWrapMode.ClampToEdge ) );

                GL.TextureParameter( texture.Id, TextureParameterName.TextureWrapT, ( int ) (
                    materialTexture.MirrorV ? TextureWrapMode.MirroredRepeat :
                    materialTexture.RepeatV ? TextureWrapMode.Repeat :
                    TextureWrapMode.ClampToEdge ) );

                data.ShaderFlags |= ( ShaderFlags ) ( 1 << sampler.TextureUnit );

                if ( sampler == Sampler.Diffuse )
                    data.DiffuseTransformation = materialTexture.TextureCoordinateMatrix;

                else if ( sampler == Sampler.Ambient )
                    data.AmbientTransformation = materialTexture.TextureCoordinateMatrix;
            }

            renderer.MaterialUniformBuffer.SetData( renderer.State, data );
        }

        private MaterialTexture GetMaterialTexture( MaterialTextureType type, int offset )
        {
            uint index = 0;
            return SourceMaterial.MaterialTextures.Where( materialTexture =>
                materialTexture.Type == type ).FirstOrDefault( materialTexture => index++ >= offset );
        }

        public Material( MMLMaterial material )
        {
            SourceMaterial = material;
        }
    }
}