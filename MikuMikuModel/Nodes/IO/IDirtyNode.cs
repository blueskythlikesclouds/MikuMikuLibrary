using System;
using System.IO;

namespace MikuMikuModel.Nodes.IO
{
    public interface IDirtyNode : INode
    {
        bool IsDirty { get; }
        event EventHandler DirtyStateChanged;
        Stream GetStream();
    }
}