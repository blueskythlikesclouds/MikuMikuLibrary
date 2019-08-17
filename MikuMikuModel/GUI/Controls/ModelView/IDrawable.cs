using System;

namespace MikuMikuModel.GUI.Controls.ModelView
{
    internal interface IDrawable : IDisposable
    {
        void Draw( GLShaderProgram shaderProgram );
    }
}