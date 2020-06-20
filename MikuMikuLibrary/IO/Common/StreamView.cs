using System;
using System.IO;

namespace MikuMikuLibrary.IO.Common
{
    public sealed class StreamView : Stream
    {
        private readonly Stream mSourceStream;
        private readonly Stream mRootStream;
        private readonly Stream mSeekingStream;
        private readonly long mPosition;

        private long mSubPosition;
        private long mSeekingStreamPosition;
        private long mLength;

        private readonly bool mLeaveOpen;

        public override bool CanRead => mSourceStream.CanRead;
        public override bool CanSeek => mSourceStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => mLength;

        public override long Position
        {
            get => mSubPosition;
            set => mSubPosition = value;
        }

        public override void Flush() => 
            mSourceStream.Flush();

        public override long Seek( long offset, SeekOrigin origin )
        {
            switch ( origin )
            {
                case SeekOrigin.Begin:
                    mSubPosition = offset;
                    break;
                case SeekOrigin.Current:
                    mSubPosition += offset;
                    break;
                case SeekOrigin.End:
                    mSubPosition = mPosition + mLength - offset;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( origin ), origin, null );
            }

            return mSubPosition;
        }

        public override void SetLength( long value ) => 
            mLength = value;

        public override int Read( byte[] buffer, int offset, int count )
        {
            if ( mSubPosition >= mLength )
                return 0;

            if ( mSubPosition + count > mLength )
                count = ( int ) ( mLength - mSubPosition );

            long previousPosition = mSeekingStream.Position;
            mSeekingStream.Seek( mSeekingStreamPosition, SeekOrigin.Begin );

            int result = mSourceStream.Read( buffer, offset, count );
            mSubPosition += count;
            mSeekingStreamPosition = mSeekingStream.Position;

            mSeekingStream.Seek( previousPosition, SeekOrigin.Begin );

            return result;
        }
          
        public override void Write( byte[] buffer, int offset, int count ) => 
            throw new NotSupportedException();

        protected override void Dispose( bool disposing )
        {
            if ( disposing && !mLeaveOpen )
                mSourceStream.Close();

            base.Dispose( disposing );
        }

        public StreamView( Stream sourceStream, Stream rootStream, long position, long length, bool leaveOpen = false )
        {
            mSourceStream = sourceStream;
            mRootStream = rootStream;
            mSeekingStream = rootStream ?? sourceStream;
            mPosition = position;
            mSubPosition = 0;
            mSeekingStreamPosition = position;
            mLength = length;
            mLeaveOpen = leaveOpen;
        }

        public StreamView( Stream sourceStream, long position, long length, bool leaveOpen = false )
            : this( sourceStream, null, position, length, leaveOpen )
        {

        }
    }
}