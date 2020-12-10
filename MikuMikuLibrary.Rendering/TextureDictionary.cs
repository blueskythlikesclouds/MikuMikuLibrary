using System;
using System.Collections.Generic;
using MikuMikuLibrary.Rendering.Textures;

namespace MikuMikuLibrary.Rendering.Scenes
{
    public sealed class TextureDictionary : IDisposable
    {
        private readonly Dictionary<uint, Texture> mTextures;

        public Texture this[ uint id ] => mTextures[ id ];

        public void Add( uint id, Texture texture )
        {
            mTextures.Add( id, texture );
        }

        public bool TryGetValue( uint id, out Texture texture )
        {
            return mTextures.TryGetValue( id, out texture );
        }

        public void Clear()
        {
            foreach ( var texture in mTextures )
                texture.Value.Dispose();

            mTextures.Clear();
        }

        public void Dispose()
        {
            foreach ( var texture in mTextures )
                texture.Value.Dispose();
        }

        public TextureDictionary()
        {
            mTextures = new Dictionary<uint, Texture>();
        }
    }
}