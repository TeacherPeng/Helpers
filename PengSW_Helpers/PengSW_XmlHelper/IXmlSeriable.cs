using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PengSW.XmlHelper
{
    public interface IXmlSeriable
    {
        void ReadFromXml(XmlNode aXmlNode);
        void WriteToXml(XmlNode aXmlNode);
    }
}
