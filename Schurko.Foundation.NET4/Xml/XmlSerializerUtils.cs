
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace Schurko.Foundation.Xml
{
    /// <summary>
    /// Serializes an object to xml.
    /// </summary>
    public class XmlSerializerUtil
    {
        /// <summary>
        /// Serialize the object to xml.
        /// </summary>
        /// <typeparam name="T">Type of object to serialize.</typeparam>
        /// <param name="item">Object to serialize.</param>
        /// <returns>XML contents representing the serialized object.</returns>
        public static string XmlSerialize<T>(T item)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter writer = new StringWriter(stringBuilder))
            {
                serializer.Serialize(writer, item);
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Serialize the object to xml.
        /// </summary>
        /// <param name="item">Object to serialize.</param>
        /// <returns>XML contents representing the serialized object.</returns>
        public static string XmlSerialize(object item)
        {
            Type type = item.GetType();
            XmlSerializer serializer = new XmlSerializer(type);
            StringBuilder stringBuilder = new StringBuilder();
            using (StringWriter writer = new StringWriter(stringBuilder))
            {
                serializer.Serialize(writer, item);
            }
            return stringBuilder.ToString();
        }


        /// <summary>
        /// Deserialize from xml to the appropriate typed object.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize.</typeparam>
        /// <param name="xmlData">XML contents with serialized object.</param>
        /// <returns>Deserialized object.</returns>
        public static T XmlDeserialize<T>(string xmlData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xmlData))
            {
                T entity = (T)serializer.Deserialize(reader);
                return entity;
            }
        }
    }
}
