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
        public Stream Stream;
        public bool OwnsStream;
        public bool IsCompressed;
        public bool IsEncrypted;
        public bool IsFutureTone;

        public Stream Open( Stream sourceStream )
        {
            if ( Stream != null )
                return Stream;

            Stream stream = sourceStream.CreateSubView( Position, Length );
            if ( Length == 0 )
                return stream;

            if ( IsEncrypted )
            {
                AesManaged aesManaged;
                if ( IsFutureTone )
                {
                    // The IV for the entry is right
                    // before the data.
                    byte[] iv = new byte[ 16 ];
                    stream.Read( iv, 0, 16 );

                    aesManaged = FarcArchive.CreateAesManagedForFT( iv );
                }

                else
                {
                    aesManaged = FarcArchive.CreateAesManaged();
                }

                var decryptor = aesManaged.CreateDecryptor();
                stream = new CryptoStream( stream, decryptor, CryptoStreamMode.Read );
            }

            if ( IsCompressed )
                stream = new GZipStream( stream, CompressionMode.Decompress );

            return stream;
        }

        public void Dispose()
        {
            if ( OwnsStream )
                Stream?.Dispose();
        }
    }
}
