using System.Collections.Generic;
using Verse;

namespace AKR_Random
{
    public class RandomizerNode_Subnodes<T> : RandomizerNode_Subnodes
    {
        public T val;
    }

    //本身不提供任何奖励。从子节点里面随机挑一个，并返回子节点的结果
    public class RandomizerNode_Subnodes : RandomizerNode_Base
    {
        protected List<RandomizerNode_Base> subnodes = new();
        public override IEnumerable<IWeightedRandomable> Candidates => subnodes;
        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            return subnodes[this.RandRewardIndex()].TryIssueGachaResult(cell, map, gambler);
        }
    }
}
