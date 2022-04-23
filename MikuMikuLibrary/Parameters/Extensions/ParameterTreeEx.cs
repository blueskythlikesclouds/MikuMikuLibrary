using System.IO;

namespace MikuMikuLibrary.Parameters.Extensions
{
    public static class ParameterTreeEx
    {
        public static void Save( this ParameterTree paramTree, Stream stream )
        {
            var writer = new ParameterTreeWriter();
            paramTree.Write( writer );
            writer.Flush( stream );
        }

        public static void Save( this ParameterTree paramTree, string filePath )
        {
            using ( var stream = File.Create( filePath ) )
                paramTree.Save( stream );
        }
    }
}