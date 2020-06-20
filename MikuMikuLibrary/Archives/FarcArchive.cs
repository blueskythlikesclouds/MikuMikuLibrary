using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.IO.Common;
using MikuMikuLibrary.IO.Sections;

namespace MikuMikuLibrary.Archives
{
    public class FarcArchive : BinaryFile, IArchive<string>
    {
        private readonly Dictionary<string, Entry> mEntries;
        private int mAlignment;

        public int Alignment
        {
            get => mAlignment;
            set => mAlignment = ( value & ( value - 1 ) ) != 0 ? AlignmentHelper.AlignToNextPowerOfTwo( value ) : value;
        }

        public bool IsCompressed { get; set; }

        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.UsesSourceStream;

        public override Endianness Endianness => Endianness.Big;

        public bool CanAdd => true;
        public bool CanRemove => true;

        public IEnumerable<string> Entries => mEntries.Keys;

        public void Add( string handle, Stream source, bool leaveOpen, ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError )
        {
            if ( mEntries.TryGetValue( handle, out var entry ) )
            {
                switch ( conflictPolicy )
                {
                    case ConflictPolicy.RaiseError:
                        throw new InvalidOperationException( $"Entry already exists ({handle})" );

                    case ConflictPolicy.Replace:
                        if ( source is EntryStream<string> entryStream && entryStream.Source == source )
                            break;

                        entry.Dispose();
                        entry.Stream = source;
                        entry.OwnsStream = !leaveOpen;
                        break;

                    case ConflictPolicy.Ignore:
                        break;
                }
            }

            else
            {
                mEntries.Add( handle, new Entry
                {
                    Handle = handle,
                    Stream = source,
                    OwnsStream = !leaveOpen
                } );
            }
        }

        public void Add( string handle, string fileName, ConflictPolicy conflictPolicy = ConflictPolicy.RaiseError ) => 
            Add( handle, File.OpenRead( fileName ), false, conflictPolicy );

        public void Remove( string handle )
        {
            if ( !mEntries.TryGetValue( handle, out var entry ) )
                return;

            entry.Dispose();
            mEntries.Remove( handle );
        }

        public EntryStream<string> Open( string handle, EntryStreamMode mode )
        {
            var entry = mEntries[ handle ];
            var entryStream = entry.Open( mStream );

            if ( mode != EntryStreamMode.MemoryStream )
                return new EntryStream<string>( entry.Handle, entryStream );

            var temp = entryStream;
            entryStream = new MemoryStream();
            temp.CopyTo( entryStream );
            entryStream.Position = 0;
            temp.Close();

            return new EntryStream<string>( entry.Handle, entryStream );
        }

        public void Clear()
        {
            foreach ( var entry in mEntries.Values )
                entry.Dispose();

            mEntries.Clear();
        }

        public bool Contains( string handle ) => 
            mEntries.ContainsKey( handle );

        public IEnumerator<string> GetEnumerator() => 
            mEntries.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            mEntries.Keys.GetEnumerator();

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            string signature = reader.ReadString( StringBinaryFormat.FixedLength, 4 );
            if ( signature != "FARC" && signature != "FArC" && signature != "FArc" )
                throw new InvalidDataException( "Invalid signature (excepted FARC/FArC/FArc)" );

            uint headerSize = reader.ReadUInt32() + 0x08;
            var originalStream = reader.BaseStream;

