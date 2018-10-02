﻿using System.Text;
using System.Xml.Linq;

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
            _Encoding = Encoding.UTF8;
        }

        public XmlProtocol(System.Text.Encoding aEncoding)
        {
            _Encoding = aEncoding;
        }

        protected System.Text.Encoding _Encoding;

        protected override void OnStart()
        {
        }

        protected override bool HasFrameHead(byte[] aBytes)
        {
            string aText = _Encoding.GetString(aBytes);
            return System.Text.RegularExpressions.Regex.IsMatch(aText, @"^\s*<([^\s/<>]+).*>");
        }

        protected override bool HasFrameTail(byte[] aBytes)
        {
            string aText = _Encoding.GetString(aBytes);
            if (!System.Text.RegularExpressions.Regex.IsMatch(aText, @"</([^\s/<>]+)>\s*$|/>\s*$|^[^<]+>\s*$")) return false;
            aText = _Encoding.GetString(_ByteBuffer.TotalBytes);
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
            try
            {
                ClarifyFrameReceived(aText);
                ClarifyObjectReceived(XDocument.Parse(aText));
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
            _Encoding = System.Text.Encoding.UTF8;
        }

        public XmlProtocolFactory(System.Text.Encoding aEncoding)
        {
            _Encoding = aEncoding;
        }

        private System.Text.Encoding _Encoding;

        public Protocol CreateProtocol() => new XmlProtocol(_Encoding);
        public string CreateName() => "XmlConnection";
    }
}
