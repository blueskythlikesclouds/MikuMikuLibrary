//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;
using System.IO;

namespace MikuMikuLibrary.IO
{
    public class StreamView : Stream
    {
        private readonly Stream stream;
        private long sourcePositionCopy;
        private readonly long streamPosition;
        private long position;
        private long length;
        private readonly long maxLength;

        public StreamView( Stream source, long position, long length )
        {
            if ( source == null )
                throw new ArgumentNullException( nameof( source ) );

            if ( position < 0 || position >= source.Length || ( position + length ) > source.Length )
                throw new ArgumentOutOfRangeException( nameof( position ) );

            if ( length <= 0 )
                throw new ArgumentOutOfRangeException( nameof( length ) );

            stream = source;
            streamPosition = position;
            this.position = 0;
            maxLength = this.length = length;
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            switch ( origin )
            {
                case SeekOrigin.Begin:
                    {
                        if ( offset > length || offset > stream.Length )
                            throw new ArgumentOutOfRangeException( nameof( offset ) );

                        position = offset;
                    }
                    break;
                case SeekOrigin.Current:
                    {
                        if ( ( position + offset ) > length || ( position + offset ) > stream.Length )
                            throw new ArgumentOutOfRangeException( nameof( offset ) );

                        position += offset;
                    }
                    break;
                case SeekOrigin.End:
                    {
                        if ( offset < length || offset > 0 )
                            throw new ArgumentOutOfRangeException( nameof( offset ) );

                        position = ( streamPosition + length ) - offset;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( origin ) );
            }

            return position;
        }

        public override void SetLength( long value )
        {
            if ( value < 0 )
                throw new ArgumentOutOfRangeException( nameof( value ) );

            if ( value > stream.Length )
                throw new ArgumentOutOfRangeException( nameof( value ) );

            length = value;
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            if ( EndOfStream )
                return 0;

            if ( ( position + count ) > length )
                count = ( int )( length - position );

            SavePosition();
            SetUnderlyingStreamPosition();
            int result = stream.Read( buffer, offset, count );
            position += count;
            RestorePosition();

            return result;
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            if ( EndOfStream )
                throw new IOException( "Attempted to write past end of stream" );

            if ( ( position + count ) > length )
                throw new IOException( "Attempted to write past end of stream" );

            SavePosition();
            SetUnderlyingStreamPosition();
            stream.Write( buffer, offset, count );
            position += count;
            RestorePosition();
        }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override long Length
        {
            get { return length; }
        }

        public override long Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool EndOfStream => position == maxLength;

        public override int ReadByte()
        {
            if ( EndOfStream )
                return -1;

            SavePosition();
            SetUnderlyingStreamPosition();
            int value = stream.ReadByte();
            position++;
            RestorePosition();

            return value;
        }

        public override void WriteByte( byte value )
        {
            if ( EndOfStream )
                throw new IOException( "Attempted to write past end of stream" );

            SavePosition();
            SetUnderlyingStreamPosition();
            stream.WriteByte( value );
            position++;
            RestorePosition();
        }

        /*
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            var value = mSourceStream.BeginRead(buffer, offset, count, callback, state);
            RestoreSourcePosition();

            return value;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            var value = mSourceStream.BeginWrite(buffer, offset, count, callback, state);
            RestoreSourcePosition();

            return value;
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            var value = mSourceStream.CopyToAsync(destination, bufferSize, cancellationToken);
            RestoreSourcePosition();

            return value;
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            var value = mSourceStream.EndRead(asyncResult);
            RestoreSourcePosition();

            return value;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            mSourceStream.EndWrite(asyncResult);
            RestoreSourcePosition();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            var value = mSourceStream.WriteAsync(buffer, offset, count, cancellationToken);
            RestoreSourcePosition();

            return value;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            SaveSourcePosition();
            SetSourcePositionForSubstream();
            var value = mSourceStream.ReadAsync(buffer, offset, count, cancellationToken);
            RestoreSourcePosition();

            return value;
        }
        */
        protected void SavePosition()
        {
            sourcePositionCopy = stream.Position;
        }

        protected void SetUnderlyingStreamPosition()
        {
            stream.Position = ( streamPosition + position );
        }

        protected void RestorePosition()
        {
            stream.Position = sourcePositionCopy;
        }
    }
}
