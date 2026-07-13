using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LMA_Lib
{
    //每个游戏月刷新一次的卡池，使用伪随机种子生成器
    public static class GachaGenerator
    {
        public static int GenerateSeed(uint seed, uint iteration)
        {
            return MurmurHash.GetInt(seed, iteration);
        }

        public static int GetPeriodStartTick(int gameTick)
        {
            return gameTick / GenDate.TicksPerQuadrum * GenDate.TicksPerQuadrum;
        }

        public static int SeedMonthly(int gameTick)
        {
            int periodStartTick = GetPeriodStartTick(gameTick);
            return GenerateSeed((uint)periodStartTick, (uint)(periodStartTick / GenDate.TicksPerQuadrum));
        }

        public static List<T> RequestPerQuadrum<T>(IList<T> pool, int count, int gameTick)
        {
            if (pool == null)
            {
                throw new ArgumentNullException(nameof(pool));
            }

            List<T> candidates = pool.Distinct().ToList();
            if (candidates.Count == 0)
            {
                throw new InvalidOperationException("月度卡池没有可用候选项。");
            }
            if (count < 0 || count > candidates.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, "生成数量不能小于0或超过卡池唯一候选项数量。");
            }

            Random random = new(SeedMonthly(gameTick));
            for (int index = candidates.Count - 1; index > 0; index--)
            {
                int targetIndex = random.Next(index + 1);
                (candidates[index], candidates[targetIndex]) = (candidates[targetIndex], candidates[index]);
            }

            return candidates.Take(count).ToList();
        }
    }
}
