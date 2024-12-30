using AK_DLL.Rewards;
using AKR_Random;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Gacha
{
    public class RandomizerNode_Reward_Operator : RandomizerNode_Base
    {
        public List<Rewards_Operator> rewardOperator = new();

        //如果抽到重复干员，那就用这个def的东西替代。
        public RandomizerDef duplicateOperatorSubstitude;
        public override IEnumerable<IWeightedRandomable> Candidates => rewardOperator;

        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            Rewards_Operator op = rewardOperator[RandRewardIndex()];
            if (op.operatorDef.CurrentRecruited)
            {
                return duplicateOperatorSubstitude.root.TryIssueGachaResult();
            }
            return op.GenerateGachaResult(cell, map, gambler, point);
        }

    }
}
