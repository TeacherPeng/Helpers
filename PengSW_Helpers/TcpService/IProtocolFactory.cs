using System;

namespace PengSW.TcpService
{
    /// <summary>
    /// 协议抽象工厂
    ///     描述协议类实例工厂的接口。
    /// </summary>
    public interface IProtocolFactory
    {
        Protocol CreateProtocol();
        string CreateName();
    }

    public class DefaultProtocolFactory<T> : IProtocolFactory where T : Protocol, new()
    {
        public DefaultProtocolFactory(string aConnectionName, TimeSpan? aReceiveTimeOut = null)
        {
            _ConnectionName = aConnectionName;
            _ReceiveTimeSpan = aReceiveTimeOut;
        }

        private string _ConnectionName;
        private TimeSpan? _ReceiveTimeSpan;

        public Protocol CreateProtocol()
        {
            T aProtocol = new T();
            if (_ReceiveTimeSpan != null) aProtocol.ReceiveTimeOut = _ReceiveTimeSpan.Value;
            return aProtocol;
        }

        public string CreateName() => _ConnectionName;
    }
}
