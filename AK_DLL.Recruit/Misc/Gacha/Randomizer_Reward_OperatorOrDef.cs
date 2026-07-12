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

        #region 自动注入Ext标记的OpDef作为奖池候选
        protected bool rewardsAutowired = false;

        public override IEnumerable<IWeightedRandomable> Candidates
        {
            get
            {
                AutowireRewards();
                return rewardOperator;
            }
        }

        //fixme: 最好是让RandomizerDef解析的时候实现自动注入扫描 但是因为现在SoftRely原因我没做
        protected void AutowireRewards()
        {
            if (rewardsAutowired) return;

            rewardsAutowired = true;
            foreach (OperatorDef operatorDef in DefDatabase<OperatorDef>.AllDefsListForReading)
            {
                Ext_RewardOperatorAutowired ext = operatorDef.GetModExtension<Ext_RewardOperatorAutowired>();
                if (ext?.targetRandomizerDef?.root != this) continue;

                AddOperator(operatorDef, ext.randWeight);
            }
        }

        public void AddOperator(OperatorDef operatorDef, int randWeight)
        {
            if (rewardOperator.Any(reward => reward.operatorDef == operatorDef)) return;

            rewardOperator.Add(new Rewards_Operator
            {
                operatorDef = operatorDef,
                weight = randWeight
            });
        }

        public void InvalidateWeightCache()
        {
            arrayCached = false;
            weightCached = null;
        }
        #endregion

        public override IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            AutowireRewards();
            if (rewardOperator.Count == 0) yield break;

            Rewards_Operator op = rewardOperator[RandRewardIndex()];
            //重复则为参与奖
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
