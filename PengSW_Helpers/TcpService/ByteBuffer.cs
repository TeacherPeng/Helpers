using System;

namespace PengSW.TcpService
{
    /// <summary>
    /// 字节缓冲区类，提供以字节块为单位的缓冲区
    /// </summary>
    public class ByteBuffer : IDisposable
    {
        #region 公开属性

        /// <summary>
        /// 已记录的字节总数
        /// </summary>
        public long TotalByteCount
        {
            get { return _TotalByteCount; }
        }

        /// <summary>
        /// 构造并返回完整的字节块
        /// </summary>
        public byte[] TotalBytes
        {
            get
            {
                if (_TotalBytes != null && _TotalBytes.Length == _TotalByteCount) return _TotalBytes;
                // 将数据帧拼接成一个完整的字节块
                byte[] aTotalBytes = new byte[_TotalByteCount];
                long i = 0;
                foreach (byte[] bBytes in _BytesList)
                {
                    System.Array.Copy(bBytes, 0, aTotalBytes, i, bBytes.Length);
                    i += bBytes.Length;
                }
                _TotalBytes = aTotalBytes;
                return aTotalBytes;
            }
        }
        private byte[] _TotalBytes;

        /// <summary>
        /// 构造并返回最近收到的字节块（如果只有一块，则返回这一块，否则返回最后两块合并在一起的字节块)
        /// </summary>
        public byte[] LastBytes
        {
            get
            {
                if (_BytesList.Count == 0) return null;
                if (_BytesList.Count == 1) return _BytesList[0];
                byte[] aLastBytes = new byte[_BytesList[_BytesList.Count - 2].Length + _BytesList[_BytesList.Count - 1].Length];
                Array.Copy(_BytesList[_BytesList.Count - 2], aLastBytes, _BytesList[_BytesList.Count - 2].Length);
                Array.Copy(_BytesList[_BytesList.Count - 1], 0, aLastBytes, _BytesList[_BytesList.Count - 2].Length, _BytesList[_BytesList.Count - 1].Length);
                return aLastBytes;
            }
        }

        #endregion

        #region 操作界面

        /// <summary>
        /// 清空字节块表
        /// </summary>
        public void ClearBuffer()
        {
            _BytesList.Clear();
            _TotalByteCount = 0;
            _TotalBytes = null;
        }

        /// <summary>
        /// 保存指定的字节块到字节块表中
        /// </summary>
        /// <param name="aBytes">待保存的字节块</param>
        public void SaveBuffer(byte[] aBytes)
        {
            _BytesList.Add(aBytes);
            _TotalByteCount += aBytes.Length;
        }

        #endregion

        #region 内部对象定义

        /// <summary>
        /// 字节块表，用来保存得到的一组字节块。
        /// </summary>
        private System.Collections.Generic.List<byte[]> _BytesList = new System.Collections.Generic.List<byte[]>();

        /// <summary>
        /// 字节块表中各字节块的字节总数。
        /// </summary>
        private long _TotalByteCount = 0;
        
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            ClearBuffer();
        }

        #endregion
    }
}
