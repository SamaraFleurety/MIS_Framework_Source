using AK_DLL.Rewards;
using AKR_Random;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AK_DLL.Gacha
{
    public class RandomizerNode_Reward_Operator : RandomizerNode_Base
    {
        public List<Rewards_Operator> rewardOperator = new();

        //如果抽到重复干员，那就用这个def的东西替代。
        public RandomizerDef duplicateOperatorSubstitude;

        private bool rewardsAutowired = false;

        public override IEnumerable<IWeightedRandomable> Candidates
        {
            get
            {
                AutowireRewards();
                return rewardOperator;
            }
        }

        private void AutowireRewards()
        {
            if (rewardsAutowired)
            {
                return;
            }
            rewardsAutowired = true;
            foreach (OperatorDef operatorDef in DefDatabase<OperatorDef>.AllDefsListForReading)
            {
                Ext_RewardOperatorAutowired ext = operatorDef.GetModExtension<Ext_RewardOperatorAutowired>();
                if (ext?.targetRandomizerDef?.root != this)
                {
                    continue;
                }
                AddOperator(operatorDef, ext.randWeight);
            }
        }

        public void AddOperator(OperatorDef operatorDef, int randWeight)
        {
            if (rewardOperator.Any(reward => reward.operatorDef == operatorDef))
            {
                return;
            }
            rewardOperator.Add(new Rewards_Operator
            {
                operatorDef = operatorDef,
                weight = randWeight
            });
        }

        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            AutowireRewards();
            if (rewardOperator.Count == 0)
            {
                yield break;
            }
            Rewards_Operator op = rewardOperator[RandRewardIndex()];
            if (op.operatorDef.CurrentRecruited)
            {
                foreach (object result in duplicateOperatorSubstitude.root.TryIssueGachaResult(cell, map, gambler, point))
                {
                    yield return result;
                }
                yield break;
            }
            foreach (object result in op.GenerateGachaResult(cell, map, gambler, point))
            {
                yield return result;
            }
        }
    }
}
