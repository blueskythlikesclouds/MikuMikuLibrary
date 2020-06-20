using System.IO;

namespace MikuMikuLibrary.Archives
{
    public sealed class EntryStream<THandle> : Stream
    {
        public Stream Source { get; }
        public THandle Handle { get; }

        public override bool CanRead => Source.CanRead;
        public override bool CanWrite => Source.CanWrite;
        public override bool CanSeek => Source.CanSeek;

        public override long Position
        {
            get => Source.Position;
            set => Source.Position = value;
        }

        public override long Length => Source.Length;

        public override void Flush() => Source.Flush();

        public override int Read( byte[] buffer, int offset, int count ) => 
            Source.Read( buffer, 0, count );

        public override long Seek( long offset, SeekOrigin origin ) => 
            Source.Seek( offset, origin );

        public override void SetLength( long value ) => 
            Source.SetLength( value );

        public override void Write( byte[] buffer, int offset, int count ) => 
            Source.Write( buffer, 0, count );

        public EntryStream( THandle entry, Stream source )
        {
            Handle = entry;
            Source = source;
        }
    }

    public enum EntryStreamMode
    {
        MemoryStream,
        OriginalStream
    }
}