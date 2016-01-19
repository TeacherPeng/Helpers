    最上层类为：Listener和Connetion。
    Listener提供监听服务，Connection提供一个连接的控制和管理。
    需要使用者实现的基类为：Protocol和IProtocolFcatory。
    Protocol为协议基类，使用者需要实现：
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
        /// 分析一个包含完整数据帧的字节块
		///     当接收线程收集齐一个完整的数据帧后，调用此操作处理数据帧。
        /// </summary>
        /// <param name="aBytes">待分析字节块</param>
        /// <returns>如果不能正常接收此字节块，则返回false</returns>
        protected abstract bool AnalyBytes(byte[] aBytes);

    在收到完整帧后，可以调用ClarifyFrameReceived方法发送帧通知事件，通知负责帧处理的对象。
    每个Connection实例要关联一个Protocol实例。因为一个Listener可能创建多个Connection，因此创建Listener实例时，要指定一个支持IProtocolFactory的实例，用来创建与Connection关联的Protocol实例。
    