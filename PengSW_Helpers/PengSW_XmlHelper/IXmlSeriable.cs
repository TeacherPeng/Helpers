using System.Xml.Linq;
using Microsoft.Win32;

namespace PengSW.XmlHelper
{
    public interface IXmlSeriable
    {
        void ReadFromXml(XElement aXElement);
        XElement CreateXElement(string aXmlNodeName);
    }

    public interface ISavable
    {
        string DefaultExt { get; }
        string Filter { get; }
        string SaveTitle { get; }
        string DefaultFileName { get; }
        void Save(string aFileName);
    }
}
