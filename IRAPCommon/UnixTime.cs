using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IRAPCommon
{

    public class UnixTime
    {
        private static DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>   
        /// 将unixtime转换为.NET的DateTime   
        /// </summary>   
        /// <param name="timeStamp">秒数</param>   
        /// <returns>转换后的时间</returns>   
        public static DateTime FromUnixTime(long timeStamp)
        {
            return new DateTime((timeStamp + 8 * 60 * 60) * 10000000 + BaseTime.Ticks);
        }

        /// <summary>   
        /// 将.NET的DateTime转换为unix time   
        /// </summary>   
        /// <param name="dateTime">待转换的时间</param>   
        /// <returns>转换后的unix time</returns>   
        public static long FromDateTime(DateTime dateTime)
        {
            return (dateTime.Ticks - BaseTime.Ticks) / 10000000 - 8 * 60 * 60;
        }

        /// <summary>
        /// 带时区的把UnixTime转换到指定时区的时间（IRAP平台使用）
        /// </summary>
        /// <param name="UnixTime"></param>
        /// <param name="Zone"></param>
        /// <returns></returns>
        public static DateTime UnixToLocalTime(long UnixTime, int Zone)
        {
            // DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0);
            long lTime = (3600 * Zone + UnixTime) * 10000000L;
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }

        /// <summary>
        /// 把本地时间转换为UnixTime时间，包含时区的概念
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long LocalTimeToUnix(DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        public static long LocalTimeToUnix(DateTime time, int Zone)
        {
            return (time.Ticks - BaseTime.Ticks) / 10000000 - Zone * 60 * 60;
        }

    }
}