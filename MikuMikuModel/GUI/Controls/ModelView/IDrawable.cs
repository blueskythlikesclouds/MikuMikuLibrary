using System;
using System.Collections.Generic;
using MikuMikuLibrary.Geometry;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    public struct DrawCommand
    {
        public GLMesh Mesh;
        public GLSubMesh SubMesh;
    }

    public interface IDrawable : IDisposable
    {
        void Submit( List<DrawCommand> opaqueCommands, List<DrawCommand> transparentCommands );
    }
}