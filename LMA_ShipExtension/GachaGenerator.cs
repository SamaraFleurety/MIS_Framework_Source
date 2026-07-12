using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Verse;

namespace LMA_Lib
{
    //每周刷新的卡池 使用伪随机种子生成器
    public static class GachaGenerator
    {
        public static int GenerateSeed(uint seed, uint iteration)
        {
            return MurmurHash.GetInt(seed, iteration);
        }

        public static int SeedWeekly(DateTime dateTime)
        {
            DateTime periodStart = GetPeriodStart(dateTime);
            return GenerateSeed((uint)periodStart.Year, (uint)WeekOfYear(periodStart));
        }

        public static List<T> RequestWeekly<T>(IList<T> pool, int count, DateTime dateTime)
        {
            if (pool == null)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            List<T> candidates = pool.Distinct().ToList();
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException("每周卡池没有可用候选项。");
            }
            if (count < 0 || count > candidates.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "生成数量不能小于0或超过卡池唯一候选项数量。");
            }

            Random random = new(SeedWeekly(dateTime));
            for (int index = candidates.Count - 1; index > 0; index--)
            {
                int targetIndex = random.Next(index + 1);
                (candidates[index], candidates[targetIndex]) = (candidates[targetIndex], candidates[index]);
            }

            return candidates.Take(count).ToList();
        }

        //本年内的第几周
        public static int WeekOfYear(DateTime date)
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime GetPeriodStart(DateTime dateTime)
        {
            DateTime date = dateTime.Date;
            int daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-daysSinceMonday);
        }
    }
}
