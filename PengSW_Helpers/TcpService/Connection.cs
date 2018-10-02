using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;
using PengSW.NotifyPropertyChanged;

namespace PengSW.TcpService
{
    /// <summary>
    /// 连接类，负责一个Tcp连接，提供以轮询方式接收字节流。
    /// </summary>
    public class Connection : NotifyPropertyChangedObject, IDisposable
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        ///     构造时指定准备使用的通讯协议。
        /// </summary>
        /// <param name="aProtocol">准备使用的通讯协议实例，Protocol实例不支持多个Connection实例共用。</param>
        public Connection(Protocol aProtocol, string aName)
        {
            Name = aName;

            // 保存准备使用的通讯协议
            Protocol = aProtocol;
            if (Protocol != null)
            {
                Protocol.SetConnection(this);
                Protocol.ByteFrameReceived += new System.Action<byte[]>(Protocol_ByteFrameReceived);
                Protocol.StringFrameReceived += new System.Action<string>(Protocol_StringFrameReceived);
                Protocol.ObjectFrameReceived += new System.Action<object>(Protocol_ObjectFrameReceived);
                Protocol.Clarify += OnProtocol_Clarify;
                ReceiveTimeOut = Protocol.ReceiveTimeOut;
            }

            _ReceiveBuffer = new byte[ReceiveBufferLength];
            _ReadCallback = new AsyncCallback(ReadCallback);

            _ReceiveTimeOutTimer = new DispatcherTimer()
            {
                Interval = ReceiveTimeOut
            };
            _ReceiveTimeOutTimer.Tick += OnRecveTimeOut_Tick;

            _HeartbeatTimer = new DispatcherTimer()
            {
                Interval = HeartBeatInterval
            };
            _HeartbeatTimer.Tick += OnHeartbeat_Tick;
        }

        private void OnHeartbeat_Tick(object sender, EventArgs e)
        {
            try
            {
                if ((_TcpClient.Client.Poll(1000, SelectMode.SelectRead) && (_TcpClient.Client.Available == 0)) || !_TcpClient.Client.Connected) throw new ApplicationException("连接状态错误");
                byte[] aBytes = Protocol == null ? new byte[0] : Protocol.HeartBeat();
                if (aBytes != null && (aBytes.Length == 0))
                {
                    if (_TcpClient.Client.Send(aBytes) != aBytes.Length) throw new ApplicationException("心跳失败！");
                }
            }
            catch (Exception ex)
            {
                ClarifyInfo($"工作线程发生错误：{ex.Message}，断开连接。", 0);
                Disconnect();
            }
        }

        private void OnRecveTimeOut_Tick(object sender, EventArgs e)
        {
            if (!Working) return;
            ClarifyInfo($"已超过[{ReceiveTimeOut.ToString()}]没有收到数据，断开连接。", 1);
            Disconnect();
        }

        public string Name { get; }

        private void OnProtocol_Clarify(string aInfo, int aLevel)
        {
            ClarifyInfo($"Protocol clarify: {aInfo}", aLevel);
        }

        #endregion

        #region 协议实例的事件反馈

        void Protocol_ObjectFrameReceived(object aObject)
        {
            ClarifyFrameReceived(aObject);
        }

        void Protocol_StringFrameReceived(string aFrame)
        {
            ClarifyFrameReceived(aFrame);
        }

        void Protocol_ByteFrameReceived(byte[] aFrame)
        {
            ClarifyFrameReceived(aFrame);
        }

        #endregion

        #region 操作界面

