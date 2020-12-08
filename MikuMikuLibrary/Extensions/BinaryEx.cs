using System.Text;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Extensions
{
    public static class BinaryEx
    {
        public static string ReadLine( this EndianBinaryReader reader )
        {
            var stringBuilder = new StringBuilder();

            char c;
            while ( reader.BaseStream.Position < reader.BaseStream.Length && ( c = reader.ReadChar() ) != '\n' && c != '\r' )
                stringBuilder.Append( c );

            return stringBuilder.ToString();
        }

        public static void WriteLine( this EndianBinaryWriter writer, string value )
        {
            writer.Write( writer.Encoding.GetBytes( value ) );
            writer.Write( '\n' );
        }       
        
        public static void WriteLine( this EndianBinaryWriter writer, string format, params object[] args )
        {
            WriteLine( writer, string.Format( format, args ) );
        }
    }
}