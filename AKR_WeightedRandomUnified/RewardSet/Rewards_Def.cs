using AKR_Random.Rewards;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random.RewardSet
{
    //返回一个def，可能已经不算奖励了。
    public class Rewards_Def : RewardSet_Base
    {
        public Def def;
        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            yield return def;
        }
    }
}