        /// <summary>
        /// 指定所使用的TcpClient对象，启动工作线程
        /// </summary>
        /// <param name="aTcpClient">要使用的TcpClient对象，可由TcpListener的AcceptTcpClient操作创建。</param>
        public void WorkWith(TcpClient aTcpClient)
        {
            // 检查是否已经存在TcpClient和NetworkStream
            if (_TcpClient != null) throw new ApplicationException("已经存在一个TcpClient！");
            if (_NetworkStream != null) throw new ApplicationException("已经存在一个网络流！");

            // 检查指定TcpClient对象
            if (aTcpClient == null || !aTcpClient.Connected) throw new ApplicationException("指定TcpClient无效或未连接！");

            // 创建网络工作流
            _TcpClient = aTcpClient;
            _NetworkStream = aTcpClient.GetStream();
            if (!_NetworkStream.CanRead || !_NetworkStream.CanWrite) throw new ApplicationException("网络数据流不可读或不可写！");

            RemoteHost = System.Net.IPAddress.Parse(((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Address.ToString()).ToString();
            RemotePort = ((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Port;
            LocalPort = ((System.Net.IPEndPoint)_TcpClient.Client.LocalEndPoint).Port;

            // 通讯协议进入预备状态
            if (Protocol != null) Protocol.Start();

            // 开始读取线程
            _NetworkStream.BeginRead(_ReceiveBuffer, 0, _ReceiveBuffer.Length, _ReadCallback, null);
            if (ReceiveTimeOut > TimeSpan.Zero)
            {
                _ReceiveTimeOutTimer.Interval = ReceiveTimeOut;
                _ReceiveTimeOutTimer.Start();
            }
            if (HeartBeatInterval > TimeSpan.Zero)
            {
                _HeartbeatTimer.Interval = HeartBeatInterval;
                _HeartbeatTimer.Start();
            }
            Working = true;
            Connected?.Invoke(this, null);
            OnPropertyChanged(nameof(IsConnected));
            _Dispatcher.Invoke(() => AllConnections.Add(this));
        }

        /// <summary>
        /// 指定要连接的远程主机地址和端口
        ///     创建相应的TcpClient对象，启动工作线程
        /// </summary>
        /// <param name="aHost">远程主机</param>
        /// <param name="aPort">远程端口</param>
        /// <returns></returns>
        public void WorkWith(string aHost, int aPort)
        {
            // 检查是否已经存在工作线程
            if (_TcpClient != null) throw new System.ApplicationException("已经存在一个TcpClient！");
            if (_NetworkStream != null) throw new System.ApplicationException("已经存在一个网络流！");

            WorkWith(new System.Net.Sockets.TcpClient(aHost, aPort));
        }

        /// <summary>
        /// 发送字节块
        /// </summary>
        /// <param name="aBytes">待发送字节块</param>
        public void SendBytes(byte[] aBytes)
        {
            if (aBytes == null || aBytes.Length == 0) return;
            SendBytes(aBytes, aBytes.Length);
        }

        /// <summary>
        /// 发送字节块
        /// </summary>
        /// <param name="aBytes">待发送字节块</param>
        /// <param name="aCount">待发送字节数</param>
        public void SendBytes(byte[] aBytes, int aCount)
        {
            if (aBytes == null || aBytes.Length == 0 || aCount <= 0) return;
            SendBytes(aBytes, 0, aCount);
        }

        /// <summary>
        /// 发送字节块
        /// </summary>
        /// <param name="aBytes">待发送字节块</param>
        /// <param name="aStartIndex">待发送的起始下标</param>
        /// <param name="aCount">待发送字节数</param>
        public void SendBytes(byte[] aBytes, int aStartIndex, int aCount)
        {
            if (aBytes == null || aStartIndex >= aBytes.Length || aStartIndex + aCount > aBytes.Length) return;
            _NetworkStream.Write(aBytes, aStartIndex, aCount);
            _NetworkStream.Flush();
        }

        public void SendText(string aText, Encoding aEncoding)
        {
            if (aText == null) return;
            SendBytes(aEncoding.GetBytes(aText));
        }

        /// <summary>
        /// 断开连接，停止接收数据
        /// </summary>
        public void Disconnect()
        {
            StopWorkThread();
            CloseNetworkStream();
            CloseTcpClient();
            Disconnected?.Invoke(this, null);
            OnPropertyChanged(nameof(IsConnected));
            _Dispatcher.Invoke(() => AllConnections.Remove(this));
        }

        #endregion

        #region 事件定义和操作

        /// <summary>
        /// 发送运行时信息
        /// </summary>
        /// <param name="aInfo">信息内容</param>
        /// <param name="aLevel">信息级别</param>
        private void ClarifyInfo(string aInfo, int aLevel)
        {
            Clarify?.Invoke(this, $"{this}{aInfo}", aLevel);
        }
        public delegate void ClarifyDelegate(Connection aConnection, string aInfo, int aLevel);
        public event ClarifyDelegate Clarify;

        /// <summary>
        /// 通知收到字节块
        /// </summary>
        /// <param name="aBytes">收到的字节块</param>
        private void ClarifyBytesReceived(byte[] aBytes)
        {
            Received?.Invoke(this, aBytes);
        }
        public delegate void ReceivedDelegate(Connection aConnection, byte[] aBytes);
        public event ReceivedDelegate Received;

        /// <summary>
        /// 通知收到数据帧
        /// </summary>
        /// <param name="aText">收到的数据帧</param>
        private void ClarifyFrameReceived(string aText)
        {
            TextFrameReceived?.Invoke(this, aText);
        }
        public event System.Action<Connection, string> TextFrameReceived;

        /// <summary>
        /// 通知收到数据帧
        /// </summary>
        /// <param name="aBytes">收到的数据帧</param>
        private void ClarifyFrameReceived(byte[] aBytes)
        {
            BytesFrameReceived?.Invoke(this, aBytes);
        }
        public event System.Action<Connection, byte[]> BytesFrameReceived;

        private void ClarifyFrameReceived(object aObject)
        {
            ObjectFrameReceived?.Invoke(this, aObject);
        }
        public event System.Action<Connection, object> ObjectFrameReceived;

        public event System.EventHandler Connected;
        public event System.EventHandler Disconnected;

        #endregion

        #region 公开属性

        /// <summary>
        /// 工作线程在空闲时的扫描时间间隔（单位：毫秒）。
        /// 此值设得越长，对系统影响越小，但实时响应效果会越差。
        /// </summary>
        public int IdleInterval { get { return _IdleInterval; } set { SetValue(ref _IdleInterval, value, nameof(IdleInterval)); } }
        private int _IdleInterval = 10;

        /// <summary>
        /// 两次数据接收的最小时间间隔，超过指定时间间隔没有收到数据，认为连接已失效，自动断开连接。
        /// 接收超时如果设为0，则即使没有收到数据，也不会自动断开连接。
        /// </summary>
        public TimeSpan ReceiveTimeOut { get { return _ReceiveTimeOut; } set { SetValue(ref _ReceiveTimeOut, value, nameof(ReceiveTimeOut)); } }
        private TimeSpan _ReceiveTimeOut = TimeSpan.Zero;

        /// <summary>
        /// 关闭连接时，等待工作线程结束的时间。
        /// </summary>
        public int ExitTimeoutInterval { get { return _ExitTimeoutInterval; } set { SetValue(ref _ExitTimeoutInterval, value, nameof(ExitTimeoutInterval)); } }
        private int _ExitTimeoutInterval = 2000;

        /// <summary>
        /// 发送心跳的时间间隔，默认为2秒。
        /// </summary>
        public TimeSpan HeartBeatInterval { get { return _HeartBeatInterval; } set { SetValue(ref _HeartBeatInterval, value, nameof(HeartBeatInterval)); } }
        private TimeSpan _HeartBeatInterval = TimeSpan.FromSeconds(2);

        /// <summary>
        /// 为接收线程指定接收缓冲区的大小，不小于1k，默认为10k
        /// </summary>
        public int ReceiveBufferLength { get { return _ReceiveBufferLength; } set { SetValue(ref _ReceiveBufferLength, value, nameof(ReceiveBufferLength)); } }
        private int _ReceiveBufferLength = 10240;

        /// <summary>
        /// 远程主机名，如果连接未建立，会导致异常
        /// </summary>
        public string RemoteHost { get { return _RemoteHost; } private set { SetValue(ref _RemoteHost, value, nameof(RemoteHost)); } }
        private string _RemoteHost = "localhost";

        /// <summary>
        /// 远程端口，如果连接未建立，会导致异常
        /// </summary>
        public int RemotePort { get { return _RemotePort; } private set { SetValue(ref _RemotePort, value, nameof(RemotePort)); } }
        private int _RemotePort = 0;

        public int LocalPort { get { return _LocalPort; } private set { SetValue(ref _LocalPort, value, nameof(LocalPort)); } }
        private int _LocalPort = 0;
        
        /// <summary>
        /// 连接描述串
        /// </summary>
        /// <returns>连接描述串</returns>
        public override string ToString() => $"[{Name}:{RemoteHost}:{RemotePort}-local:{LocalPort}]";

        /// <summary>
        /// 检测连接是否建立
        /// </summary>
        public bool IsConnected => _TcpClient != null && _TcpClient.Connected;

        public Protocol Protocol { get; }

        #endregion

        #region 服务线程

        /// <summary>
        /// 数据接收线程
        ///     以轮询方式接收字节流。
        /// </summary>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                if (!Working) return;
                int aReadBytesCount = _NetworkStream.EndRead(ar);
                if (aReadBytesCount > 0)
                {
                    // ClarifyInfo($"收到[{aReadBytesCount}]字节的数据……", 1);
                    byte[] aCopy = new byte[aReadBytesCount];
                    System.Array.Copy(_ReceiveBuffer, aCopy, aReadBytesCount);

                    // 通知收到数据
                    ClarifyBytesReceived(aCopy);

                    // 协议实例分析数据
                    if (Protocol != null) Protocol.OnBytesReceived(aCopy);
                    _NetworkStream.BeginRead(_ReceiveBuffer, 0, _ReceiveBuffer.Length, _ReadCallback, null);
                }
                else throw new ApplicationException("连接中断");
            }
            catch (ApplicationException ex)
            {
                ClarifyInfo($"{ex.Message}，连接将断开。", 0);
                Disconnect();
            }
            catch (IOException ex)
            {
                ClarifyInfo($"{ex.Message}，连接将断开。", 0);
                Disconnect();
            }
            catch (Exception ex)
            {
                ClarifyInfo($"{ex.Message}，连接将断开。", 0);
                ClarifyInfo(ex.StackTrace, 9);
                Disconnect();
            }
        }

        #endregion

        #region 内部对象

        private TcpClient _TcpClient = null;
        private NetworkStream _NetworkStream = null;
        private byte[] _ReceiveBuffer;
        private readonly AsyncCallback _ReadCallback;
        private DispatcherTimer _ReceiveTimeOutTimer;
        private DispatcherTimer _HeartbeatTimer;

        private bool Working
        {
            get { return _Working; }
            set { _Working = value; }
        }
        private volatile bool _Working = true;

        #endregion

        #region 内部操作

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseTcpClient()
        {
            if (_TcpClient != null)
            {
                try
                {
                    ClarifyInfo($"关闭TcpClient……", 1);
                    _TcpClient.Close();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo($"关闭TcpClient发生错误：{ex.Message}", 0);
                }
                finally
                {
                    _TcpClient = null;
                }
            }
        }

        /// <summary>
        /// 关闭网络流
        /// </summary>
        private void CloseNetworkStream()
        {
            if (_NetworkStream != null)
            {
                try
                {
                    ClarifyInfo($"关闭网络流……", 1);
                    _NetworkStream.Close();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo($"关闭网络流发生错误：{ex.Message}", 0);
                }
                finally
                {
                    _NetworkStream = null;
                }
            }
        }

        /// <summary>
        /// 结束工作线程
        /// </summary>
        private void StopWorkThread()
        {
            // 发送停止标志，通知工作线程结束
            Working = false;
            _ReceiveTimeOutTimer.Stop();
            _HeartbeatTimer.Stop();
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 显式撤销连接对象
        /// </summary>
        public void Dispose()
        {
            if (IsConnected) Disconnect();
        }

        #endregion

        #region 已建立的连接实例集合

        private static Dispatcher _Dispatcher = Dispatcher.CurrentDispatcher;
        public static void SetUIDispatcher(Dispatcher aDispatcher) => _Dispatcher = aDispatcher;
        public static ObservableCollection<Connection> AllConnections { get; } = new ObservableCollection<Connection>();

        #endregion
    }
}