            if ( signature == "FARC" )
            {
                int flags = reader.ReadInt32();
                bool isCompressed = ( flags & 2 ) != 0;
                bool isEncrypted = ( flags & 4 ) != 0;
                int padding = reader.ReadInt32();
                mAlignment = reader.ReadInt32();

                IsCompressed = isCompressed;

                // Hacky way of checking Future Tone.
                // There's a very low chance this isn't going to work, though.
                Format = isEncrypted && ( mAlignment & ( mAlignment - 1 ) ) != 0 ? BinaryFormat.FT : BinaryFormat.DT;

                if ( Format == BinaryFormat.FT )
                {
                    reader.SeekBegin( 0x10 );
                    var iv = reader.ReadBytes( 0x10 );
                    var aesManaged = CreateAesManagedForFT( iv );
                    var decryptor = aesManaged.CreateDecryptor();
                    var cryptoStream = new CryptoStream( reader.BaseStream, decryptor, CryptoStreamMode.Read );
                    reader = new EndianBinaryReader( cryptoStream, Encoding.UTF8, Endianness.Big );
                    mAlignment = reader.ReadInt32();
                }

                Format = reader.ReadInt32() == 1 ? BinaryFormat.FT : BinaryFormat.DT;

                int entryCount = reader.ReadInt32();
                if ( Format == BinaryFormat.FT )
                    padding = reader.ReadInt32(); // No SeekCurrent!! CryptoStream does not support it.

                while ( originalStream.Position < headerSize )
                {
                    string name = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint offset = reader.ReadUInt32();
                    uint compressedSize = reader.ReadUInt32();
                    uint uncompressedSize = reader.ReadUInt32();

                    if ( Format == BinaryFormat.FT )
                    {
                        flags = reader.ReadInt32();
                        isCompressed = ( flags & 2 ) != 0;
                        isEncrypted = ( flags & 4 ) != 0;
                    }

                    long fixedSize = 0;

                    if ( isEncrypted )
                        fixedSize = AlignmentHelper.Align( isCompressed ? compressedSize : uncompressedSize, 16 );

                    else if ( isCompressed )
                        fixedSize = compressedSize;

                    else
                        fixedSize = uncompressedSize;

                    fixedSize = Math.Min( fixedSize, originalStream.Length - offset );

                    mEntries.Add( name, new Entry
                    {
                        Handle = name,
                        Position = offset,
                        UnpackedLength = uncompressedSize,
                        CompressedLength = Math.Min( compressedSize, originalStream.Length - offset ),
                        Length = fixedSize,
                        IsCompressed = isCompressed && compressedSize != uncompressedSize,
                        IsEncrypted = isEncrypted,
                        IsFutureTone = Format == BinaryFormat.FT
                    } );

                    // There's sometimes extra padding on some FARC files which
                    // causes this loop to throw an exception. This check fixes it.
                    if ( Format == BinaryFormat.FT && --entryCount == 0 )
                        break;
                }
            }

            else if ( signature == "FArC" )
            {
                mAlignment = reader.ReadInt32();

                while ( reader.Position < headerSize )
                {
                    string name = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint offset = reader.ReadUInt32();
                    uint compressedSize = reader.ReadUInt32();
                    uint uncompressedSize = reader.ReadUInt32();

                    long fixedSize = Math.Min( compressedSize, reader.Length - offset );

                    mEntries.Add( name, new Entry
                    {
                        Handle = name,
                        Position = offset,
                        UnpackedLength = uncompressedSize,
                        CompressedLength = fixedSize,
                        Length = fixedSize,
                        IsCompressed = compressedSize != uncompressedSize
                    } );
                }

                IsCompressed = true;
            }

            else if ( signature == "FArc" )
            {
                mAlignment = reader.ReadInt32();

                while ( reader.Position < headerSize )
                {
                    string name = reader.ReadString( StringBinaryFormat.NullTerminated );
                    uint offset = reader.ReadUInt32();
                    uint size = reader.ReadUInt32();

                    long fixedSize = Math.Min( size, reader.Length - offset );

                    mEntries.Add( name, new Entry
                    {
                        Handle = name,
                        Position = offset,
                        UnpackedLength = fixedSize,
                        Length = fixedSize
                    } );
                }

                IsCompressed = false;
            }
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( IsCompressed ? "FArC" : "FArc", StringBinaryFormat.FixedLength, 4 );
            writer.ScheduleWriteOffset( OffsetMode.Size, () =>
            {
                writer.Write( mAlignment );

                foreach ( var entry in mEntries.Values.OrderBy( x => x.Handle ) )
                {
                    writer.Write( entry.Handle, StringBinaryFormat.NullTerminated );
                    writer.ScheduleWriteOffset( OffsetMode.OffsetAndSize, () =>
                    {
                        writer.Align( mAlignment, 0x78 );

                        long position = writer.Position;

                        entry.CopyTo( writer.BaseStream, mStream, IsCompressed );

                        entry.Position = position;
                        entry.Length = writer.Position - position;

                        entry.IsCompressed = IsCompressed;

                        if ( IsCompressed )
                        {
                            entry.CompressedLength = entry.Length;
                        }
                        else
                        {
                            entry.CompressedLength = -1;
                            entry.UnpackedLength = entry.Length;
                        }

                        entry.IsEncrypted = false;
                        entry.IsFutureTone = false;

                        if ( entry.Stream != null )
                        {
                            entry.UnpackedLength = entry.Stream.Length;

                            if ( entry.OwnsStream )
                                entry.Stream.Dispose();

                            entry.Stream = null;
                            entry.OwnsStream = false;
                        }

                        return position;
                    } );
                    if ( IsCompressed )
                        writer.Write( ( uint ) ( entry.Stream?.Length ?? entry.UnpackedLength ) );
                }
            } );

            writer.PerformScheduledWrites();
            writer.Align( mAlignment, 0x78 );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                foreach ( var entry in mEntries.Values )
                    entry.Dispose();

            base.Dispose( disposing );
        }

