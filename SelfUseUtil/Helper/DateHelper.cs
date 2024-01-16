using Core.Json.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    public static class DateHelper
    {
        // 根据开始时间和结束时间获取中间经过的几个季度节点的开始结束时间
        public static List<DateDto> GetQuarterNodes(DateTimeOffset start, DateTimeOffset end)
        {
            List<DateDto> nodes = new List<DateDto>();
            // 获取开始时间所在季度的结束时间
            DateTimeOffset quarterEnd = start.GetQuarterEnd();
            // 如果开始时间所在季度的结束时间大于等于结束时间，返回空列表
            if (quarterEnd >= end) return nodes;

            var dateDto = new DateDto
            {
                Year = start.Year,
                Quarter = GetCurrentQuarter(start),
                StartTime = start,
                EndTime = quarterEnd.UtcDateTime.AddHours(8).AddDays(1).AddSeconds(-1),
            };
            // 将所在季度的开始结束时间加入列表
            nodes.Add(dateDto);
            // 循环遍历下一个季度的开始时间和结束时间，直到结束时间所在季度为止
            while (true)
            {
                // 获取下一个季度的开始时间
                DateTimeOffset nextQuarterStart = quarterEnd.AddDays(1);
                // 如果下一个季度的开始时间大于等于结束时间，跳出循环
                if (nextQuarterStart >= end) break;
                // 获取下一个季度的结束时间
                DateTimeOffset nextQuarterEnd = nextQuarterStart.GetQuarterEnd();
                // 如果下一个季度的结束时间大于等于结束时间，跳出循环
                if (nextQuarterEnd >= end) break;
                var currentQuarter = new DateDto
                {
                    Year = nextQuarterStart.Year,
                    Quarter = GetCurrentQuarter(nextQuarterStart),
                    StartTime = nextQuarterStart.UtcDateTime.AddHours(8),
                    EndTime = nextQuarterEnd.UtcDateTime.AddHours(8).AddDays(1).AddSeconds(-1),
                };
                // 将下一个季度的时间加入列表
                nodes.Add(currentQuarter);
                // 更新当前季度的结束时间为下一个季度的结束时间
                quarterEnd = nextQuarterEnd;
            }
            // 返回列表
            return nodes;
        }

        // 获取日期所在季度的开始时间
        public static DateTime GetQuarterStart(this DateTimeOffset date)
        {
            return new DateTime(date.Year, (date.Month - 1) / 3 * 3 + 1, 1);
        }

        // 获取日期所在季度的结束时间
        public static DateTime GetQuarterEnd(this DateTimeOffset date)
        {
            return date.GetQuarterStart().AddMonths(3).AddDays(-1);
        }

        public static int GetCurrentQuarter(this DateTimeOffset date)
        {
            return ((date.Month - 1) / 3) + 1;
        }
        public static long ToUnixTimestamp(this DateTimeOffset value)
        {
            return (long)(value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
        }
    }

    [Serializable]
    public class DateDto
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public long StartTimeStamp { get { return DateHelper.ToUnixTimestamp(StartTime); } }
        public long EndTimeStamp { get { return DateHelper.ToUnixTimestamp(EndTime); } }

        public double? QuarterData { get; set; }
        public int? Sort
        {
            get { return int.Parse($"{Year}{Quarter}"); }
        }

        public DateDto DeepCopy(DateDto obj)
        {
            return obj.ToJson().ToObject<DateDto>();
        }
    }
}
