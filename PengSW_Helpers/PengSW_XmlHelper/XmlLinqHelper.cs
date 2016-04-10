using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace PengSW.XmlHelper
{
    public static class XmlLinqHelper
    {
        public static string GetChildNodeValue(this XElement aXElement, string aChildNodeName, string aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XElement aChildXElement = aXElement.Element(aChildNodeName);
            if (aChildXElement == null) return aDefaultValue;
            return aChildXElement.Value;
        }

        public static string GetAttributeValue(this XElement aXElement, string aAttributeName)
        {
            if (aXElement == null) throw new System.ApplicationException($"无法从无效的Xml结点中提取[{aAttributeName}]属性！");
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) throw new System.ApplicationException($"在[{aXElement}]中没有找到[{aAttributeName}]属性！");
            return aXAttribute.Value;
        }

        public static string GetAttributeValue(this XElement aXElement, string aAttributeName, string aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) return aDefaultValue;
            return aXAttribute.Value;
        }

        public static DateTime GetAttributeValue(this XElement aXElement, string aAttributeName, DateTime aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) return aDefaultValue;
            DateTime aValue = aDefaultValue;
            if (!DateTime.TryParse(aXAttribute.Value, out aValue)) return aDefaultValue;
            return aValue;
        }

        public static TimeSpan GetAttributeValue(this XElement aXElement, string aAttributeName, TimeSpan aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) return aDefaultValue;
            TimeSpan aValue = aDefaultValue;
            if (!TimeSpan.TryParse(aXAttribute.Value, out aValue)) return aDefaultValue;
            return aValue;
        }

        public static double GetAttributeValue(this XElement aXElement, string aAttributeName, double aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) return aDefaultValue;
            double aValue = aDefaultValue;
            if (!double.TryParse(aXAttribute.Value, out aValue)) return aDefaultValue;
            return aValue;
        }
        public static int GetAttributeValue(this XElement aXElement, string aAttributeName, int aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) return aDefaultValue;
            int aValue = aDefaultValue;
            if (!int.TryParse(aXAttribute.Value, out aValue)) return aDefaultValue;
            return aValue;
        }

        public static bool GetAttributeValue(this XElement aXElement, string aAttributeName, bool aDefaultValue)
        {
            if (aXElement == null) return aDefaultValue;
            XAttribute aXAttribute = aXElement.Attribute(aAttributeName);
            if (aXAttribute == null) return aDefaultValue;
            bool aValue = aDefaultValue;
            if (!bool.TryParse(aXAttribute.Value, out aValue)) return aDefaultValue;
            return aValue;
        }

        public static XElement GetXElement(this XNode aParentXElement, string aXPath, string aPromptName = "")
        {
            XElement aXElement = aParentXElement.XPathSelectElement(aXPath);
            if (aXElement == null) throw new System.ApplicationException($"未找到{aPromptName}：[{aXPath}]");
            return aXElement;
        }

        public static XElement GetXElement(this XNode aParentXElement, string aXPath, IXmlNamespaceResolver aXmlNamespaceResolver, string aPromptName = "")
        {
            XElement aXElement = aParentXElement.XPathSelectElement(aXPath, aXmlNamespaceResolver);
            if (aXElement == null) throw new System.ApplicationException(string.Format("未找到{0}：[{1}]", aPromptName, aXPath));
            return aXElement;
        }

        public static DateTime GetDateTime(this XElement aParentXElement, string aXPath, string aPromptName = "")
        {
            XElement aDateTimeXElement = aParentXElement.GetXElement(aXPath, aPromptName);
            DateTime aDateTime;
            if (!DateTime.TryParse(aDateTimeXElement.Value, out aDateTime)) throw new ApplicationException(string.Format("无效的日期或时间：[{0}]！", aDateTimeXElement.Value));
            return aDateTime;
        }

        public static TimeSpan GetTimeSpan(this XElement aParentXElement, string aXPath, string aPromptName = "")
        {
            XElement aTimeSpanXElement = aParentXElement.GetXElement(aXPath, aPromptName);
            string aTimeSpanText = System.Text.RegularExpressions.Regex.Replace(aTimeSpanXElement.Value, @"(\d+:\d+:\d+):(\d+)", "$1.$2");
            TimeSpan aTimeSpan;
            if (!TimeSpan.TryParse(aTimeSpanText, out aTimeSpan)) throw new ApplicationException(string.Format("无效的日期或时间：[{0}]！", aTimeSpanText));
            return aTimeSpan;
        }

        public static int GetInt(this XElement aParentXElement, string aXPath, string aPromptName = "")
        {
            XElement aIntXElement = aParentXElement.GetXElement(aXPath, aPromptName);
            int aInt;
            if (!int.TryParse(aIntXElement.Value, out aInt)) throw new System.ApplicationException(string.Format("无效的整数值：[{0}]！", aIntXElement.Value));
            return aInt;
        }

        public static int GetInt(this XElement aParentXElement, string aXPath, int aDefaultValue)
        {
            int aInt = aDefaultValue;
            try
            {
                XElement aIntXElement = aParentXElement.GetXElement(aXPath, "");
                int.TryParse(aIntXElement.Value, out aInt);
            }
            catch
            {
            }
            return aInt;
        }

        public static double GetDouble(this XElement aParentXElement, string aXPath, string aPromptName = "")
        {
            XElement aValueXElement = aParentXElement.GetXElement(aXPath, aPromptName);
            double aValue;
            if (!double.TryParse(aValueXElement.Value, out aValue)) throw new System.ApplicationException(string.Format("无效的数值：[{0}]！", aValueXElement.Value));
            return aValue;
        }

        public static bool GetBool(this XElement aParentXElement, string aXPath, string aPromptName = "")
        {
            XElement aValueXElement = aParentXElement.GetXElement(aXPath, aPromptName);
            bool aValue;
            if (!bool.TryParse(aValueXElement.Value, out aValue)) throw new System.ApplicationException(string.Format("无效的逻辑值：[{0}]！", aValueXElement.Value));
            return aValue;
        }

        public static XElement WriteTypeToXml<T>(this T aObject, XElement aXElement, bool aAssemblyEnabled = true)
        {
            if (aObject == null || aXElement == null) return aXElement;
            aXElement.Add(
                aAssemblyEnabled ? new XAttribute("Assembly", aObject.GetType().Assembly.FullName) : null,
                new XAttribute("TypeName", aObject.GetType().FullName));
            return aXElement;
        }

        public static T CreateObjectFromXml<T>(this XElement aXElement, string aDefaultAssembly = null)
        {
            if (aXElement == null) return default(T);
            string aAssembly = aXElement.GetAttributeValue("Assembly", null);
            if (aAssembly == null) aAssembly = aDefaultAssembly;
            string aTypeName = aXElement.GetAttributeValue("TypeName", null);
            if (aTypeName == null) return default(T);
            System.Runtime.Remoting.ObjectHandle aObjectHandler = Activator.CreateInstance(aAssembly, aTypeName);
            T aObject = (T)aObjectHandler.Unwrap();
            return aObject;
        }

        public static int GetInt(this XElement aXElement, int aDefaultValue)
        {
            int aValue = aDefaultValue;
            int.TryParse(aXElement.Value, out aValue);
            return aValue;
        }

        public static bool GetBool(this XElement aXElement, bool aDefaultValue)
        {
            bool aValue = aDefaultValue;
            bool.TryParse(aXElement.Value, out aValue);
            return aValue;
        }

        public static object GetEnum(this XElement aXElement, Type aEnumType, object aDefaultValue)
        {
            try
            {
                return Enum.Parse(aEnumType, aXElement.Value);
            }
            catch
            {
                return aDefaultValue;
            }
        }

        public static int GetIntAttribute(this XElement aXElement, string aAttributeName, int aDefaultValue)
        {
            try
            {
                return int.Parse(aXElement.Attribute(aAttributeName).Value);
            }
            catch
            {
                return aDefaultValue;
            }
        }

        public static bool GetBoolAttribute(this XElement aXElement, string aAttributeName)
        {
            string aValueString = aXElement.GetAttributeValue(aAttributeName);
            bool aValue;
            if (!bool.TryParse(aValueString, out aValue)) throw new System.ApplicationException(string.Format("[{0}]中的属性[{1}]无法解析为逻辑值！", aXElement, aAttributeName));
            return aValue;
        }

        public static bool GetBoolAttribute(this XElement aXElement, string aAttributeName, bool aDefaultValue)
        {
            try
            {
                return bool.Parse(aXElement.Attribute(aAttributeName).Value);
            }
            catch
            {
                return aDefaultValue;
            }
        }

        public static double GetDoubleAttribute(this XElement aXElement, string aAttributeName)
        {
            string aValueString = aXElement.GetAttributeValue(aAttributeName);
            double aValue;
            if (!double.TryParse(aValueString, out aValue)) throw new System.ApplicationException(string.Format("[{0}]中的属性[{1}]无法解析为数值！", aXElement, aAttributeName));
            return aValue;
        }

        public static double GetDoubleAttribute(this XElement aXElement, string aAttributeName, Double aDefaultValue)
        {
            try
            {
                return double.Parse(aXElement.Attribute(aAttributeName).Value);
            }
            catch
            {
                return aDefaultValue;
            }
        }

        public static object GetEnumAttribute(this XElement aXElement, string aAttributeName, Type aEnumType, object aDefaultValue)
        {
            try
            {
                return Enum.Parse(aEnumType, aXElement.Attribute(aAttributeName).Value);
            }
            catch
            {
                return aDefaultValue;
            }
        }

        public static XAttribute CreateXAttribute(this object sender, string aAttributeName, object aAttributeValue)
        {
            return CreateXAttribute(aAttributeName, aAttributeValue);
        }
        public static XAttribute CreateXAttribute(string aAttributeName, object aAttributeValue)
        {
            if (aAttributeValue == null) return null;
            return new XAttribute(aAttributeName, aAttributeValue);
        }
        public static XAttribute CreateXAttribute(this object sender, string aAttributeName, TimeSpan aAttributeValue)
        {
            return CreateXAttribute(aAttributeName, aAttributeValue);
        }
        public static XAttribute CreateXAttribute(string aAttributeName, TimeSpan aAttributeValue)
        {
            if (aAttributeValue == null) return null;
            return new XAttribute(aAttributeName, aAttributeValue.ToString("hh\\:mm\\:ss\\.fff"));
        }
    }
}