        public static AesManaged CreateAesManaged()
        {
            return new AesManaged
            {
                KeySize = 128,
                Key = new byte[]
                {
                    // project_diva.bin
                    0x70, 0x72, 0x6F, 0x6A, 0x65, 0x63, 0x74, 0x5F, 0x64, 0x69, 0x76, 0x61, 0x2E, 0x62, 0x69, 0x6E
                },
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros,
                IV = new byte[ 16 ]
            };
        }

        public static AesManaged CreateAesManagedForFT( byte[] iv = null )
        {
            return new AesManaged
            {
                KeySize = 128,
                Key = new byte[]
                {
                    0x13, 0x72, 0xD5, 0x7B, 0x6E, 0x9E, 0x31, 0xEB, 0xA2, 0x39, 0xB8, 0x3C, 0x15, 0x57, 0xC6, 0xBB
                },
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros,
                IV = iv ?? new byte[ 16 ]
            };
        }

        public FarcArchive()
        {
            mEntries = new Dictionary<string, Entry>( StringComparer.OrdinalIgnoreCase );
            mAlignment = 0x10;
        }

        internal class Entry : IDisposable
        {
            public string Handle;
            public long Position;
            public long Length;
            public long CompressedLength;
            public long UnpackedLength;
            public Stream Stream;
            public bool OwnsStream;

            public bool IsCompressed;
            public bool IsEncrypted;
            public bool IsFutureTone;

            public void Dispose()
            {
                if ( OwnsStream )
                    Stream?.Dispose();
            }

            public Stream Open( Stream source )
            {
                if ( Stream != null )
                    return Stream;

                if ( Length == 0 || UnpackedLength == 0 )
                    return Stream.Null;

                var stream = source;

                stream.Seek( Position, SeekOrigin.Begin );

                if ( IsEncrypted )
                    stream = GetDecryptingStream( stream, true );

                if ( IsCompressed )
                    stream = GetDecompressingStream( stream, stream == source );

                long position = Position;

                if ( IsFutureTone && IsEncrypted )
                    position += 16;

                return new StreamView( stream, source, position, UnpackedLength, stream == source );
            }

            internal void CopyTo( Stream destination, Stream source, bool compress )
            {
                if ( Stream != null )
                {
                    if ( Stream.Length == 0 )
                        return;

                    Stream.Seek( 0, SeekOrigin.Begin );
                    CopyCompressedIf( compress, Stream );
                    return;
                }

                if ( Length == 0 || UnpackedLength == 0 )
                    return;

                source.Seek( Position, SeekOrigin.Begin );

                Stream sourceStream;

                if ( IsEncrypted )
                {
                    var streamView = new StreamView( source, Position, Length, true );
                    sourceStream = new StreamView( GetDecryptingStream( streamView ), streamView, 0, IsCompressed ? CompressedLength : UnpackedLength );
                }

                else if ( IsCompressed )
                {
                    sourceStream = new StreamView( source, Position, CompressedLength, true );
                }

                else
                {
                    sourceStream = new StreamView( source, Position, UnpackedLength, true );
                }

                if ( IsCompressed && !compress )
                    sourceStream = new StreamView( GetDecompressingStream( sourceStream, sourceStream == source ), sourceStream, 0, UnpackedLength );

                CopyCompressedIf( !IsCompressed && compress, sourceStream );

                sourceStream.Close();

                void CopyCompressedIf( bool condition, Stream stream )
                {
                    if ( condition )
                    {
                        using ( var gzipStream = new GZipStream( destination, CompressionMode.Compress, true ) )
                        {
                            stream.CopyTo( gzipStream );
                        }
                    }

                    else
                    {
                        stream.CopyTo( destination );
                    }
                }
            }

            internal CryptoStream GetDecryptingStream( Stream stream, bool leaveOpen = false )
            {
                AesManaged aesManaged;
                if ( IsFutureTone )
                {
                    var iv = new byte[ 16 ];
                    stream.Read( iv, 0, 16 );

                    aesManaged = CreateAesManagedForFT( iv );
                }

                else
                {
                    aesManaged = CreateAesManaged();
                }

                var decryptor = aesManaged.CreateDecryptor();
                return new NonClosingCryptoStream( stream, decryptor, CryptoStreamMode.Read, leaveOpen );
            }

            internal static GZipStream GetDecompressingStream( Stream stream, bool leaveOpen = false ) => 
                new GZipStream( stream, CompressionMode.Decompress, leaveOpen );

            private class NonClosingCryptoStream : CryptoStream
            {
                private readonly bool mLeaveOpen;

                protected override void Dispose( bool disposing )
                {
                    if ( !HasFlushedFinalBlock )
                        FlushFinalBlock();

                    base.Dispose( !mLeaveOpen );
                }

                public NonClosingCryptoStream( Stream stream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen )
                    : base( stream, transform, mode )
                {
                    mLeaveOpen = leaveOpen;
                }
            }
        }
    }
}