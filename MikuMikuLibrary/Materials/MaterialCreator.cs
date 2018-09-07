using MikuMikuLibrary.Materials;
using MikuMikuLibrary.Textures;

namespace MikuMikuLibrary.Processing.Materials
{
    public static class MaterialCreator
    {
        //public static Material CreateSkinMaterialF( Texture diffuse, Texture ambient, Texture specular, Texture toonCurve )
        //{
        //    var material = new Material();
        //    material.Field00 = 129;
        //    material.Shader = "SKIN";

        //    if ( diffuse != null )
        //    {
        //        material.Diffuse.TextureID = diffuse.ID;
        //        material.Diffuse.Field00 = 48;
        //        material.Diffuse.Field01 = 2359520;
        //        material.Diffuse.Field02 = 241;
        //        material.Diffuse.Field05 = 1f;
        //        material.Diffuse.Field06 = 1f;
        //        material.Diffuse.Field11 = 1f;
        //        material.Diffuse.Field16 = 1f;
        //        material.Diffuse.Field21 = 1f;
        //    }

        //    if ( ambient != null )
        //    {
        //        material.Ambient.TextureID = ambient.ID;
        //        material.Ambient.Field01 = 2359520;
        //        material.Ambient.Field02 = 241;
        //        material.Ambient.Field05 = 1f;
        //        material.Ambient.Field06 = 1f;
        //        material.Ambient.Field11 = 1f;
        //        material.Ambient.Field16 = 1f;
        //        material.Ambient.Field21 = 1f;
        //    }

        //    if ( specular != null )
        //    {
        //        material.Specular.TextureID = specular.ID;
        //        material.Specular.Field01 = 2359520;
        //        material.Specular.Field02 = 243;
        //        material.Specular.Field05 = 1f;
        //        material.Specular.Field06 = 1f;
        //        material.Specular.Field11 = 1f;
        //        material.Specular.Field16 = 1f;
        //        material.Specular.Field21 = 1f;
        //    }

        //    if ( toonCurve != null )
        //    {
        //        material.ToonCurve.TextureID = toonCurve.ID;
        //        material.ToonCurve.Field01 = 327904;
        //        material.ToonCurve.Field02 = 241;
        //        material.ToonCurve.Field05 = 1f;
        //        material.ToonCurve.Field06 = 1f;
        //        material.ToonCurve.Field11 = 1f;
        //        material.ToonCurve.Field16 = 1f;
        //        material.ToonCurve.Field21 = 1f;
        //    }

        //    material.Field02 = 2688;
        //    material.Field03 = 1f;
        //    material.Field04 = 1f;
        //    material.Field05 = 1f;
        //    material.Field06 = 1f;
        //    material.Field07 = 0.3f;
        //    material.Field08 = 0.3f;
        //    material.Field09 = 0.3f;
        //    material.Field10 = 1f;
        //    material.Field11 = 1f;
        //    material.Field12 = 1f;
        //    material.Field13 = 1f;
        //    material.Field14 = 0.8f;
        //    material.Field18 = 1f;
        //    material.Shininess = 10f;
        //    material.Field25 = 1f;

        //    return material;
        //}

        //public static Material CreateClothMaterialF( Texture diffuse, Texture ambient, Texture specular, Texture toonCurve )
        //{
        //    var material = new Material();
        //    material.Field00 = 129;
        //    material.Shader = "SKIN";

        //    if ( diffuse != null )
        //    {
        //        material.Diffuse.TextureID = diffuse.ID;
        //        material.Diffuse.Field00 = 48;
        //        material.Diffuse.Field01 = 2359520;
        //        material.Diffuse.Field02 = 241;
        //        material.Diffuse.Field05 = 1f;
        //        material.Diffuse.Field06 = 1f;
        //        material.Diffuse.Field11 = 1f;
        //        material.Diffuse.Field16 = 1f;
        //        material.Diffuse.Field21 = 1f;
        //    }

        //    if ( ambient != null )
        //    {
        //        material.Ambient.TextureID = ambient.ID;
        //        material.Ambient.Field01 = 2359520;
        //        material.Ambient.Field02 = 241;
        //        material.Ambient.Field05 = 1f;
        //        material.Ambient.Field06 = 1f;
        //        material.Ambient.Field11 = 1f;
        //        material.Ambient.Field16 = 1f;
        //        material.Ambient.Field21 = 1f;
        //    }

        //    if ( specular != null )
        //    {
        //        material.Specular.TextureID = specular.ID;
        //        material.Specular.Field01 = 2359520;
        //        material.Specular.Field02 = 243;
        //        material.Specular.Field05 = 1f;
        //        material.Specular.Field06 = 1f;
        //        material.Specular.Field11 = 1f;
        //        material.Specular.Field16 = 1f;
        //        material.Specular.Field21 = 1f;
        //    }

        //    if ( toonCurve != null )
        //    {
        //        material.ToonCurve.TextureID = toonCurve.ID;
        //        material.ToonCurve.Field01 = 327904;
        //        material.ToonCurve.Field02 = 241;
        //        material.ToonCurve.Field05 = 1f;
        //        material.ToonCurve.Field06 = 1f;
        //        material.ToonCurve.Field11 = 1f;
        //        material.ToonCurve.Field16 = 1f;
        //        material.ToonCurve.Field21 = 1f;
        //    }

