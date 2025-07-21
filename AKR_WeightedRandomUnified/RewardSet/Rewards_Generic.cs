using AKR_Random.Rewards;
using System.Collections.Generic;
using Verse;

namespace AKR_Random.RewardSet
{
    public class Rewards_Generic<T> : RewardSet_Base
    {
        public T rewardSingle;
        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            yield return rewardSingle;
        }

        public override bool RewardValidator(object reward)
        {
            return reward is T;
        }

        public override void SetReward(object reward)
        {
            rewardSingle = (T)reward;
        }
    }
}
