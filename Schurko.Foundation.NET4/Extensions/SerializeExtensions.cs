using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class SerializeExtensions
    {
        private static readonly object DictionaryLock = new object();
        private static readonly Type XmlType = typeof(XmlDocument);
        private static readonly Type StringType = typeof(string);
        private static readonly Type ByteArrayType = typeof(byte[]);
        private static readonly Dictionary<Type, XmlSerializer> Serializers = new Dictionary<Type, XmlSerializer>();

        public static XmlDocument Serialize(this object c) => c.Serialize<XmlDocument>();

        public static T Serialize<T>(this object objSerialize) where T : class => objSerialize.Serialize<T>(false);

        public static T Serialize<T>(this object objSerialize, bool suppressErrors) where T : class
        {
            XmlSerializer serializer = GetSerializer(objSerialize.GetType());
            return Serialize<T>(objSerialize, serializer, suppressErrors);
        }

        public static T Serialize<T>(
          this object objSerialize,
          bool suppressErrors,
          Type[] additionalTypeSupport)
          where T : class
        {
            XmlSerializer serializer = new XmlSerializer(objSerialize.GetType(), additionalTypeSupport);
            return Serialize<T>(objSerialize, serializer, suppressErrors);
        }

        public static T Serialize<T, X>(this List<X> objSerialise, bool suppressErrors) where T : class
        {
            Type type = typeof(List<X>);
            List<Type> typeList = new List<Type>();
            foreach (X x in objSerialise)
            {
                if (!typeList.Contains(x.GetType()))
                    typeList.Add(x.GetType());
            }
            Type[] array = typeList.ToArray();
            XmlSerializer serializer = new XmlSerializer(type, array);
            return Serialize<T>(objSerialise, serializer, suppressErrors);
        }

        private static T Serialize<T>(
          object objSerialize,
          XmlSerializer serializer,
          bool suppressErrors)
          where T : class
        {
            StringBuilder output = null;
            XmlWriter xmlWriter = null;
            T obj = default;
            Type type1 = typeof(T);
            Type type2 = objSerialize.GetType();
            if (!(type1 == XmlType) && !(type1 == StringType) && !(type1 == ByteArrayType))
                throw new InvalidCastException(string.Format("Unable to return the type [{0}] using this method. Method supports XmlDocument, String, byte[]", type1.Name));
            try
            {
                lock (type2)
                {
                    output = new StringBuilder();
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        Indent = true,
                        OmitXmlDeclaration = true
                    };
                    xmlWriter = XmlWriter.Create(output, settings);
                    serializer.Serialize(xmlWriter, objSerialize);
                    xmlWriter.Close();
                }
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                    throw;
            }
            finally
            {
                xmlWriter?.Close();
            }
            if (output != null && output.Length > 0)
            {
                if (type1 == XmlType)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(output.ToString());
                    obj = xmlDocument as T;
                }
                if (type1 == ByteArrayType)
                    obj = Encoding.Default.GetBytes(output.ToString()) as T;
                if (type1 == StringType)
                    obj = output.ToString() as T;
            }
            return obj;
        }

        public static T Deserialize<T>(this byte[] xmlData, Encoding encoder, bool suppressErrors) where T : class
        {
            if (encoder == null)
                encoder = Encoding.Default;
            string xml = encoder.GetString(xmlData);
            XmlDocument xmlData1 = new XmlDocument();
            xmlData1.LoadXml(xml);
            return xmlData1.Deserialize<T>(suppressErrors);
        }

        public static T Deserialize<T>(this string stringData) where T : class => stringData.Deserialize<T>(false);

        public static T Deserialize<T>(this string stringData, bool suppressErrors) where T : class
        {
            XmlDocument xmlData = new XmlDocument();
            try
            {
                xmlData.LoadXml(stringData);
            }
            catch
            {
                if (!suppressErrors)
                    throw;
            }
            return xmlData.Deserialize<T>(suppressErrors);
        }

        public static T Deserialize<T>(this XmlDocument xmlData) where T : class => xmlData.Deserialize<T>(false);

        public static T Deserialize<T>(this XmlDocument xmlData, bool suppressErrors) where T : class
        {
            XmlSerializer serializer = GetSerializer(typeof(T));
            return DeserializeInternal<T>(xmlData, serializer, suppressErrors);
        }

        public static T Deserialize<T>(
          this XmlDocument xmlData,
          Type[] additionalTypes,
          bool suppressErrors)
          where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), additionalTypes);
            return DeserializeInternal<T>(xmlData, serializer, suppressErrors);
        }

        private static T DeserializeInternal<T>(
          XmlDocument xmlData,
          XmlSerializer serializer,
          bool suppressErrors)
          where T : class
        {
            Type objectType = typeof(T);
            T obj = default;
            try
            {
                string objectXmlRootName = GetObjectXmlRootName(objectType);
                if (xmlData.DocumentElement.Name != objectXmlRootName && xmlData.DocumentElement.Name.IndexOf("Array") < 0)
                {
                    obj = Deserialize<T>(xmlData, serializer);
                }
                else
                {
                    XmlReader xmlReader = XmlReader.Create(new StringReader(xmlData.InnerXml));
                    obj = serializer.Deserialize(xmlReader) as T;
                }
            }
            catch
            {
                if (!suppressErrors)
                    throw;
            }
            return obj;
        }

        private static XmlSerializer GetSerializer(Type targetType)
        {
            XmlSerializer serializer;
            lock (DictionaryLock)
            {
                if (Serializers.ContainsKey(targetType))
                {
                    serializer = Serializers[targetType];
                }
                else
                {
                    serializer = new XmlSerializer(targetType);
                    Serializers[targetType] = serializer;
                }
            }
            return serializer;
        }

        public static XmlDocument SuppressNameSpace(this XmlDocument data)
        {
            string xml = Regex.Replace(data.InnerXml, "(xmlns:[\\d\\D\\w\\W]+?[\"][\\d\\D\\w\\W]+?[\"])", "");
            data.LoadXml(xml);
            return data;
        }

        public static T Deserialize<T>(
          XmlDocument xmlObject,
          string XPathSelect,
          string objectXPathPlaceholder)
        {
            Type objectType = typeof(T);
            XmlDocument xmlObjectData = new XmlDocument();
            if (!string.IsNullOrWhiteSpace(XPathSelect))
            {
                XmlNode xmlNode = xmlObject.SelectSingleNode(XPathSelect.Replace(objectXPathPlaceholder, GetObjectXmlRootName(objectType)));
                if (xmlNode == null)
                    return default;
                xmlObjectData.LoadXml(xmlNode.OuterXml);
            }
            return Deserialize<T>(xmlObjectData, null);
        }

        public static T Deserialize<T>(XmlDocument xmlObjectData, XmlSerializer xmlSerializer)
        {
            Type type = typeof(T);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlObjectData.InnerXml);
            string objectXmlRootName = GetObjectXmlRootName(type);
            if (xmlDocument.DocumentElement.Name != objectXmlRootName && xmlDocument.DocumentElement.Name.IndexOf("Array") < 0)
            {
                string namespaceUri = xmlDocument.DocumentElement.NamespaceURI;
                XmlNode xmlNode = namespaceUri == null || namespaceUri.Length <= 0 ? xmlDocument.SelectSingleNode("//" + objectXmlRootName) : xmlDocument.SelectSingleNode("//*[name()='" + objectXmlRootName + "']");
                if (xmlNode != null)
                    xmlDocument.LoadXml(xmlNode.OuterXml);
            }
            if (xmlSerializer == null)
                xmlSerializer = new XmlSerializer(type);
            XmlTextReader xmlTextReader = new XmlTextReader(xmlDocument.InnerXml, XmlNodeType.Document, null);
            return (T)xmlSerializer.Deserialize(xmlTextReader);
        }

        public static string GetObjectXmlRootName(Type objectType)
        {
            string objectXmlRootName = objectType.Name;
            XmlRootAttribute customAttribute = GetCustomAttribute<XmlRootAttribute>(objectType);
            if (customAttribute != null)
                objectXmlRootName = customAttribute.ElementName;
            return objectXmlRootName;
        }

        public static object GetCustomAttribute(Type attributeType, PropertyInfo property)
        {
            foreach (object customAttribute in property.GetCustomAttributes(true))
            {
                if (customAttribute.GetType() == attributeType || customAttribute.GetType().GetInterface(attributeType.Name) != null)
                    return customAttribute;
            }
            return null;
        }

        public static object GetCustomAttribute(Type attributeType, Type objectType)
        {
            object[] customAttributes = objectType.GetCustomAttributes(true);
            for (int index = 0; index < customAttributes.Length; ++index)
            {
                if (customAttributes[index].GetType() == attributeType || customAttributes[index].GetType().GetInterface(attributeType.Name) != null)
                    return customAttributes[index];
            }
            return null;
        }

        public static T GetCustomAttribute<T>(Type objectType) where T : Attribute
        {
            T customAttribute = default;
            object[] customAttributes = objectType.GetCustomAttributes(typeof(T), true);
            if (customAttributes.Length != 0)
                customAttribute = customAttributes[0] as T;
            return customAttribute;
        }
    }
}
