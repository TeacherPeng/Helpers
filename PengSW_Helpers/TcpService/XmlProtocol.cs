namespace PengSW.TcpService
{
    /// <summary>
    /// 基于Xml的应用协议类。
    ///     约定通讯双方传送的内容为Xml格式。
    ///     约定两个Xml完整帧不会在同一个帧中出现。也就是说，在一个数据块中，当出现首节的尾标签时，不会紧随出现下一个节标签。
    /// </summary>
    public class XmlProtocol : Protocol
    {
        /// <summary>
        /// 默认为使用UTF8编码
        /// </summary>
        public XmlProtocol()
        {
            m_Encoding = System.Text.Encoding.UTF8;
        }

        public XmlProtocol(System.Text.Encoding aEncoding)
        {
            m_Encoding = aEncoding;
        }

        protected System.Text.Encoding m_Encoding;

        protected override void OnStart()
        {
        }

        protected override bool HasFrameHead(byte[] aBytes)
        {
            string aText = m_Encoding.GetString(aBytes);
            return System.Text.RegularExpressions.Regex.IsMatch(aText, @"^\s*<([^\s/<>]+).*>");
        }

        protected override bool HasFrameTail(byte[] aBytes)
        {
            string aText = m_Encoding.GetString(aBytes);
            if (!System.Text.RegularExpressions.Regex.IsMatch(aText, @"</([^\s/<>]+)>\s*$|/>\s*$|^[^<]+>\s*$")) return false;
            aText = m_Encoding.GetString(_ByteBuffer.TotalBytes);
            try
            {
                new System.Xml.XmlDocument().LoadXml(aText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool AnalyBytes(Connection aConnection, byte[] aBytes)
        {
            string aText = m_Encoding.GetString(aBytes);
            try
            {
                System.Xml.XmlDocument aXmlDocument = new System.Xml.XmlDocument();
                aXmlDocument.LoadXml(aText);
                ClarifyFrameReceived(aText);
                ClarifyObjectReceived(aXmlDocument);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class XmlProtocolFactory : IProtocolFactory
    {
        public XmlProtocolFactory()
        {
            m_Encoding = System.Text.Encoding.UTF8;
        }

        public XmlProtocolFactory(System.Text.Encoding aEncoding)
        {
            m_Encoding = aEncoding;
        }

        private System.Text.Encoding m_Encoding;

        public Protocol CreateProtocol()
        {
            return new XmlProtocol(m_Encoding);
        }
    }
}
