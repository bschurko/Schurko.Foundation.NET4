
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class TransformExtension
    {
        public static string Transform(this XmlDocument obj, string xslUrl)
        {
            XmlDocument xslDoc = new XmlDocument();
            xslDoc.Load(xslUrl);
            return PerformTransform(obj, null, xslDoc, null);
        }

        public static string Transform(this XmlDocument obj, XsltArgumentList args, XmlDocument xslDoc) => PerformTransform(obj, args, xslDoc, null);

        private static string PerformTransform(
          XmlDocument xmlToTransform,
          XsltArgumentList args,
          XmlDocument xslDoc,
          XmlWriterSettings writerSettings)
        {
            XslCompiledTransform compiledTransform = new XslCompiledTransform();
            compiledTransform.Load(xslDoc);
            XmlWriterSettings settings = writerSettings;
            if (settings == null)
            {
                settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
            }
            StringBuilder output = new StringBuilder();
            XmlWriter results = XmlWriter.Create(output, settings);
            compiledTransform.Transform(xmlToTransform, args, results);
            return output.ToString();
        }
    }
}
