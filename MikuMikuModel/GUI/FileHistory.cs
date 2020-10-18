using System;
using System.Collections.Generic;
using System.IO;
using MikuMikuModel.Resources;

namespace MikuMikuModel.GUI
{
    public static class FileHistory
    {
        private static readonly List<string> sFiles;

        public static int MaxCount
        {
            get => ValueCache.Get( nameof( FileHistory ) + nameof( MaxCount ), 10 );
            set => ValueCache.Set( nameof( FileHistory ) + nameof( MaxCount ), value );
        }

        public static IReadOnlyList<string> Files => sFiles;

        public static void Add( string filePath )
        {
            filePath = Path.GetFullPath( filePath );

            sFiles.RemoveAll( x => x.Equals( filePath, StringComparison.OrdinalIgnoreCase ) );
            sFiles.Add( filePath );

            if ( sFiles.Count > MaxCount )
                sFiles.RemoveRange( 0, sFiles.Count - MaxCount );

            ValueCache.Set( "FileHistory", string.Join( "|", sFiles ) );
        }

        static FileHistory()
        {
            sFiles = new List<string>( MaxCount );

            string filesData = ValueCache.Get<string>( "FileHistory" );

            if ( string.IsNullOrEmpty( filesData ) )
                return;

            var split = filesData.Split( new[] { '|' }, StringSplitOptions.RemoveEmptyEntries );
            
            foreach ( string filePath in split )
                sFiles.Add( Path.GetFullPath( filePath ) );
        }
    }
}