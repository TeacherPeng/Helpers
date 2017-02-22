using PengSW.NotifyPropertyChanged;
using System;
using System.Collections.Generic;

namespace PengSW.TcpService
{
    /// <summary>
    /// 监听类，负责监听指定的端口，响应连接请求。
    /// </summary>
    public class Listener : NotifyPropertyChangedObject
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        ///     创建Listener时，须指定要监听的端口以及准备使用的通讯协议的协议工厂。
        /// </summary>
        /// <param name="aListenPort">要监听的端口</param>
        /// <param name="aProtocolFactory">准备使用的通讯协议的协议工厂</param>
        public Listener(string aName, int aListenPort, IProtocolFactory aProtocolFactory)
        {
            Name = aName;
            ListenPort = aListenPort;
            _ProtocolFactory = aProtocolFactory;
            _ListenCallback = new AsyncCallback(ListenCallback);
        }

        #endregion

        #region 操作界面

        /// <summary>
        /// 启动监听
        ///     创建监听线程，启动监听。
        /// </summary>
        public void StartListen()
        {
            if (IsListening) return;

            // 创建TcpListener
            ClarifyInfo($"创建TcpListener，监听端口[{ListenPort}]……", 1);
            _TcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, ListenPort);

            // 启动监听
            try
            {
                ClarifyInfo($"启动监听端口[{ListenPort}]……", 1);
                IsListening = true;
                _TcpListener.Start();
            }
            catch (System.Exception ex)
            {
                ClarifyInfo(string.Format("启动监听端口[{0}]发生错误：{1}", ListenPort, ex.Message), 0);
                IsListening = false;
                return;
            }

