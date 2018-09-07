using System;
using System.IO;

namespace MikuMikuModel.FormatModules
{
    public interface IFormatModule
    {
        FormatModuleFlags Flags { get; }

        string Name { get; }
        string[] Extensions { get; }

        Type ModelType { get; }

        bool CanImport( Stream source, string fileName );
        bool CanImport( string fileName );

        object Import( Stream source, string fileName );
        object Import( string filePath );

        void Export( object obj, Stream destination, string fileName );
        void Export( object obj, string filePath );
    }

    [Flags]
    public enum FormatModuleFlags
    {
        Import = 1,
        Export = 2,
    }
}
