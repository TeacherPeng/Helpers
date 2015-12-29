using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Soap;

namespace PengSW.XmlHelper
{
    /// <summary>
    /// 基于System.Xml的XmlDocument，以扩展方法的形式包装常用的Xml操作。
    /// </summary>
    public static class XmlHelper
    {
        public static string GetXPathValue(this XmlNode aXmlNode, string aXPath, string aDefaultValue)
        {
            if (aXmlNode == null) return aDefaultValue;
            XmlNode aTargetNode = aXmlNode.SelectSingleNode(aXPath);
            if (aTargetNode == null) return aDefaultValue;
            return aTargetNode.InnerText;
        }

        public static int GetXPathValue(this XmlNode aXmlNode, string aXPath, int aDefaultValue)
        {
            if (aXmlNode == null) return aDefaultValue;
            XmlNode aTargetNode = aXmlNode.SelectSingleNode(aXPath);
            if (aTargetNode == null) return aDefaultValue;
            int aValue = aDefaultValue;
            if (!int.TryParse(aTargetNode.InnerText, out aValue)) return aDefaultValue;
            return aValue;
        }

        public static bool GetXPathValue(this XmlNode aXmlNode, string aXPath, bool aDefaultValue)
        {
            if (aXmlNode == null) return aDefaultValue;
            XmlNode aTargetNode = aXmlNode.SelectSingleNode(aXPath);
            if (aTargetNode == null) return aDefaultValue;
            bool aValue = aDefaultValue;
            if (!bool.TryParse(aTargetNode.InnerText, out aValue)) return aDefaultValue;
            return aValue;
        }

        public static XmlNode CreateChildNode(this XmlNode aXmlNode, string aNodeName)
        {
            if (aXmlNode == null) return null;
            return aXmlNode.AppendChild(aXmlNode.OwnerDocument.CreateElement(aNodeName));
        }

        public static XmlNode CreateChildNode(this XmlNode aXmlNode, string aNodeName, string aValue)
        {
            if (aXmlNode == null) return null;
            XmlNode aChildNode = CreateChildNode(aXmlNode, aNodeName);
            aChildNode.InnerText = aValue;
            return aChildNode;
        }

        public static void CreateAttribute(this XmlNode aXmlNode, string aAttributeName, string aValue)
        {
            if (aXmlNode == null) return;
            aXmlNode.Attributes.Append(aXmlNode.OwnerDocument.CreateAttribute(aAttributeName)).Value = aValue;
        }

        public static string GetAttributeValue(this XmlNode aXmlNode, string aAttributeName, string aDefaultValue)
        {
            if (aXmlNode == null) return aDefaultValue;
            if (aXmlNode.Attributes[aAttributeName] == null) return aDefaultValue;
            return aXmlNode.Attributes[aAttributeName].Value;
        }

        public static void ReadFromXml(this System.Data.DataTable aDataTable, XmlNode aXmlNode)
        {
            if (aDataTable == null) return;
            aDataTable.Clear();
            if (aXmlNode == null) return;
            System.IO.StringReader aStringReader = new System.IO.StringReader(aXmlNode.InnerText);
            XmlReader aXmlReader = XmlReader.Create(aStringReader);
            aDataTable.ReadXml(aXmlReader);
        }

        public static void WriteToXml(this System.Data.DataTable aDataTable, XmlNode aXmlNode)
        {
            if (aXmlNode == null || aDataTable == null) return;
            System.Text.StringBuilder aStringBuilder = new System.Text.StringBuilder();
            XmlWriterSettings aXmlWriterSettings = new XmlWriterSettings();
            aXmlWriterSettings.OmitXmlDeclaration = true;
            XmlWriter aXmlWriter = XmlWriter.Create(aStringBuilder, aXmlWriterSettings);
            aDataTable.WriteXml(aXmlWriter, System.Data.XmlWriteMode.WriteSchema);
            aXmlWriter.Close();
            aXmlNode.InnerText = aStringBuilder.ToString();
        }

        public static void SerializeTo<T>(this T aObject, Stream aStream)
        {
            SoapFormatter aFormatter = new SoapFormatter();
            aFormatter.Serialize(aStream, aObject);
        }

        public static void SerializeTo<T>(this T aObject, string aFileName)
        {
            using (FileStream aStream = new FileStream(aFileName, FileMode.Create, FileAccess.Write))
            {
                SerializeTo(aObject, aStream);
                aStream.Close();
            }
        }

        public static T SerializeFrom<T>(Stream aStream)
        {
            T aObject = default(T);
            SoapFormatter aFormatter = new SoapFormatter();
            aObject = (T)aFormatter.Deserialize(aStream);
            return aObject;
        }

        public static T SerializeFrom<T>(string aFileName)
        {
            T aObject = default(T);
            using (FileStream aStream = new FileStream(aFileName, FileMode.Open, FileAccess.Read))
            {
                aObject = SerializeFrom<T>(aStream);
                aStream.Close();
            }
            return aObject;
        }
    }
}
