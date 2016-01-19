using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PengSW.TcpService
{
    public static class BytesHelper
    {
        public static string BytesToString(this byte[] aBytes)
        {
            StringBuilder aStringBuilder = new StringBuilder();
            foreach (byte aByte in aBytes)
            {
                aStringBuilder.Append(aByte.ToString("X2"));
                aStringBuilder.Append(",");
            }
            if (aStringBuilder.Length > 0) aStringBuilder.Length--;
            return aStringBuilder.ToString();
        }
    }
}
