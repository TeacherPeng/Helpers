using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace PengSW.TcpService
{
    /// <summary>
    /// 连接类，负责一个Tcp连接，提供以轮询方式接收字节流。
    /// </summary>
    public class Connection : IDisposable, INotifyPropertyChanged
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        ///     构造时指定准备使用的通讯协议。
        /// </summary>
        /// <param name="aProtocol">准备使用的通讯协议实例</param>
        public Connection(Protocol aProtocol)
        {
            // 指定默认属性值
            IdleInterval = 10;
            ReceiveTimeOut = TimeSpan.Zero;
            ExitTimeoutInterval = 2000;
            HeartBeatInterval = TimeSpan.FromSeconds(2);

            // 保存准备使用的通讯协议
            _Protocol = aProtocol;
            if (_Protocol != null)
            {
                _Protocol.ByteFrameReceived += new System.Action<byte[]>(Protocol_ByteFrameReceived);
                _Protocol.StringFrameReceived += new System.Action<string>(Protocol_StringFrameReceived);
                _Protocol.ObjectFrameReceived += new System.Action<object>(Protocol_ObjectFrameReceived);
                _Protocol.Clarify += OnProtocol_Clarify;
                ReceiveTimeOut = _Protocol.ReceiveTimeOut;
            }

            AllConnections.Add(this);
        }

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
        public void WorkWith(System.Net.Sockets.TcpClient aTcpClient)
        {
            // 检查是否已经存在工作线程
            if (_TcpClient != null) throw new System.ApplicationException("已经存在一个TcpClient！");
            if (_NetworkStream != null) throw new System.ApplicationException("已经存在一个网络流！");
            if (m_WorkThread != null) throw new System.ApplicationException("已经存在一个工作线程！");

            // 检查指定TcpClient对象
            if (aTcpClient == null || !aTcpClient.Connected) throw new System.ApplicationException("指定TcpClient无效或未连接！");

            // 创建网络工作流
            _TcpClient = aTcpClient;
            _NetworkStream = aTcpClient.GetStream();
            if (!_NetworkStream.CanRead || !_NetworkStream.CanWrite) throw new System.ApplicationException("网络数据流不可读或不可写！");

            m_RemoteHost = System.Net.IPAddress.Parse(((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Address.ToString()).ToString();
            m_RemotePort = ((System.Net.IPEndPoint)_TcpClient.Client.RemoteEndPoint).Port;

            // 创建工作线程
            m_WorkThread = new System.Threading.Thread(new System.Threading.ThreadStart(ReceiveThread));
            m_WorkThread.IsBackground = true;

            // 启动工作线程
            Working = true;
            m_WorkThread.Start();
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
            if (m_WorkThread != null) throw new System.ApplicationException("已经存在一个工作线程！");

            WorkWith(new System.Net.Sockets.TcpClient(aHost, aPort));
            m_RemoteHost = aHost;
            m_RemotePort = aPort;
            Connected?.Invoke(this, null);
            OnPropertyChanged(nameof(IsConnected));
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
            Clarify?.Invoke(this, string.Format("{0} {1}", ToString(), aInfo), aLevel);
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

        protected void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region 公开属性

        /// <summary>
        /// 工作线程在空闲时的扫描时间间隔。
        /// 此值设得越长，对系统影响越小，但实时响应效果会越差。
        /// </summary>
        public int IdleInterval { get; set; }

        /// <summary>
        /// 两次数据接收的最小时间间隔，超过指定时间间隔没有收到数据，认为连接已失效，自动断开连接。
        /// 接收超时如果设为0，则即使没有收到数据，也不会自动断开连接。
        /// </summary>
        public TimeSpan ReceiveTimeOut { get; set; }

        /// <summary>
        /// 关闭连接时，等待工作线程结束的时间。
        /// </summary>
        public int ExitTimeoutInterval { get; set; }

        /// <summary>
        /// 发送心跳的时间间隔，默认为2秒。
        /// </summary>
        public TimeSpan HeartBeatInterval { get; set; }

        /// <summary>
        /// 为接收线程指定接收缓冲区的大小，不小于1k，默认为10k
        /// </summary>
        public int ReceiveBufferLength 
        {
            get { return m_ReceiveBufferLength; }
            set { m_ReceiveBufferLength = value < 1024 ? 1024 : value; }
        }
        private int m_ReceiveBufferLength = 10240;

        /// <summary>
        /// 远程主机名，如果连接未建立，会导致异常
        /// </summary>
        public string RemoteHost
        {
            get
            {
                return m_RemoteHost;
            }
            private set
            {
                m_RemoteHost = value;
            }
        }
        private string m_RemoteHost = "unknown";

        /// <summary>
        /// 远程端口，如果连接未建立，会导致异常
        /// </summary>
        public int RemotePort
        {
            get
            {
                return m_RemotePort;
            }
            private set
            {
                m_RemotePort = value;
            }
        }
        private int m_RemotePort = 0;

        /// <summary>
        /// 连接描述串
        /// </summary>
        /// <returns>连接描述串</returns>
        public override string ToString()
        {
            return string.Format("[Connection to {0}:{1}]", RemoteHost, RemotePort);
        }

        /// <summary>
        /// 检测连接是否建立
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _TcpClient != null && _TcpClient.Connected;
            }
        }

        #endregion

        #region 服务线程

        /// <summary>
        /// 数据接收线程
        ///     以轮询方式接收字节流。
        /// </summary>
        private void ReceiveThread()
        {
            try
            {
                ClarifyInfo(string.Format("连接[{0}:{1}]的工作线程开始工作……", m_RemoteHost, m_RemotePort), 1);

                // 通讯协议进入预备状态
                if (_Protocol != null) _Protocol.Start();

                byte[] aByteBuffer = new byte[ReceiveBufferLength];

                // 接收数据，将收到的数据片通知通讯协议处理。
                Stopwatch aReceiveStopwatch = new Stopwatch();
                Stopwatch aHeartBeatStopwatch = new Stopwatch();
                aReceiveStopwatch.Start();
                aHeartBeatStopwatch.Start();
                while (Working)
                {
                    try
                    {
                        if (_NetworkStream.DataAvailable)
                        {
                            // 以字节块为单位接收数据
                            int aReadBytesCount = _NetworkStream.Read(aByteBuffer, 0, aByteBuffer.Length);
                            if (aReadBytesCount > 0)
                            {
                                byte[] aCopy = new byte[aReadBytesCount];
                                System.Array.Copy(aByteBuffer, aCopy, aReadBytesCount);

                                // 通知收到数据
                                ClarifyBytesReceived(aCopy);

                                // 协议实例分析数据
                                if (_Protocol != null) _Protocol.ClarifyReceivedBytes(this, aCopy);
                            }
                            aReceiveStopwatch = Stopwatch.StartNew();
                        }
                        else
                        {
                            // 没有数据要接收时，暂时交出控制权，以减少对系统响应的影响。
                            System.Threading.Thread.Sleep(IdleInterval);
                            if (ReceiveTimeOut > TimeSpan.Zero && aReceiveStopwatch.Elapsed > ReceiveTimeOut)
                            {
                                ClarifyInfo($"已超过[{ReceiveTimeOut.ToString()}]没有收到数据，断开连接。", 1);
                                Disconnect();
                                break;
                            }
                            else
                            {
                                // 发送心跳，检查连接状态
                                try
                                {
                                    byte[] aBytes = _Protocol == null ? new byte[0] : _Protocol.HeartBeat();
                                    if (aBytes.Length == 0 || aHeartBeatStopwatch.Elapsed >= HeartBeatInterval)
                                    {
                                        _TcpClient.Client.Send(aBytes);
                                        aHeartBeatStopwatch = Stopwatch.StartNew();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClarifyInfo(string.Format("连接[{0}:{1}]的工作线程发生错误：{2}，断开连接。", m_RemoteHost, m_RemotePort, ex.Message), 0);
                                    Disconnect();
                                    break;
                                }
                            }
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        ClarifyInfo(ex.Message, 0);
                    }
                }
                ClarifyInfo(string.Format("连接[{0}:{1}]的工作线程停止。", m_RemoteHost, m_RemotePort), 1);
            }
            catch (System.Threading.ThreadAbortException)
            {
                // 调用ResetAbort来阻止再次引发此异常，令线程正常结束。
                ClarifyInfo(string.Format("连接[{0}:{1}]的工作线程被中止……", m_RemoteHost, m_RemotePort), 0);
                System.Threading.Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                ClarifyInfo(string.Format("连接[{0}:{1}]的工作线程发生错误：{2}", m_RemoteHost, m_RemotePort, ex.Message), 0);
                ClarifyInfo(ex.StackTrace, 4);
            }
        }

        #endregion

        #region 内部对象

        private System.Net.Sockets.TcpClient _TcpClient = null;
        private System.Net.Sockets.NetworkStream _NetworkStream = null;
        private System.Threading.Thread m_WorkThread = null;

        private Protocol _Protocol = null;

        private bool Working
        {
            get { return m_Working; }
            set { m_Working = value; }
        }
        private volatile bool m_Working = true;

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
                    ClarifyInfo(string.Format("关闭连接[{0}:{1}]的TcpClient……", m_RemoteHost, m_RemotePort), 1);
                    _TcpClient.Close();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("关闭连接[{0}:{1}]的TcpClient发生错误：{2}", m_RemoteHost, m_RemotePort, ex.Message), 0);
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
                    ClarifyInfo(string.Format("关闭连接到[{0}:{1}]的网络流……", m_RemoteHost, m_RemotePort), 1);
                    _NetworkStream.Close();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("关闭连接到[{0}:{1}]的网络流发生错误：{2}", m_RemoteHost, m_RemotePort, ex.Message), 0);
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

            // 结束工作线程
            if (m_WorkThread != null)
            {
                try
                {
                    ClarifyInfo(string.Format("停止连接到[{0}:{1}]的工作线程……", m_RemoteHost, m_RemotePort), 1);

                    // 等待工作线程正常结束
                    m_WorkThread.Join(ExitTimeoutInterval);

                    // 若不能正常结束，则强制停止工作线程
                    // if (m_WorkThread.IsAlive) m_WorkThread.Abort();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("结束连接到[{0}:{1}]的工作线程发生错误：{2}", m_RemoteHost, m_RemotePort, ex.Message), 0);
                }
                finally
                {
                    m_WorkThread = null;
                }
            }
        }

        #endregion

        #region IDisposable 成员

        /// <summary>
        /// 显式撤销连接对象
        /// </summary>
        public void Dispose()
        {
            Disconnect();
            AllConnections.Remove(this);
        }

        #endregion

        #region 已建立的连接实例集合

        public static ObservableCollection<Connection> AllConnections { get { return _AllConnections; } }
        private static ObservableCollection<Connection> _AllConnections = new ObservableCollection<Connection>();

        #endregion
    }
}
