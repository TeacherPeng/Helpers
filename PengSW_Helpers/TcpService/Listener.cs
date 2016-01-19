using System.Collections.Generic;

namespace PengSW.TcpService
{
    /// <summary>
    /// 监听类，负责监听指定的端口，响应连接请求。
    /// </summary>
    public class Listener
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        ///     创建Listener时，须指定要监听的端口以及准备使用的通讯协议的协议工厂。
        /// </summary>
        /// <param name="aListenPort">要监听的端口</param>
        /// <param name="aProtocolFactory">准备使用的通讯协议的协议工厂</param>
        public Listener(int aListenPort, IProtocolFactory aProtocolFactory)
        {
            ListenPort = aListenPort;
            IsListening = false;
            m_ProtocolFactory = aProtocolFactory;
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
            ClarifyInfo(string.Format("创建TcpListener，监听端口[{0}]……", ListenPort), 1);
            m_TcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, ListenPort);

            // 创建并启动ListenerThread
            ClarifyInfo(string.Format("启动侦听线程，监听端口[{0}]……", ListenPort), 1);
            m_ListenerThread = new System.Threading.Thread(new System.Threading.ThreadStart(ListenThread));
            m_ListenerThread.IsBackground = true;
            m_ListenerThread.Start();
            
            IsListening = true;
        }

        /// <summary>
        /// 停止监听，并清理所建立的所有连接。
        /// </summary>
        public void StopListen()
        {
            IsListening = false;

            // 停止侦听线程
            if (m_ListenerThread != null)
            {
                try
                {
                    ClarifyInfo(string.Format("停止端口[{0}]上的侦听线程……", ListenPort), 1);
                    m_ListenerThread.Join(ExitTimeoutInterval);
                    if (m_ListenerThread.IsAlive) m_ListenerThread.Abort();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("停止端口[{1}]上的侦听线程发生错误：{0}", ex.Message, ListenPort), 0);
                }
                finally
                {
                    m_ListenerThread = null;
                }
            }

            // 关闭侦听
            if (m_TcpListener != null)
            {
                try
                {
                    ClarifyInfo(string.Format("关闭对端口[{0}]的侦听……", ListenPort), 1);
                    m_TcpListener.Stop();
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("关闭对端口[{1}]的侦听发生错误：{0}", ex.Message, ListenPort), 0);
                }
                finally
                {
                    m_TcpListener = null;
                }
            }

            // 关闭连接
            foreach (Connection aConnection in m_Connections)
            {
                try
                {
                    ClarifyInfo(string.Format("关闭与[{0}:{1}]的连接……", aConnection.RemoteHost, aConnection.RemotePort), 1);
                    aConnection.Disconnected -= Connection_Disconnected;    // 禁止连接发送断开消息，避免集合发生改变
                    aConnection.Disconnect();
                    ClarifyDisconnected(aConnection);   // 已禁止断开响应，手动发出断开通知事件。
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("关闭与[{1}:{2}]的连接发生错误：{0}", ex.Message, aConnection.RemoteHost, aConnection.RemotePort), 0);
                }
            }
            m_Connections.Clear();
            ClarifyInfo(string.Format("关闭通过端口[{0}]建立的连接完成。", ListenPort), 1);
        }

        /// <summary>
        /// 删除指定的连接
        /// </summary>
        /// <param name="aConnection"></param>
        public void RemoveConnection(PengSW.TcpService.Connection aConnection)
        {
            m_Connections.Remove(aConnection);
        }

        /// <summary>
        /// 向所有的连接广播
        /// </summary>
        /// <param name="aBytes">广播内容</param>
        public void SayToAll(byte[] aBytes)
        {
            foreach (PengSW.TcpService.Connection aConnection in m_Connections)
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

        /// <summary>
        /// 监听的端口
        ///     在构造时指定此端口，不得修改。
        /// </summary>
        public int ListenPort
        {
            get { return m_ListenPort; }
            private set
            {
                if (!IsListening) m_ListenPort = value;
            }
        }
        private int m_ListenPort;

        /// <summary>
        /// 标志当前是否处于监听状态
        ///     外部只能读取此状态，不能设置此状态。
        ///     必须通过StartListen操作来启动监听，通过StopListen操作来停止监听。
        /// </summary>
        public bool IsListening
        {
            get { return m_Listening; }
            private set { m_Listening = value; }
        }
        private volatile bool m_Listening;

        /// <summary>
        /// 工作线程在空闲时的扫描时间间隔。
        ///     此值设得越长，对系统影响越小，但实时响应效果会越差。
        /// </summary>
        public int IdleInterval
        {
            get { return m_IdleInterval; }
            set { m_IdleInterval = value; }
        }
        private int m_IdleInterval = 500;

        /// <summary>
        /// 关闭连接时，等待工作线程结束的时间。
        /// </summary>
        public int ExitTimeoutInterval
        {
            get { return m_ExitTimeoutInterval; }
            set { m_ExitTimeoutInterval = value; }
        }
        private int m_ExitTimeoutInterval = 2000;

        /// <summary>
        /// 已建立连接数
        /// </summary>
        public int ConnectionCount
        {
            get { return m_Connections.Count; }
        }

        #endregion

        #region 服务线程

        /// <summary>
        /// 监听线程，接受客户端的连接请求
        /// </summary>
        private void ListenThread()
        {
            if (m_TcpListener == null) return;

            // 启动监听
            try
            {
                ClarifyInfo(string.Format("启动监听端口[{0}]……", ListenPort), 1);
                IsListening = true;
                m_TcpListener.Start();
            }
            catch (System.Exception ex)
            {
                ClarifyInfo(string.Format("启动监听端口[{0}]发生错误：{1}", ListenPort, ex.Message), 0);
                IsListening = false;
                return;
            }

            // 主工作循环，等待并处理连接请求
            ClarifyInfo(string.Format("开始监听端口[{0}]的主工作过程，等待连接请求……", ListenPort), 1);
            while (IsListening)
            {
                try
                {
                    if (m_TcpListener.Pending())
                    {
                        ClarifyInfo(string.Format("监听端口[{0}]收到连接请求……", ListenPort), 1);
                        // 创建Connection对象，并建立连接
                        // 在连接建立之前，没有发现方法来识别请求者，这意味着在连接之前不可能区分请求者，因而也不可能根据不同的请求者建立不同性质的连接。
                        // 将连接能否建立以及如何建立交托给Connection类处理。
                        Connection aConnection = new Connection(m_ProtocolFactory == null ? null : m_ProtocolFactory.CreateProtocol());
                        aConnection.Clarify += new Connection.ClarifyDelegate(Connection_Clarify);
                        aConnection.Received += new Connection.ReceivedDelegate(Clarify_Received);
                        aConnection.BytesFrameReceived += new System.Action<Connection, byte[]>(Connection_BytesFrameReceived);
                        aConnection.TextFrameReceived += new System.Action<Connection, string>(Connection_TextFrameReceived);
                        aConnection.ObjectFrameReceived += new System.Action<Connection, object>(Connection_ObjectFrameReceived);
                        aConnection.Disconnected += new System.EventHandler(Connection_Disconnected);
                        System.Net.Sockets.TcpClient aTcpClient = m_TcpListener.AcceptTcpClient();
                        try
                        {
                            aConnection.WorkWith(aTcpClient);
                            // 如果连接被接纳，则添加到连接表中，并发出通知。
                            m_Connections.Add(aConnection);
                            ClarifyConnected(aConnection);
                        }
                        catch (System.Exception ex)
                        {
                            ClarifyInfo(string.Format("接受端口[{0}]上的连接请求发生错误：{1}", ListenPort, ex.Message), 0);

                            // 如果连接被拒绝，则断开连接客户端。
                            aTcpClient.Close();
                            aConnection.Dispose();
                        }
                    }
                    else
                    {
                        // 如果没有连接请求，则进入短暂的休眠状态，以减少对系统响应的影响。
                        System.Threading.Thread.Sleep(IdleInterval);
                    }
                }
                catch (System.Exception ex)
                {
                    ClarifyInfo(string.Format("监听端口[{0}]的主工作过程循环发生错误：{1}", ListenPort, ex.Message), 0);
                }
            }
        }

        void Connection_ObjectFrameReceived(Connection aConnection, object aObject)
        {
            if (ObjectFrameReceived != null) ObjectFrameReceived(aConnection, aObject);
        }

        void Connection_Disconnected(object sender, System.EventArgs e)
        {
            m_Connections.Remove((Connection)sender);
            ClarifyDisconnected((Connection)sender);
        }

        void Connection_TextFrameReceived(Connection aConnection, string aText)
        {
            if (TextFrameReceived  != null) TextFrameReceived(aConnection, aText);
        }

        void Connection_BytesFrameReceived(Connection aConnection, byte[] aBytes)
        {
            if (BytesFrameReceived != null) BytesFrameReceived(aConnection, aBytes);
        }

        #endregion

        #region 操作通知

        public event System.Action<PengSW.TcpService.Connection, string> TextFrameReceived;
        public event System.Action<PengSW.TcpService.Connection, byte[]> BytesFrameReceived;
        public event System.Action<PengSW.TcpService.Connection, object> ObjectFrameReceived;

        public delegate void ConnectedDelegate(PengSW.TcpService.Listener aListener, PengSW.TcpService.Connection aConnection);
        public event ConnectedDelegate Connected;

        private void ClarifyConnected(PengSW.TcpService.Connection aConnection)
        {
            ConnectedDelegate aTempEvent = Connected;
            if (aTempEvent!=null) aTempEvent(this, aConnection);
        }

        public delegate void DisconnectedDelegate(PengSW.TcpService.Listener aListener, PengSW.TcpService.Connection aConnection);
        public event DisconnectedDelegate Disconnected;

        private void ClarifyDisconnected(PengSW.TcpService.Connection aConnection)
        {
            DisconnectedDelegate aTempEvent = Disconnected;
            if (aTempEvent != null) aTempEvent(this, aConnection);
        }

        public delegate void ClarifyDelegate(string aInfo, int aLevel);
        public event ClarifyDelegate Clarify;

        /// <summary>
        /// 发送运行时信息
        /// </summary>
        /// <param name="aInfo">信息内容</param>
        /// <param name="aLevel">信息级别</param>
        private void ClarifyInfo(string aInfo, int aLevel)
        {
            ClarifyDelegate aTempEvent = Clarify;
            if (aTempEvent != null) aTempEvent(aInfo, aLevel);
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

        private System.Threading.Thread m_ListenerThread = null;
        private System.Net.Sockets.TcpListener m_TcpListener = null;

        private System.Collections.Generic.List<Connection> m_Connections = new List<Connection>();

        private IProtocolFactory m_ProtocolFactory = null;

        #endregion
    }
}