        //    material.Field02 = 2688;
        //    material.Field03 = 1f;
        //    material.Field04 = 1f;
        //    material.Field05 = 1f;
        //    material.Field06 = 1f;
        //    material.Field07 = 0.3f;
        //    material.Field08 = 0.3f;
        //    material.Field09 = 0.3f;
        //    material.Field10 = 1f;
        //    material.Field11 = 1f;
        //    material.Field12 = 1f;
        //    material.Field13 = 1f;
        //    material.Field14 = 0.8f;
        //    material.Field18 = 1f;
        //    material.Shininess = 6f;
        //    material.Field25 = 1f;

        //    return material;
        //}

        //public static Material CreateHairMaterialF( Texture diffuse, Texture ambient, Texture specular, Texture toonCurve )
        //{
        //    var material = new Material();
        //    material.Field00 = 129;
        //    material.Shader = "HAIR";

        //    if ( diffuse != null )
        //    {
        //        material.Diffuse.TextureID = diffuse.ID;
        //        material.Diffuse.Field00 = 48;
        //        material.Diffuse.Field01 = 2359520;
        //        material.Diffuse.Field02 = 241;
        //        material.Diffuse.Field05 = 1f;
        //        material.Diffuse.Field06 = 1f;
        //        material.Diffuse.Field11 = 1f;
        //        material.Diffuse.Field16 = 1f;
        //        material.Diffuse.Field21 = 1f;
        //    }

        //    if ( ambient != null )
        //    {
        //        material.Ambient.TextureID = ambient.ID;
        //        material.Ambient.Field01 = 2359520;
        //        material.Ambient.Field02 = 241;
        //        material.Ambient.Field05 = 1f;
        //        material.Ambient.Field06 = 1f;
        //        material.Ambient.Field11 = 1f;
        //        material.Ambient.Field16 = 1f;
        //        material.Ambient.Field21 = 1f;
        //    }

        //    if ( specular != null )
        //    {
        //        material.Specular.TextureID = specular.ID;
        //        material.Specular.Field01 = 2359520;
        //        material.Specular.Field02 = 243;
        //        material.Specular.Field05 = 1f;
        //        material.Specular.Field06 = 1f;
        //        material.Specular.Field11 = 1f;
        //        material.Specular.Field16 = 1f;
        //        material.Specular.Field21 = 1f;
        //    }

        //    if ( toonCurve != null )
        //    {
        //        material.ToonCurve.TextureID = toonCurve.ID;
        //        material.ToonCurve.Field01 = 327904;
        //        material.ToonCurve.Field02 = 241;
        //        material.ToonCurve.Field05 = 1f;
        //        material.ToonCurve.Field06 = 1f;
        //        material.ToonCurve.Field11 = 1f;
        //        material.ToonCurve.Field16 = 1f;
        //        material.ToonCurve.Field21 = 1f;
        //    }

        //    material.Field02 = 2688;
        //    material.Field03 = 1f;
        //    material.Field04 = 1f;
        //    material.Field05 = 1f;
        //    material.Field06 = 1f;
        //    material.Field07 = 0.3f;
        //    material.Field08 = 0.3f;
        //    material.Field09 = 0.3f;
        //    material.Field10 = 1f;
        //    material.Field11 = 1f;
        //    material.Field12 = 1f;
        //    material.Field13 = 1f;
        //    material.Field14 = 0.8f;
        //    material.Field18 = 1f;
        //    material.Shininess = 10f;
        //    material.Field25 = 1f;

        //    return material;
        //}

        public static Material CreatePhongMaterialF( Texture diffuse )
        {
            var material = new Material();
            material.Field00 = 1;
            material.Shader = "BLINN";

            if ( diffuse != null )
            {
                material.Diffuse.TextureID = diffuse.ID;
                material.Diffuse.Field00 = 48;
                material.Diffuse.Field01 = 2365635;
                material.Diffuse.Field02 = 241;
                material.Diffuse.Field05 = 1f;
                material.Diffuse.Field06 = 1f;
                material.Diffuse.Field11 = 1f;
                material.Diffuse.Field16 = 1f;
                material.Diffuse.Field21 = 1f;
            }

            material.Field02 = 2688;
            //material.Field03 = 1f;
            //material.Field04 = 1f;
            //material.Field05 = 1f;
            //material.Field06 = 1f;
            //material.Field10 = 1f;
            //material.Field11 = 0.9092721f;
            //material.Field12 = 0.9215686f;
            //material.Field13 = 0.7589389f;
            //material.Field14 = 0.5f;
            //material.Field15 = 0.5f;
            //material.Field16 = 0.5f;
            //material.Field17 = 0.5f;
            //material.Field18 = 1f;
            material.DiffuseColor = new Misc.Color( 1, 1, 1, 1 );
            material.SpecularColor = new Misc.Color( 1, 1, 1, 1 );
            material.Shininess = 20f;
            material.Field25 = 1f;

            return material;
        }
    }
}
