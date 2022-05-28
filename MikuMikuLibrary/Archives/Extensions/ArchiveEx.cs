using System;
using System.IO;
using MikuMikuLibrary.IO;

namespace MikuMikuLibrary.Archives.Extensions
{
    public static class ArchiveEx
    {
        public static T Open<T>( this IArchive archive, string fileName, Func<Stream, T> factory = null )
            where T : IBinaryFile, new()
        {
            return ArchiveUtility.Open( archive, fileName, factory );
        }

        public static void Add( this IArchive archive, string fileName, IBinaryFile binaryFile,
            ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError, Action<Stream> saver = null )
        {
            ArchiveUtility.Add( archive, fileName, binaryFile, conflictPolicy, saver );
        }
    }
}