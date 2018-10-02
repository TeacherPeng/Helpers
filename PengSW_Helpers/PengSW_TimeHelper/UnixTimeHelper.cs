using System;

namespace PengSW.TimeHelper
{
    /// <summary>
    /// UnixTimeHelper提供基于UTC时间的Unix时间戳与时间值之间的相互转换
    /// 默认时间戳单位为秒。
    /// 毫秒级时间戳单位转换方法带有_ms后缀。
    /// </summary>
    public static class UnixTimeHelper
    {
        private static DateTime _UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long GetUnitTime() => (long)((DateTime.UtcNow - _UnixStartTime).TotalSeconds);
        public static DateTime UnixTimeToTime(this long aUnixTime) => _UnixStartTime.AddSeconds(aUnixTime).ToLocalTime();
        public static long TimeToUnixTime(this DateTime aTime) => (long)((aTime.ToUniversalTime() - _UnixStartTime).TotalSeconds);
        public static long GetUnitTime_ms() => (long)((DateTime.UtcNow - _UnixStartTime).TotalMilliseconds);
        public static DateTime UnixTimeToTime_ms(this long aUnixTime) => _UnixStartTime.AddMilliseconds(aUnixTime).ToLocalTime();
        public static long TimeToUnixTime_ms(this DateTime aTime) => (long)((aTime.ToUniversalTime() - _UnixStartTime).TotalMilliseconds);
    }
}
