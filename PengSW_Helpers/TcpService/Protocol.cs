using System;

namespace PengSW.TcpService
{
    /// <summary>
    /// 通讯协议基类
    ///     数据按帧为单位进行处理，通讯协议负责将一组字节块解析成一组数据帧。
    ///     基本概念：
    ///         数据片(Chip)：每次收到的一个字节块称为一个数据片。
    ///     默认规则为：
    ///     1、输入为数据片流，输出为数据帧流；
    ///     2、帧起始不一定在数据片的起始位置；
    ///     3、一帧可能分割到一组连续的数据片中；
    ///     4、一个数据片可能包含多个帧；
    ///     默认策略为：
    ///         在预备状态时，等待含有帧起始的数据片，然后进入接收状态；
    ///         在接收状态下，若收到一个完整帧，则解析出该帧；若提取出数据帧后，没有新的帧起始，则清空接收存储，进入预备状态；
    ///     实现类需要实现的操作概念：
    ///         数据片是否含有帧起始；
    ///         数据片是否含有帧结束；
    ///     建议协议类只负责帧的解析，不负责帧的处理，通过通知事件将收到的帧发送出去，由其他专门负责处理的对象来处理。
    /// </summary>
    public abstract class Protocol : IDisposable
    {
        #region 初始化
        
        public Protocol()
        {
            FrameMaxBytes = 4096 * 1024;
            FrameTimeOut = TimeSpan.Zero;
            ReceiveTimeOut = TimeSpan.Zero;
        }

        /// <summary>
        /// 由于Protocol实例中会保存收到的数据，因此一个Protocol实例不能被多个Connection实例共用。（更合理的设计应该是分成协议类和协议数据类，将协议与收到的数据分开）
        /// 因此Protocol实例应该与所属的Connection实例关联，但由于Protocol实例是构造Connection实例的参数，因此不能在Protocol的构造参数中指定所属的Connection实例。
        /// 约定Connection实例在构造时，需要将自身关联到Protocol实例中。
        /// </summary>
        protected Connection _Connection;
        public void SetConnection(Connection aConnection)
        {
            if (_Connection == null) _Connection = aConnection;
        }

        #endregion

        #region IDisposable 成员

        public virtual void Dispose()
        {
            _ByteBuffer.Dispose();
        }

        #endregion

        #region 操作界面

        /// <summary>
        /// 进入预备态，准备开始处理通讯数据。
        /// </summary>
        public void Start()
        {
            HeadReceived = false;       // 标记进入预备状态
            OnStart();                  // 调用扩展操作，子类可以通过派生此函数来定义扩展操作。
        }

        /// <summary>
        /// 通知接收到一个数据片
        /// </summary>
        /// <param name="aConnection">发出通知的连接</param>
        /// <param name="aBytes">接收到的数据片</param>
        public void OnBytesReceived(byte[] aBytes)
        {
            // 超时检查，如果超时则将帧首标志复位，准备重新接收帧
            System.DateTime aReceivedTime = System.DateTime.Now;
            if (HeadReceived && FrameTimeOut > TimeSpan.Zero && aReceivedTime - _ReceivedTime > FrameTimeOut)
            {
                ClarifyInfo($"收到数据时间[{aReceivedTime:HH:mm:ss}]距上次收到数据时间[{_ReceivedTime:HH:mm:ss}]已超过帧超时设置[{FrameTimeOut:hh\\:mm\\:ss}]，将重新开始接收帧。");
                HeadReceived = false;
            }
            _ReceivedTime = aReceivedTime;

            // 如果在预备状态收到帧起始，则进入接收状态
            if (!HeadReceived && HasFrameHead(aBytes))
            {
                // 清空接收缓冲区，准备开始接收数据
                _ByteBuffer.ClearBuffer();
                HeadReceived = true;
            }

            // 在接收状态下，接收此数据片，并判断是否已到帧结束
            if (HeadReceived)
            {
                // 接收此数据片
                _ByteBuffer.SaveBuffer(aBytes);

                // 如果出现帧结束片，则帧接收已完整
                if (HasFrameTail(_ByteBuffer.LastBytes))
                {
                    // 分析收到的包含完整数据帧的字节块
                    AnalyBytes(_ByteBuffer.TotalBytes);

                    // 复位接收标志，准备接收新的数据帧
                    HeadReceived = false;
                }
                else if (FrameMaxBytes > 0 && _ByteBuffer.TotalByteCount > FrameMaxBytes)
                {
                    // 如果已超过帧字节数上限还没有收到帧尾，则复位接收标志，准备重新开始接收。
                    ClarifyInfo($"已收到[{_ByteBuffer.TotalByteCount}]字节的数据，超过帧最大字节数设置[{FrameMaxBytes}]，将丢弃已接收的数据，重新开始接收。");
                    HeadReceived = false;
                }
            }
        }

