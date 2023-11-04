
using System.Xml;
using System.Xml.XPath;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class XmlReaderExtend
    {
        public static XmlDocument AsXmlDocument(this XmlReader xmlResult, string rootName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XPathNavigator navigator = new XPathDocument(xmlResult).CreateNavigator();
            XmlElement element = xmlDocument.CreateElement(rootName);
            element.InnerXml = navigator.OuterXml;
            xmlDocument.AppendChild(element);
            return xmlDocument;
        }

        public static XmlDocument BuildXml(this XmlReader xmlResult) => xmlResult.AsXmlDocument("root");
    }
}
