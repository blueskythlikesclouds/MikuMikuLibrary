using System.IO;
using System.Xml;

namespace MikuMikuModel.Modules.Xml
{
    public class XmlDocumentModule : FormatModule<XmlDocument>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Xml Document";
        public override string[] Extensions => new[] { "xml" };

        protected override XmlDocument ImportCore( Stream source, string fileName )
        {
            var document = new XmlDocument();
            document.Load( source );
            return document;
        }

        protected override void ExportCore( XmlDocument model, Stream destination, string fileName ) =>
            model.Save( destination );
    }
}