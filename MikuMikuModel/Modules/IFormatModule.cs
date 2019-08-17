using System;
using System.IO;

namespace MikuMikuModel.Modules
{
    public interface IFormatModule
    {
        FormatModuleFlags Flags { get; }
        Type ModelType { get; }

        string Name { get; }
        string[] Extensions { get; }

        object Import( string filePath );
        object Import( Stream source, string fileName = null );

        void Export( object model, string filePath );
        void Export( object model, Stream destination, string fileName = null );

        bool Match( string fileName );
        bool Match( byte[] buffer );
    }

    [Flags]
    public enum FormatModuleFlags
    {
        Import = 1 << 0,
        Export = 1 << 1
    }
}