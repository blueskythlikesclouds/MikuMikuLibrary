using System.IO;
using Newtonsoft.Json;

namespace MikuMikuLibrary.Objects.Processing.Json
{
    public static class JsonExporter
    {
        public static void ExportToFile(object objectToSerialize, string outputFilePath)
        {
            string json = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            File.WriteAllText(outputFilePath, json);
        }
    }
}