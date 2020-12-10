using System.Collections.Generic;
using MikuMikuLibrary.Rendering.Shaders;
using MikuMikuLibrary.Rendering.Textures;

namespace MikuMikuLibrary.Rendering
{
    public class Sampler
    {
        public static readonly Sampler Diffuse = new Sampler( 0, nameof( Diffuse ) );
        public static readonly Sampler Ambient = new Sampler( 1, nameof( Ambient ) );
        public static readonly Sampler Normal = new Sampler( 2, nameof( Normal ) );
        public static readonly Sampler Specular = new Sampler( 3, nameof( Specular ) );
        public static readonly Sampler Transparency = new Sampler( 4, nameof( Transparency ) );
        public static readonly Sampler Environment = new Sampler( 5, nameof( Environment ) );
        public static readonly Sampler ToonCurve = new Sampler( 6, nameof( ToonCurve ) );

        public static readonly Sampler ShadowMap = new Sampler( 7, nameof( ShadowMap ) );
        public static readonly Sampler SSS = new Sampler( 8, nameof( SSS ) );

        public static readonly Sampler DiffuseIBL = new Sampler( 9, nameof( DiffuseIBL ) );
        public static readonly Sampler DiffuseIBLShadowed = new Sampler( 10, nameof( DiffuseIBLShadowed ) );
        public static readonly Sampler SpecularIBLShiny = new Sampler( 11, nameof( SpecularIBLShiny ) );
        public static readonly Sampler SpecularIBLRough = new Sampler( 12, nameof( SpecularIBLRough ) );
        public static readonly Sampler SpecularIBLShinyShadowed = new Sampler( 13, nameof( SpecularIBLShinyShadowed ) );
        public static readonly Sampler SpecularIBLRoughShadowed = new Sampler( 14, nameof( SpecularIBLRoughShadowed ) );

        public static readonly IReadOnlyList<Sampler> All = new[]
        {
            Diffuse,
            Ambient,
            Normal,
            Specular,
            Environment,
            Transparency,
            ToonCurve,

            ShadowMap,
            SSS,

            DiffuseIBL,
            DiffuseIBLShadowed,
            SpecularIBLShiny,
            SpecularIBLRough,
            SpecularIBLShinyShadowed,
            SpecularIBLRoughShadowed
        };

        public int TextureUnit { get; }
        public string UniformName { get; }

        public void Bind( State state, Texture texture )
        {
            texture.Bind( state, TextureUnit );
        }

        public void Bind( Shader shader )
        {
            shader.SetUniform( UniformName, TextureUnit );
        }

        public Sampler( int textureUnit, string name )
        {
            TextureUnit = textureUnit;
            UniformName = $"u{name}Texture";
        }
    }
}