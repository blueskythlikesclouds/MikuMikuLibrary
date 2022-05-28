using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Archives.CriMw
{
    // This is made SPECIFICALLY for MM+ CPKs.
    // Do not expect it to work for anything else.
    
    public class CpkArchive : BinaryFile, IArchive<string>
    {
        private readonly Dictionary<string, Entry> mEntries =
            new Dictionary<string, Entry>( StringComparer.OrdinalIgnoreCase );

        public override BinaryFileFlags Flags => BinaryFileFlags.Load | BinaryFileFlags.UsesSourceStream;
        public override Endianness Endianness => Endianness.Little;

        public bool CanAdd => false;
        public bool CanRemove => false;
        public IEnumerable<string> Entries => mEntries.Keys;
        public bool Contains(string handle) => mEntries.ContainsKey(handle);
        public void Add(string handle, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy) => throw new NotSupportedException();
        public void Add(string handle, string fileName, ConflictPolicy conflictPolicy) => throw new NotSupportedException();
        public void Remove(string handle) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();

        public EntryStream<string> Open( string handle, EntryStreamMode mode )
        {
            var entry = mEntries[ handle ];
            Stream stream = entry.Open( mStream );

            if ( mode == EntryStreamMode.MemoryStream )
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo( memoryStream );
                stream.Close();
                stream = memoryStream;
                stream.Seek( 0, SeekOrigin.Begin );
            }

            return new EntryStream<string>( entry.Path, stream );
        }

        // Very fragile, will break if you give it a path with more than one separator.
        public void Extract( string dstDirectoryPath, string dirPath = null )
        {
            if ( !string.IsNullOrEmpty( dirPath ) )
            {
                dirPath = dirPath.Replace( '\\', '/' );
                if ( !dirPath.EndsWith( "/" ) )
                    dirPath += "/";
            }

            foreach ( var entry in mEntries.Values )
            {
                string path = entry.Path;

                if ( !string.IsNullOrEmpty( dirPath ) )
                {
                    if ( !entry.Path.StartsWith( dirPath, StringComparison.OrdinalIgnoreCase ) )
                        continue;

                    path = path.Substring( dirPath.Length );
                }
                
                string filePath = Path.Combine( dstDirectoryPath, path );
                Directory.CreateDirectory( Path.GetDirectoryName( filePath ) );

                using ( var dstStream = File.Create( filePath ) )
                using ( var entryStream = entry.Open( mStream ) )
                    entryStream.CopyTo( dstStream );
            }
        }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            var cpk = UtfTable.ReadFromChunk( reader, "CPK " );

            long tocOffset = cpk[ 0 ].Get<long>( "TocOffset" );
            long contentOffset = cpk[ 0 ].Get<long>( "ContentOffset" );
            int alignment = cpk[ 0 ].Get<int>( "Align" );

            reader.ReadAtOffset( tocOffset, () =>
            {
                var toc = UtfTable.ReadFromChunk( reader, "TOC " );

                foreach ( var row in toc )
                {
                    var entry = new Entry
                    {
                        Path = Path.Combine( row.Get<string>( "DirName" ), row.Get<string>( "FileName" ) ).Replace( '\\', '/' ),
                        Position = row.Get<long>( "FileOffset" ),
                        Length = row.Get<long>( "FileSize" )
                    };

                    if ( contentOffset < tocOffset )
                        entry.Position += contentOffset;
                    else
                        entry.Position += tocOffset;

                    entry.Position = AlignmentHelper.Align( entry.Position, alignment );
                    
                    mEntries[ entry.Path ] = entry;
                }
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            throw new NotSupportedException();
        }

        public IEnumerator<string> GetEnumerator() => mEntries.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Entry
        {
            public string Path;
            public long Position;
            public long Length;

            public Stream Open( Stream stream )
            {
                var aesManaged = new AesManaged
                {
                    KeySize = 256,
                    Key = new byte[]
                    {
                        0xCF, 0x53, 0xBF, 0x9C, 0x37, 0x67, 0xAF, 0xB0, 0x35, 0x54, 0x4E, 0xB9, 0x96, 0xAA, 0x24, 0x39,
                        0x26, 0x5D, 0x40, 0x89, 0x7E, 0xD0, 0x1C, 0x3A, 0x6B, 0xA6, 0x5D, 0xD5, 0xFD, 0x6C, 0x19, 0xA3
                    },
                    BlockSize = 128,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    IV = new byte[]
                    {
                        0xC2, 0x55, 0xFD, 0x73, 0xD8, 0x30, 0xFA, 0xEF, 0xD5, 0x32, 0x08, 0x54, 0xA2, 0x26, 0x44, 0x14
                    }
                };

                return new CryptoStream( new StreamView( stream, Position, Length, true ),
                    aesManaged.CreateDecryptor(), CryptoStreamMode.Read );
            }
        }
    }
}