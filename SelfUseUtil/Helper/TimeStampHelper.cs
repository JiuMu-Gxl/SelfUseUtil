using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    /// <summary>
    /// 时间戳操作类
    /// </summary>
    public class TimeStampHelper
    {
        #region 标准北京时间（1970年01月01日08时00分00秒）
        #region 10位时间戳转换（秒级）
        /// <summary>
        /// 【秒级】获取时间（北京时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetDateTimeSeconds(long timestamp)
        {
            long begtime = timestamp * 10000000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【秒级】生成10位时间戳（北京时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetTimeStampSeconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalSeconds);
        }
        #endregion

        #region 13位时间戳转换（毫秒级）
        /// <summary>
        /// 【毫秒级】获取时间（北京时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetDateTimeMilliseconds(long timestamp)
        {
            long begtime = timestamp * 10000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【毫秒级】生成13位时间戳（北京时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetTimeStampMilliseconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalMilliseconds);
        }
        #endregion
        #endregion

        #region 标准格林威治时间（Unix时间戳：1970年01月01日00时00分00秒）
        #region 10位时间戳转换（秒级）
        /// <summary>
        /// 【秒级】获取时间（格林威治时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetUnixDateTimeSeconds(long timestamp)
        {
            long begtime = timestamp * 10000000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【秒级】生成10位时间戳（格林威治时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetUnixTimeStampSeconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalSeconds);
        }
        #endregion

        #region 13位时间戳转换（毫秒级）
        /// <summary>
        /// 【毫秒级】获取时间（格林威治时间）
        /// </summary>
        /// <param name="timestamp">10位时间戳</param>
        public static DateTime GetUnixDateTimeMilliseconds(long timestamp)
        {
            long begtime = timestamp * 10000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dt = new DateTime(time_tricks);//转化为DateTime
            return dt;
        }
        /// <summary>
        /// 【毫秒级】生成13位时间戳（格林威治时间）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long GetUnixTimeStampMilliseconds(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64((dt - dateStart).TotalMilliseconds);
        }
        #endregion
        #endregion
    }
}