            // 主工作循环，等待并处理连接请求
            ClarifyInfo(string.Format("开始监听端口[{0}]的主工作过程，等待连接请求……", ListenPort), 1);
            _TcpListener.BeginAcceptTcpClient(_ListenCallback, null);
            IsListening = true;
        }

        /// <summary>
        /// 停止监听，并清理所建立的所有连接。
        /// </summary>
        public void StopListen()
        {
            // 停止侦听
            IsListening = false;
            ClarifyInfo($"停止端口[{ListenPort}]上的侦听……", 1);
            
            // 关闭侦听
            if (_TcpListener != null)
            {
                try
                {
                    ClarifyInfo(string.Format("关闭对端口[{0}]的侦听……", ListenPort), 1);
                    _TcpListener.Stop();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("关闭对端口[{1}]的侦听发生错误：{0}", ex.Message, ListenPort), 0);
                }
                finally
                {
                    _TcpListener = null;
                }
            }

            // 关闭连接
            foreach (Connection aConnection in _Connections)
            {
                try
                {
                    ClarifyInfo($"关闭与{this}的连接……", 1);
                    aConnection.Disconnected -= Connection_Disconnected;    // 禁止连接发送断开消息，避免集合发生改变
                    aConnection.Disconnect();
                    Disconnected?.Invoke(this, aConnection);  // 已禁止断开响应，手动发出断开通知事件。
                    aConnection.Dispose();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo($"关闭与{this}的连接发生错误：{ex.Message}", 0);
                }
            }
            _Connections.Clear();
            OnPropertyChanged(nameof(ConnectionCount));
            ClarifyInfo(string.Format("关闭通过端口[{0}]建立的连接完成。", ListenPort), 1);
        }

        /// <summary>
        /// 向所有的连接广播
        /// </summary>
        /// <param name="aBytes">广播内容</param>
        public void SayToAll(byte[] aBytes)
        {
            foreach (Connection aConnection in _Connections)
            {
                if (aConnection.IsConnected)
                {
                    try
                    {
                        aConnection.SendBytes(aBytes);
                    }
                    catch (System.Exception ex)
                    {
                        ClarifyInfo(ex.Message, 0);
                    }
                }
            }
        }

        #endregion

        #region 公开属性

        public string Name { get; }

        public int ListenPort { get; }

        /// <summary>
        /// 标志当前是否处于监听状态
        ///     外部只能读取此状态，不能设置此状态。
        ///     必须通过StartListen操作来启动监听，通过StopListen操作来停止监听。
        /// </summary>
        public bool IsListening { get { return _IsListening; } private set { SetValue(ref _IsListening, value, nameof(IsListening)); } }
        private bool _IsListening = false;

        /// <summary>
        /// 工作线程在空闲时的扫描时间间隔。
        ///     此值设得越长，对系统影响越小，但实时响应效果会越差。
        /// </summary>
        public int IdleInterval { get { return _IdleInterval; } set { SetValue(ref _IdleInterval, value, nameof(IdleInterval)); } }
        private int _IdleInterval = 500;

        /// <summary>
        /// 关闭连接时，等待工作线程结束的时间。
        /// </summary>
        public int ExitTimeoutInterval { get { return _ExitTimeoutInterval; } set { SetValue(ref _ExitTimeoutInterval, value, nameof(ExitTimeoutInterval)); } }
        private int _ExitTimeoutInterval = 2000;

        /// <summary>
        /// 已建立连接数
        /// </summary>
        public int ConnectionCount => _Connections.Count;

        #endregion

        #region 服务线程

        private void ListenCallback(IAsyncResult ar)
        {
            if (!IsListening) return;
            try
            {
                ClarifyInfo($"监听端口[{ListenPort}]收到连接请求……", 1);
                System.Net.Sockets.TcpClient aTcpClient = _TcpListener.EndAcceptTcpClient(ar);

                // 创建Connection对象，并建立连接
                // 在连接建立之前，没有发现方法来识别请求者，这意味着在连接之前不可能区分请求者，因而也不可能根据不同的请求者建立不同性质的连接。
                // 将连接能否建立以及如何建立交托给Connection类处理。
                Connection aConnection = new Connection(_ProtocolFactory == null ? null : _ProtocolFactory.CreateProtocol(), _ProtocolFactory == null ? null : _ProtocolFactory.CreateName());
                aConnection.Clarify += new Connection.ClarifyDelegate(Connection_Clarify);
                aConnection.Received += new Connection.ReceivedDelegate(Clarify_Received);
                aConnection.BytesFrameReceived += new System.Action<Connection, byte[]>(Connection_BytesFrameReceived);
                aConnection.TextFrameReceived += new System.Action<Connection, string>(Connection_TextFrameReceived);
                aConnection.ObjectFrameReceived += new System.Action<Connection, object>(Connection_ObjectFrameReceived);
                aConnection.Disconnected += new System.EventHandler(Connection_Disconnected);
                try
                {
                    aConnection.WorkWith(aTcpClient);
                    // 如果连接被接纳，则添加到连接表中，并发出通知。
                    ClarifyInfo($"端口[{ListenPort}]上的连接请求已建立：{aConnection}。", 9);
                    _Connections.Add(aConnection);
                    Connected?.Invoke(this, aConnection);
                    OnPropertyChanged(nameof(ConnectionCount));
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("接受端口[{0}]上的连接请求发生错误：{1}", ListenPort, ex.Message), 0);

                    // 如果连接被拒绝，则断开连接客户端。
                    aTcpClient.Close();
                    aConnection.Dispose();
                }
                if (IsListening) _TcpListener.BeginAcceptTcpClient(_ListenCallback, null);
            }
            catch (System.Exception ex)
            {
                ClarifyInfo($"端口[{ListenPort}]的连接请求处理发生错误：{ex.Message}", 0);
            }
        }

        void Connection_ObjectFrameReceived(Connection aConnection, object aObject)
        {
            ObjectFrameReceived?.Invoke(aConnection, aObject);
        }

        void Connection_Disconnected(object sender, System.EventArgs e)
        {
            _Connections.Remove(sender as Connection);
            Disconnected?.Invoke(this, sender as Connection);
            OnPropertyChanged(nameof(ConnectionCount));
        }

        void Connection_TextFrameReceived(Connection aConnection, string aText)
        {
            TextFrameReceived?.Invoke(aConnection, aText);
        }

        void Connection_BytesFrameReceived(Connection aConnection, byte[] aBytes)
        {
            BytesFrameReceived?.Invoke(aConnection, aBytes);
        }

        #endregion

        #region 操作通知

        public event System.Action<PengSW.TcpService.Connection, string> TextFrameReceived;
        public event System.Action<PengSW.TcpService.Connection, byte[]> BytesFrameReceived;
        public event System.Action<PengSW.TcpService.Connection, object> ObjectFrameReceived;

        public delegate void ConnectedDelegate(PengSW.TcpService.Listener aListener, PengSW.TcpService.Connection aConnection);
        public event ConnectedDelegate Connected;

        public delegate void DisconnectedDelegate(PengSW.TcpService.Listener aListener, PengSW.TcpService.Connection aConnection);
        public event DisconnectedDelegate Disconnected;

        public delegate void ClarifyDelegate(string aInfo, int aLevel);
        public event ClarifyDelegate Clarify;

        /// <summary>
        /// 发送运行时信息
        /// </summary>
        /// <param name="aInfo">信息内容</param>
        /// <param name="aLevel">信息级别</param>
        private void ClarifyInfo(string aInfo, int aLevel)
        {
            Clarify?.Invoke(aInfo, aLevel);
        }

        private void Connection_Clarify(Connection aConnection, string aInfo, int aLevel)
        {
            ClarifyInfo(aInfo, aLevel);
        }

        public delegate void ReceivedDelegate(Connection aConnection, byte[] aBytes);
        public event ReceivedDelegate Received;

        private void Clarify_Received(Connection aConnection, byte[] aBytes)
        {
            Received?.Invoke(aConnection, aBytes);
        }

        #endregion

        #region 内部对象

        private System.Net.Sockets.TcpListener _TcpListener = null;
        private AsyncCallback _ListenCallback;

        private List<Connection> _Connections = new List<Connection>();

        private IProtocolFactory _ProtocolFactory = null;

        #endregion
    }
}
