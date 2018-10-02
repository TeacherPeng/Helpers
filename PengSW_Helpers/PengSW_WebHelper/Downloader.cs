using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static PengSW.RuntimeLog.RL;

namespace PengSW.WebHelper
{
    public static class Downloader
    {
        public static string QueryDownloadFileName(string aUrl)
        {
            HttpWebRequest aHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            string aFileName;
            using (HttpWebResponse aHttpWebResponse = (HttpWebResponse)aHttpWebRequest.GetResponse())
            {
                aFileName = QueryFileName(aHttpWebResponse);
                aHttpWebResponse.Close();
            }
            return aFileName;
        }

        /// <summary>
        /// 下载指定Url，并返回下载源声明的文件名。
        /// </summary>
        /// <param name="aUrl">下载目标Url</param>
        /// <param name="aFileName">返回下载源声明的文件名</param>
        /// <returns>下载得到的字节块</returns>
        public static byte[] Download(string aUrl, out string aFileName)
        {
            HttpWebRequest aHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            byte[] aBytes;
            using (HttpWebResponse aHttpWebResponse = (HttpWebResponse)aHttpWebRequest.GetResponse())
            {
                aFileName = QueryFileName(aHttpWebResponse);
                aBytes = DownloadBytes(aHttpWebResponse);
                aHttpWebResponse.Close();
            }
            return aBytes;
        }

        /// <summary>
        /// 将指定Url下载到指定本地目录下，并返回下载结果的含路径本地文件名。
        ///     本地文件名的确定优先顺序为：
        ///         首先由aTargetFileName来确定，如果未指定aTargetFileName，
        ///         则由下载返回的文件名来确定，如果下载不返回文件名，
        ///         则由aUrl来确定，如果aUrl仍不能确定，
        ///         则生成随机文件名。
        ///     本地文件名后缀的确定优先顺序为：
        ///         首先由下载返回的文件名中的后缀来确定，如果下载不返回文件名，
        ///         则由aUrl中的文件名后缀来确定，如果aUrl中不含后缀，
        ///         则由aDefaultExt来确定。
        /// </summary>
        /// <param name="aUrl">下载目标Url</param>
        /// <param name="aLocalFolder">要下载到的本地目录</param>
        /// <param name="aTargetFileName">下载到本地后的目标文件名</param>
        /// <param name="aDefaultExt">下载到本地后的缺省文件名后缀（含.），如".jpg"</param>
        /// <returns>返回下载到本地后的带路径文件名</returns>
        public static string Download(string aUrl, string aLocalFolder, string aTargetFileName, string aDefaultExt, bool aOverwrite = false)
        {
            // 下载并获得远程文件名
            string aRemoteFileName = QueryDownloadFileName(aUrl);

            // 确定本地文件名
            string aLocalFileName = aTargetFileName;
            if (string.IsNullOrWhiteSpace(aLocalFileName)) aLocalFileName = aRemoteFileName;
            if (string.IsNullOrWhiteSpace(aLocalFileName)) aLocalFileName = Path.GetFileName(aUrl);
            if (string.IsNullOrWhiteSpace(aLocalFileName)) aLocalFileName = Path.GetRandomFileName();

            // 确定本地文件名后缀
            if (string.IsNullOrWhiteSpace(Path.GetExtension(aLocalFileName)))
            {
                string aExtension = Path.GetExtension(aRemoteFileName);
                if (string.IsNullOrWhiteSpace(aExtension)) aExtension = Path.GetExtension(aUrl);
                if (string.IsNullOrWhiteSpace(aExtension)) aExtension = aDefaultExt;
                aLocalFileName += aExtension;
            }

            // 保存本地文件并返回本地文件名
            aLocalFileName = Path.Combine(aLocalFolder, aLocalFileName);
            if (aOverwrite || !File.Exists(aLocalFileName))
            {
                HttpWebRequest aHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
                using (HttpWebResponse aHttpWebResponse = (HttpWebResponse)aHttpWebRequest.GetResponse())
                {
                    DownloadToFile(aHttpWebResponse, aLocalFileName);
                }
            }
            return aLocalFileName;
        }

        public static Tuple<string, byte[]> DownloadNoSave(string aUrl, string aLocalFolder, string aTargetFileName, string aDefaultExt)
        {
            // 下载并获得远程文件名
            string aRemoteFileName;
            byte[] aBytes = Download(aUrl, out aRemoteFileName);

            // 确定本地文件名
            string aLocalFileName = aTargetFileName;
            if (string.IsNullOrWhiteSpace(aLocalFileName)) aLocalFileName = aRemoteFileName;
            if (string.IsNullOrWhiteSpace(aLocalFileName)) aLocalFileName = Path.GetFileName(aUrl);
            if (string.IsNullOrWhiteSpace(aLocalFileName)) aLocalFileName = Path.GetRandomFileName();

            // 确定本地文件名后缀
            if (string.IsNullOrWhiteSpace(Path.GetExtension(aLocalFileName)))
            {
                string aExtension = Path.GetExtension(aRemoteFileName);
                if (string.IsNullOrWhiteSpace(aExtension)) aExtension = Path.GetExtension(aUrl);
                if (string.IsNullOrWhiteSpace(aExtension)) aExtension = aDefaultExt;
                aLocalFileName += aExtension;
            }

            // 返回本地文件名和下载的数据块
            aLocalFileName = Path.Combine(aLocalFolder, aLocalFileName);
            return Tuple.Create(aLocalFileName, aBytes);
        }

