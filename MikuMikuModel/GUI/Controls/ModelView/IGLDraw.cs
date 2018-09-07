using System;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    internal interface IGLDraw : IDisposable
    {
        void Draw( GLShaderProgram shaderProgram );
    }
}
