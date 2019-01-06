using System.IO;

namespace MikuMikuLibrary.Archives
{
    public sealed class EntryStream<THandle> : Stream
    {
        private Stream mSource;

        public THandle Handle { get; }

        public override bool CanRead => mSource.CanRead;
        public override bool CanWrite => mSource.CanWrite;
        public override bool CanSeek => mSource.CanSeek;

        public override long Position
        {
            get => mSource.Position;
            set => mSource.Position = value;
        }

        public override long Length => mSource.Length;

        public override void Flush()
        {
            mSource.Flush();
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            return mSource.Read( buffer, 0, count );
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            return mSource.Seek( offset, origin );
        }

        public override void SetLength( long value )
        {
            mSource.SetLength( value );
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            mSource.Write( buffer, 0, count );
        }

        public EntryStream( THandle entry, Stream source )
        {
            Handle = entry;
            mSource = source;
        }
    }

    public enum EntryStreamMode
    {
        MemoryStream,
        OriginalStream,
    };
}
