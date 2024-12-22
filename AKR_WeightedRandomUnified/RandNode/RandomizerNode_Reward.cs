using AKR_Random.Rewards;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random
{
    //从rewards里面随机挑一个，并给予对应的奖励
    public class RandomizerNode_Reward : RandomizerNode_Base
    {
        public List<RewardSet_Base> rewards = new();
        public override IEnumerable<IWeightedRandomable> Candidates => rewards;

        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null)
        {
            base.TryIssueGachaResult(cell, map, gambler);
            return rewards[Algorithm.WeightArrayRand(weightCached)].GenerateGachaResult(cell, map, gambler);
        }
    }
}
