using System;
using MikuMikuLibrary.IBLs;

// we breakin the law here
using MMLIBL = MikuMikuLibrary.IBLs.IBL;

namespace MikuMikuLibrary.Rendering.IBLs
{
    public sealed class IBL : IDisposable
    {
        public MMLIBL SourceIBL { get; }

        public LightMap DiffuseIBL { get; }
        public LightMap DiffuseIBLShadowed { get; }
        public LightMap SpecularIBLShiny { get; }
        public LightMap SpecularIBLRough { get; }
        public LightMap SpecularIBLShinyShadowed { get; }
        public LightMap SpecularIBLRoughShadowed { get; }

        public void Bind( State state )
        {
            DiffuseIBL.Bind( state, Sampler.DiffuseIBL.TextureUnit );
            DiffuseIBLShadowed.Bind( state, Sampler.DiffuseIBLShadowed.TextureUnit );
            SpecularIBLShiny.Bind( state, Sampler.SpecularIBLShiny.TextureUnit );
            SpecularIBLRough.Bind( state, Sampler.SpecularIBLRough.TextureUnit );
            SpecularIBLShinyShadowed.Bind( state, Sampler.SpecularIBLShinyShadowed.TextureUnit );
            SpecularIBLRoughShadowed.Bind( state, Sampler.SpecularIBLRoughShadowed.TextureUnit );
        }

        public void Dispose()
        {
            DiffuseIBL.Dispose();
            DiffuseIBLShadowed.Dispose();
            SpecularIBLShiny.Dispose();
            SpecularIBLRough.Dispose();
            SpecularIBLShinyShadowed.Dispose();
            SpecularIBLRoughShadowed.Dispose();
        }

        public IBL( State state, MMLIBL ibl )
        {
            SourceIBL = ibl;
            DiffuseIBL = new LightMap( state, ibl.LightMaps[ ( int ) LightMapType.DiffuseIBL ], false );
            DiffuseIBLShadowed = new LightMap( state, ibl.LightMaps[ ( int ) LightMapType.DiffuseIBLShadowed ], false );
            SpecularIBLShiny = new LightMap( state, ibl.LightMaps[ ( int ) LightMapType.SpecularIBLShiny ], true );
            SpecularIBLRough = new LightMap( state, ibl.LightMaps[ ( int ) LightMapType.SpecularIBLRough ], true );
            SpecularIBLShinyShadowed = new LightMap( state, ibl.LightMaps[ ( int ) LightMapType.SpecularIBLShinyShadowed ], true );
            SpecularIBLRoughShadowed = new LightMap( state, ibl.LightMaps[ ( int ) LightMapType.SpecularIBLRoughShadowed ], true );
        }
    }
}