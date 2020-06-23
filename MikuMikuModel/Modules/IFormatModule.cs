using System;
using System.Collections.Generic;
using System.IO;

namespace MikuMikuModel.Modules
{
    [Flags]
    public enum FormatExtensionFlags
    {
        Import = 1 << 0,
        Export = 1 << 1
    }

    public class FormatExtension
    {
        public string Name { get; }
        public string Extension { get; }
        public FormatExtensionFlags Flags { get; }

        public FormatExtension( string name, string extension, FormatExtensionFlags flags )
        {
            Name = name;
            Extension = extension;
            Flags = flags;
        }
    }

    public interface IFormatModule
    {
        Type ModelType { get; }
        IReadOnlyList<FormatExtension> Extensions { get; }

        object Import( string filePath );
        object Import( Stream source, string fileName = null );

        void Export( object model, string filePath );
        void Export( object model, Stream destination, string fileName = null );

        bool Match( string fileName );
        bool Match( byte[] buffer );
    }
}