using MikuMikuLibrary.Bones;
using MikuMikuLibrary.Databases;
using MikuMikuLibrary.IO;
using MikuMikuLibrary.Stages;

namespace DatabaseConverter;

internal class Program
{
    private static readonly DatabaseInfo[] sDatabaseInfos =
    {
        new(typeof(AetDatabase), "aetdb", "aei"),
        new(typeof(BoneData), "bonedata", "bon"),
        new(typeof(MotionDatabase), "motdb", null),
        new(typeof(ObjectDatabase), "objdb", "osi"),
        new(typeof(SpriteDatabase), "sprdb", "spi"),
        new(typeof(StageData), "stagedata", "stg"),
        new(typeof(StringArray), "strarray", "str"),
        new(typeof(StringArray), "stringarray", "str"),
        new(typeof(TextureDatabase), "texdb", "txi")
    };

    private static DatabaseInfo GetDatabaseInfo(string fileName)
    {
        if (fileName.EndsWith(".xml"))
            fileName = Path.ChangeExtension(fileName, null);

        string extension = Path.GetExtension(fileName).Trim('.');

        fileName = Path.GetFileNameWithoutExtension(fileName).Replace("_", "");

        return sDatabaseInfos.FirstOrDefault(databaseInfo =>
            fileName.IndexOf(databaseInfo.ClassicFileName, StringComparison.OrdinalIgnoreCase) != -1 ||
            !string.IsNullOrEmpty(databaseInfo.ModernFileExtension) &&
            databaseInfo.ModernFileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase));
    }

    private static void Main(string[] args)
    {
        string sourceFileName = null;
        string destinationFileName = null;

        foreach (string arg in args)
        {
            if (sourceFileName == null)
                sourceFileName = arg;

            else if (destinationFileName == null)
                destinationFileName = arg;
        }

        if (sourceFileName == null)
        {
            Console.WriteLine(@"~~~~~~~~~~~~~~~~~~
Database Converter
~~~~~~~~~~~~~~~~~~
This tool allows you to convert database files to XML and vice versa.

~~~~~
Usage
~~~~~
DatabaseConverter [source] [destination]

Destination is optional, which makes it possible to do drag and drop onto the executable.

~~~~~~~~~~~~~~~
Supported Files
~~~~~~~~~~~~~~~
AET Database (aet_db.bin)
Bone Database (bone_data.bin)
Edit Database (edit_db.bin)
Motion Database (mot_db.bin within mot_db.farc)
Object Database (obj_db.bin)
Sprite Database (spr_db.bin)
Stage Database (stage_data.bin)
String Array (str_array.bin and string_array.bin)
Texture Database (tex_db.bin)

DLC database files in Future Tone (files with 'mdata' prefix) are also supported.");
            Console.ReadLine();
            return;
        }

        if (destinationFileName == null)
            destinationFileName = sourceFileName;

        var databaseInfo = GetDatabaseInfo(sourceFileName);

        if (databaseInfo != null)
        {
            if (sourceFileName.EndsWith("xml", StringComparison.OrdinalIgnoreCase))
            {
                var serializer = new XmlSerializer(databaseInfo.Type);

                IBinaryFile database;

                using (var source = File.OpenText(sourceFileName))
                    database = (IBinaryFile)serializer.Deserialize(source);

                if (BinaryFormatUtilities.IsModern(database.Format) && !string.IsNullOrEmpty(databaseInfo.ModernFileExtension))
                    destinationFileName = Path.ChangeExtension(destinationFileName, null);

                else
                    destinationFileName = Path.ChangeExtension(destinationFileName, "bin");

                database.Save(destinationFileName);
            }
            else
            {
                var database = (IBinaryFile)Activator.CreateInstance(databaseInfo.Type);
                database.Load(sourceFileName);

                if (BinaryFormatUtilities.IsModern(database.Format))
                    destinationFileName = Path.ChangeExtension(destinationFileName, databaseInfo.ModernFileExtension) + ".xml";

                else
                    destinationFileName = Path.ChangeExtension(destinationFileName, "xml");

                var serializer = new XmlSerializer(databaseInfo.Type);

                using (var destination = File.CreateText(destinationFileName))
                    serializer.Serialize(destination, database);
            }
        }
        else
        {
            throw new InvalidDataException("Database type could not be detected");
        }
    }

    private class DatabaseInfo
    {
        public readonly Type Type;
        public readonly string ClassicFileName;
        public readonly string ModernFileExtension;

        public DatabaseInfo(Type type, string classicFileName, string modernFileExtension)
        {
            Type = type;
            ClassicFileName = classicFileName;
            ModernFileExtension = modernFileExtension;
        }
    }
}