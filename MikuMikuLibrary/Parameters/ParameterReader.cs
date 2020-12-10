using System.Globalization;
using System.Numerics;
using MikuMikuLibrary.Extensions;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Parameters
{
    public class ParameterReader
    {
        private EndianBinaryReader mReader;

        private bool mEnd;

        private string mLine;
        private int mLinePosition;

        public string HeadToken { get; private set; }

        public bool Read()
        {
            if ( mEnd )
                return false;

            do
            {
                mLine = mReader.ReadLine();
            } while ( string.IsNullOrEmpty( mLine ) && mReader.Position < mReader.Length );

            if ( string.IsNullOrEmpty( mLine ) )
            {
                mEnd = true;
                return false;
            }

            mLine = mLine.Trim();
            mLinePosition = -1;

            if ( ( HeadToken = ReadToken() ) != "EOF" )
                return true;

            mEnd = true;
            return false;
        }

        public string ReadToken()
        {
            int previousPosition = mLinePosition;

            do
            {
                mLinePosition = mLine.IndexOf( ' ', mLinePosition + 1 );
            } while ( mLinePosition >= 0 && mLinePosition < mLine.Length - 1 && mLine[ mLinePosition + 1 ] == ' ' );

            if ( mLinePosition == -1 )
                mLinePosition = mLine.Length;

            return mLine.Substring( previousPosition + 1, mLinePosition - previousPosition - 1 );
        }

        public bool ReadBoolean()
        {
            return ReadUInt32() != 0;
        }

        public int ReadInt32()
        {
            string token = ReadToken();
            return int.TryParse( token, out int value ) ? value : 0;
        }       
        
        public uint ReadUInt32()
        {
            string token = ReadToken();
            return uint.TryParse( token, out uint value ) ? value : 0u;
        }

        public float ReadSingle()
        {
            string token = ReadToken();
            return float.TryParse( token, NumberStyles.Any, CultureInfo.InvariantCulture, out float value ) ? value : 0.0f;
        }

        public Vector2 ReadVector2()
        {
            return new Vector2( ReadSingle(), ReadSingle() );
        }       
        
        public Vector3 ReadVector3()
        {
            return new Vector3( ReadSingle(), ReadSingle(), ReadSingle() );
        }       
        
        public Vector4 ReadVector4()
        {
            return new Vector4( ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle() );
        }

        public ParameterReader( EndianBinaryReader reader )
        {
            mReader = reader;
        }
    }
}