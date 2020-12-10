using System;
using MikuMikuLibrary.Geometry;
using MikuMikuLibrary.Rendering.IBLs;

namespace MikuMikuLibrary.Rendering.Scenes
{
    public sealed class Scene : IDisposable
    {
        public BoundingSphere BoundingSphere { get; set; }

        public Node CharacterRoot { get; set; }
        public Node StageRoot { get; set; }
        public TextureDictionary Textures { get; }

        public void Dispose()
        {
            CharacterRoot?.Dispose();
            StageRoot?.Dispose();
            Textures.Dispose();
        }

        public Scene()
        {
            Textures = new TextureDictionary();
        }
    }
}