        public static string QueryFileName(HttpWebResponse aHttpWebResponse)
        {
            string aFileName;
            // 取下载文件的指定文件名
            string[] aDispositions = aHttpWebResponse.Headers.GetValues("Content-Disposition");
            if (aDispositions != null && aDispositions.Length > 0)
            {
                Match aMatch = Regex.Match(aDispositions[0], @"filename=\""(.+)\""");
                if (aMatch != null && aMatch.Success)
                    aFileName = aMatch.Groups[1].Value;
                else
                    aFileName = null;
            }
            else
                aFileName = null;
            return aFileName;
        }

        public static string DownloadAsyn(string aUrl, string aLocalPath, string aTargetFileName = null)
        {
            HttpWebRequest aHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            HttpWebResponse aHttpWebResponse = (HttpWebResponse)aHttpWebRequest.GetResponse();

            string aFileName = QueryFileName(aHttpWebResponse);
            if (string.IsNullOrWhiteSpace(aFileName))
            {
                aFileName = Path.GetFileName(aUrl);
                if (string.IsNullOrWhiteSpace(aFileName)) aFileName = Path.GetRandomFileName();
            }

            if (!string.IsNullOrWhiteSpace(aTargetFileName))
            {
                aFileName = $"{aTargetFileName}{Path.GetExtension(aFileName)}";
            }
            aFileName = Path.Combine(aLocalPath, aFileName);

            // 下载内容
            ThreadPool.QueueUserWorkItem(new WaitCallback(r => DownloadThread(aHttpWebResponse, aFileName)), null);

            return aFileName;
        }

        public static byte[] DownloadBytes(HttpWebResponse aHttpWebResponse)
        {
            const int BLOCKLEN = 4096;
            List<byte[]> aBuffer = new List<byte[]>();
            using (Stream aStreamReader = aHttpWebResponse.GetResponseStream())
            {
                byte[] aBytes = new byte[BLOCKLEN];
                int aLength;
                while ((aLength = aStreamReader.Read(aBytes, 0, BLOCKLEN)) > 0)
                {
                    byte[] aRealBytes = new byte[aLength];
                    Array.Copy(aBytes, 0, aRealBytes, 0, aLength);
                    aBuffer.Add(aRealBytes);
                }
                aStreamReader.Close();
            }
            aHttpWebResponse.Close();
            if (aBuffer.Count == 0) return null;
            if (aBuffer.Count == 1) return aBuffer[0];

            // 拼合下载到的数据块
            byte[] aTotalBytes;
            int aTotalBytesCount = (from r in aBuffer select r.Length).Sum();
            aTotalBytes = new byte[aTotalBytesCount];
            int aOffset = 0;
            foreach (byte[] aBlock in aBuffer)
            {
                Array.Copy(aBlock, 0, aTotalBytes, aOffset, aBlock.Length);
                aOffset += aBlock.Length;
            }
            return aTotalBytes;
        }

        public static void DownloadToFile(HttpWebResponse aHttpWebResponse, string aFileName)
        {
            const int BLOCKLEN = 4096;
            using (Stream aStreamReader = aHttpWebResponse.GetResponseStream())
            {
                using (FileStream aFileStream = new FileStream(aFileName, FileMode.Create))
                {
                    byte[] aBytes = new byte[BLOCKLEN];
                    int aLength;
                    while ((aLength = aStreamReader.Read(aBytes, 0, BLOCKLEN)) > 0)
                    {
                        aFileStream.Write(aBytes, 0, aLength);
                    }
                    aFileStream.Close();
                }
                aStreamReader.Close();
            }
            aHttpWebResponse.Close();
        }

        private static void DownloadThread(HttpWebResponse aHttpWebResponse, string aFileName)
        {
            try
            {
                byte[] aBytes = DownloadBytes(aHttpWebResponse);
                File.WriteAllBytes(aFileName, aBytes);
            }
            catch (Exception ex)
            {
                E(ex, "Download Thread");
            }
        }

        public static string DownloadString(string aUrl, Encoding aEncoding)
        {
            using (WebClient aWebClient = new WebClient())
            {
                aWebClient.Encoding = aEncoding;
                return aWebClient.DownloadString(aUrl);
            }
        }

        public static string DownloadString(string aUrl)
        {
            return DownloadString(aUrl, Encoding.UTF8);
        }

        public static string PostRequest(string aUrl, string aData, Encoding aRequestEncoding, Encoding aResponseEncoding)
        {
            byte[] aBytes = aRequestEncoding.GetBytes(aData);
            HttpWebRequest aWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            aWebRequest.Method = "POST";
            aWebRequest.ContentType = "application/x-www-form-urlencoded";
            aWebRequest.ContentLength = aBytes.Length;
            string aResult;
            using (Stream aStream = aWebRequest.GetRequestStream())
            {
                aStream.Write(aBytes, 0, aBytes.Length);
                aStream.Close();
            }
            using (HttpWebResponse aResponse = (HttpWebResponse)aWebRequest.GetResponse())
            {
                using (StreamReader aStreamReader = new StreamReader(aResponse.GetResponseStream(), aResponseEncoding))
                {
                    aResult = aStreamReader.ReadToEnd();
                    aStreamReader.Close();
                }
                aResponse.Close();
            }
            return aResult;
        }
    }
}
