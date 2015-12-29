using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using static PengSW.RuntimeLog.RL;

namespace PengSW.WebHelper
{
    public static class Downloader
    {
        public static byte[] Download(string aUrl, out string aFileName)
        {
            List<byte[]> aBuffer = new List<byte[]>();
            HttpWebRequest aHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            using (HttpWebResponse aHttpWebResponse = (HttpWebResponse)aHttpWebRequest.GetResponse())
            {
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

                // 下载内容
                const int BLOCKLEN = 4096;
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
            }

            if (aBuffer.Count == 0) return null;
            if (aBuffer.Count == 1) return aBuffer[0];

            // 拼合下载到的数据块
            int aTotalBytesCount = (from r in aBuffer select r.Length).Sum();
            byte[] aTotalBytes = new byte[aTotalBytesCount];
            int aOffset = 0;
            foreach (byte[] aBlock in aBuffer)
            {
                Array.Copy(aBlock, 0, aTotalBytes, aOffset, aBlock.Length);
                aOffset += aBlock.Length;
            }
            return aTotalBytes;
        }

        public static string DownloadAsyn(string aUrl, string aLocalPath, string aTargetFileName = null)
        {
            HttpWebRequest aHttpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            HttpWebResponse aHttpWebResponse = (HttpWebResponse)aHttpWebRequest.GetResponse();
            // 取下载文件的指定文件名
            string aFileName;
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

        private static void DownloadThread(HttpWebResponse aHttpWebResponse, string aFileName)
        {
            try
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
                if (aBuffer.Count == 0) return;

                byte[] aTotalBytes;
                if (aBuffer.Count == 1) aTotalBytes = aBuffer[0];

                // 拼合下载到的数据块
                int aTotalBytesCount = (from r in aBuffer select r.Length).Sum();
                aTotalBytes = new byte[aTotalBytesCount];
                int aOffset = 0;
                foreach (byte[] aBlock in aBuffer)
                {
                    Array.Copy(aBlock, 0, aTotalBytes, aOffset, aBlock.Length);
                    aOffset += aBlock.Length;
                }
                File.WriteAllBytes(aFileName, aTotalBytes);
            }
            catch (Exception ex)
            {
                E(ex, "Download Thread");
            }
        }
    }
}
