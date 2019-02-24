using MikuMikuLibrary.IO.Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace MikuMikuLibrary.Archives.Farc
{
    internal class InternalEntry : IDisposable
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
        
        public Stream Open( Stream source )
        {
            if ( Stream != null )
                return Stream;

            if ( Length == 0 || UnpackedLength == 0 )
                return Stream.Null;
                
            Stream stream = source;
            stream.Seek( Position, SeekOrigin.Begin );
            
            if ( IsEncrypted )
                stream = GetDecryptingStream( stream, true );

            if ( IsCompressed )
                stream = GetDecompressingStream( stream, stream == source );

            return stream.CreateSubView( Position, UnpackedLength, stream == source );
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
                sourceStream = source.CreateSubView( Position, Length );
                sourceStream = GetDecryptingStream( sourceStream );
                sourceStream = sourceStream.CreateSubView( 0, IsCompressed ? CompressedLength : UnpackedLength, false );
            }
            else if ( IsCompressed )
            {
                sourceStream = source.CreateSubView( Position, CompressedLength );
            }
            else
            {
                sourceStream = source.CreateSubView( Position, UnpackedLength );
            }
            
            if ( IsCompressed && !compress )
            {
                sourceStream = GetDecompressingStream( sourceStream );
                sourceStream = sourceStream.CreateSubView( 0, UnpackedLength, false );
            }
            
            CopyCompressedIf( !IsCompressed && compress, sourceStream );
            
            sourceStream.Close();
            
            void CopyCompressedIf( bool condition, Stream stream )
            {
                if ( condition )
                {
                    using ( var gzipStream = new GZipStream( destination, CompressionMode.Compress, true ) )
                        stream.CopyTo( gzipStream );
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
                byte[] iv = new byte[ 16 ];
                stream.Read( iv, 0, 16 );

                aesManaged = FarcArchive.CreateAesManagedForFT( iv );
            }

            else
            {
                aesManaged = FarcArchive.CreateAesManaged();
            }

            var decryptor = aesManaged.CreateDecryptor();
            return new NonClosingCryptoStream( stream, decryptor, CryptoStreamMode.Read, leaveOpen );
        }
        
        internal static GZipStream GetDecompressingStream( Stream stream, bool leaveOpen = false ) =>
            new GZipStream( stream, CompressionMode.Decompress, leaveOpen );

        public void Dispose()
        {
            if ( OwnsStream )
                Stream?.Dispose();
        }
        
        private class NonClosingCryptoStream : CryptoStream
        {
            private bool mLeaveOpen;
        
            public NonClosingCryptoStream( Stream stream, ICryptoTransform transform, CryptoStreamMode mode, bool leaveOpen )
                : base( stream, transform, mode )
            {
                mLeaveOpen = leaveOpen;
            }

            protected override void Dispose( bool disposing )
            {
                if ( !HasFlushedFinalBlock )
                    FlushFinalBlock();

                base.Dispose( !mLeaveOpen );
            }
        }
    }
}
