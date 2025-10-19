using System.Collections.Generic;
using Verse;

namespace AKR_Random
{
    //允许利用def，重复利用某些随机池
    public class RandomizerNode_Def : RandomizerNode_Base
    {
        public override int Weight
        {
            get
            {
                if (!inheritCandidatesWeight) return weight;
                return subDef.root.Weight;  //从def继承权重
            }
        }

        //只有subDef一个选项。如果需要多个def之间随机那就写多个这个node，不用整别的
        public RandomizerDef subDef;
        public override IEnumerable<IWeightedRandomable> Candidates
        {
            get { yield return subDef.root; }
        }

        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            return subDef.root.TryIssueGachaResult(cell, map, gambler);
        }

        //sub def自己会算
        protected override void CalculateWeightArray()
        {
            return;
        }
    }
}