        public virtual byte[] HeartBeat()
        {
            return new byte[0];
        }

        #endregion

        #region 事件定义及操作

        public event Action<string> StringFrameReceived;
        public event Action<byte[]> ByteFrameReceived;
        public event Action<object> ObjectFrameReceived;
        public event Action<string, int> Clarify;

        protected void ClarifyFrameReceived(string aFrame)
        {
            StringFrameReceived?.Invoke(aFrame);
        }
        protected void ClarifyFrameReceived(byte[] aFrame)
        {
            ByteFrameReceived?.Invoke(aFrame);
        }
        protected void ClarifyObjectReceived(object aObject)
        {
            ObjectFrameReceived?.Invoke(aObject);
        }
        protected void ClarifyInfo(string aInfo, int aLevel = 4)
        {
            Clarify?.Invoke(aInfo, aLevel);
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 一帧最多占用字节数
        ///     如果从帧起始数据块开始，超过指定最大字节数还没有收到帧结尾数据块，将视为数据错误。
        ///     如果将最大字节数设为0，意味着不限制帧的字节数。
        ///     默认为4M。
        /// </summary>
        public uint FrameMaxBytes { get; set; } = 4096 * 1024;

        /// <summary>
        /// 组成帧的字节块接收到的时间的最大间隔（单位为毫秒）。
        ///     如果前后两个字节块接收到的时间间隔超过此值，即使还没有收到帧尾字节块，
        ///     也认为已出现故障，之前收到的字节块将被放弃。
        ///     帧超时设为0，表示不判断帧超时。
        ///     默认不判断帧超时。
        /// </summary>
        public TimeSpan FrameTimeOut { get; set; }

        /// <summary>
        /// 最长允许静默时间，当超过指定时间没有收到数据时，认为连接已停用，自动断开连接。如果将允许静默时间设为0，则表示即使没有收到数据，也不自动断开连接。默认为0。
        /// </summary>
        public TimeSpan ReceiveTimeOut { get; set; }

        #endregion

        #region 虚操作

        /// <summary>
        /// 实现类通过实现此操作，指定进入预备态所需要做的工作。
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// 判断指定数据片中是否含有帧起始
        /// </summary>
        /// <param name="aBytes">待判断的数据片</param>
        /// <returns>若数据片含有帧起始，返回true</returns>
        protected abstract bool HasFrameHead(byte[] aBytes);

        /// <summary>
        /// 判断指定数据片中是否含有帧结尾
        /// </summary>
        /// <param name="aBytes">待判断的数据片</param>
        /// <returns>若数据片含有帧结尾，返回true</returns>
        protected abstract bool HasFrameTail(byte[] aBytes);

        /// <summary>
        /// 分析一个包含完整的数据帧的字节块
        /// </summary>
        /// <param name="aConnection">调用分析方法的连接，可用来发送回复信息</param>
        /// <param name="aBytes">待分析字节块</param>
        /// <returns>如果不能正常接收此字节块，则返回false</returns>
        protected abstract bool AnalyBytes(byte[] aBytes);

        #endregion

        #region 内部属性

        /// <summary>
        /// 标志是否已经收到帧起始块
        /// </summary>
        protected bool HeadReceived { get; set; }

        #endregion

        #region 内部对象

        /// <summary>
        /// 存储接收到的字节块的容器
        /// </summary>
        protected ByteBuffer _ByteBuffer = new ByteBuffer();

        /// <summary>
        /// 记录收到数据块的时间，用来判断帧超时
        /// </summary>
        protected DateTime _ReceivedTime = DateTime.MinValue;

        #endregion
    }
}
