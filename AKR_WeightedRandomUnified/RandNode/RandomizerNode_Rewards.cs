using AKR_Random.Rewards;
using System.Collections.Generic;
using Verse;

namespace AKR_Random
{
    //最通用的奖励节点，从rewards里面随机挑一个，并给予对应的奖励
    public class RandomizerNode_Rewards : RandomizerNode_Base
    {
        public List<RewardSet_Base> rewards = new();
        public override IEnumerable<IWeightedRandomable> Candidates => rewards;

        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            return rewards[RandRewardIndex()].GenerateGachaResult(cell, map, gambler);
        }
    }
}