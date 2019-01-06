//===============================================================//
// Taken and modified from: https://github.com/TGEnigma/Amicitia //
//===============================================================//

using System;
using System.IO;

namespace MikuMikuLibrary.IO.Common
{
    public class StreamView : Stream
    {
        private readonly Stream mStream;
        private long mSourcePositionCopy;
        private readonly long mStreamPosition;
        private long mPosition;
        private long mLength;
        private readonly long mMaxLength;

        public StreamView( Stream source, long position, long length )
        {
            if ( source == null )
                throw new ArgumentNullException( nameof( source ) );

            if ( position < 0 || position >= source.Length || ( position + length ) > source.Length )
                throw new ArgumentOutOfRangeException( nameof( position ) );

            if ( length <= 0 )
                throw new ArgumentOutOfRangeException( nameof( length ) );

            mStream = source;
            mStreamPosition = position;
            mPosition = 0;
            mMaxLength = mLength = length;
        }

        public override void Flush()
        {
            mStream.Flush();
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            switch ( origin )
            {
                case SeekOrigin.Begin:
                    {
                        if ( offset > mLength || offset > mStream.Length )
                            throw new ArgumentOutOfRangeException( nameof( offset ) );

                        mPosition = offset;
                    }
                    break;
                case SeekOrigin.Current:
                    {
                        if ( ( mPosition + offset ) > mLength || ( mPosition + offset ) > mStream.Length )
                            throw new ArgumentOutOfRangeException( nameof( offset ) );

                        mPosition += offset;
                    }
                    break;
                case SeekOrigin.End:
                    {
                        if ( offset < mLength || offset > 0 )
                            throw new ArgumentOutOfRangeException( nameof( offset ) );

                        mPosition = ( mStreamPosition + mLength ) - offset;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException( nameof( origin ) );
            }

            return mPosition;
        }

        public override void SetLength( long value )
        {
            if ( value < 0 )
                throw new ArgumentOutOfRangeException( nameof( value ) );

            if ( value > mStream.Length )
                throw new ArgumentOutOfRangeException( nameof( value ) );

            mLength = value;
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            if ( EndOfStream )
                return 0;

            if ( ( mPosition + count ) > mLength )
                count = ( int )( mLength - mPosition );

            SavePosition();
            SetUnderlyingStreamPosition();
            int result = mStream.Read( buffer, offset, count );
            mPosition += count;
            RestorePosition();

            return result;
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            if ( EndOfStream )
                throw new IOException( "Attempted to write past end of stream" );

            if ( ( mPosition + count ) > mLength )
                throw new IOException( "Attempted to write past end of stream" );

            SavePosition();
            SetUnderlyingStreamPosition();
            mStream.Write( buffer, offset, count );
            mPosition += count;
            RestorePosition();
        }

        public override bool CanRead
        {
            get { return mStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return mStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return mStream.CanWrite; }
        }

        public override long Length
        {
            get { return mLength; }
        }

        public override long Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public bool EndOfStream => mPosition == mMaxLength;

        public override int ReadByte()
        {
            if ( EndOfStream )
                return -1;

            SavePosition();
            SetUnderlyingStreamPosition();
            int value = mStream.ReadByte();
            mPosition++;
            RestorePosition();

            return value;
        }

        public override void WriteByte( byte value )
        {
            if ( EndOfStream )
                throw new IOException( "Attempted to write past end of stream" );

            SavePosition();
            SetUnderlyingStreamPosition();
            mStream.WriteByte( value );
            mPosition++;
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
            mSourcePositionCopy = mStream.Position;
        }

        protected void SetUnderlyingStreamPosition()
        {
            mStream.Position = ( mStreamPosition + mPosition );
        }

        protected void RestorePosition()
        {
            mStream.Position = mSourcePositionCopy;
        }
    }
}
