using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PengSW.TcpService
{
    /// <summary>
    /// 基于Xml的应用协议类。
    ///     约定通讯双方传送的内容为Xml格式。
    ///     约定多个Xml完整帧可能在同一个帧中出现，因此返回结果为Xml集合。
    /// </summary>
    public class XmlsProtocol : Protocol
    {
        /// <summary>
        /// 默认为使用UTF8编码
        /// </summary>
        public XmlsProtocol()
        {
            _Encoding = System.Text.Encoding.UTF8;
        }

        public XmlsProtocol(System.Text.Encoding aEncoding)
        {
            _Encoding = aEncoding;
        }

        protected System.Text.Encoding _Encoding;

        protected override void OnStart()
        {
        }

        private static readonly Regex _FrameHeadRegex = new Regex(@"^\s*<([^\s/<>]+).*>");
        protected override bool HasFrameHead(byte[] aBytes)
        {
            string aText = _Encoding.GetString(aBytes);
            return _FrameHeadRegex.IsMatch(aText);
        }

        private static readonly Regex _FrameTailRegex = new Regex(@"</([^\s/<>]+)>\s*$|/>\s*$|^[^<]+>\s*$");
        private static readonly Regex _FilteRegex = new Regex(@"<\?.+?\?>");
        protected override bool HasFrameTail(byte[] aBytes)
        {
            string aText = _Encoding.GetString(aBytes);
            if (!_FrameTailRegex.IsMatch(aText)) return false;
            aText = _Encoding.GetString(_ByteBuffer.TotalBytes);
            aText = _FilteRegex.Replace(aText, "");
            aText = $"<xml>{aText}</xml>";
            try
            {
                XDocument.Parse(aText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool AnalyBytes(byte[] aBytes)
        {
            string aText = _Encoding.GetString(aBytes);
            aText = _FilteRegex.Replace(aText, "");
            aText = $"<xml>{aText}</xml>";
            try
            {
                ClarifyFrameReceived(aText);
                ClarifyObjectReceived((from r in XDocument.Parse(aText).Root.Elements() select new XDocument(r)).ToList());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Test()
        {
            string aText = @"<?xml version=""1.0""?>
<Message xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>1</Type>
  <Name>txtName1</Name>
  <Value>大树</Value>
</Message><?xml version=""1.0""?>
<Message xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Type>1</Type>
  <Name>txtName2</Name>
  <Value>大叶</Value>
</Message>";
            OnBytesReceived(_Encoding.GetBytes(aText));
        }
    }

    public class XmlsProtocolFactory : IProtocolFactory
    {
        public XmlsProtocolFactory()
        {
            _Encoding = System.Text.Encoding.UTF8;
        }

        public XmlsProtocolFactory(System.Text.Encoding aEncoding)
        {
            _Encoding = aEncoding;
        }

        private System.Text.Encoding _Encoding;

        public Protocol CreateProtocol() => new XmlsProtocol(_Encoding);
        public string CreateName() => "XmlsConnection";
    }
}
