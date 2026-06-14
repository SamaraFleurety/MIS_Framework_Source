using AKR_Random.Rewards;
using System.Collections.Generic;
using Verse;

namespace AK_DLL.Rewards
{
    //gacha奖励 一个干员
    public class Rewards_Operator : RewardSet_Base
    {
        public OperatorDef operatorDef;

        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            if (operatorDef.CurrentRecruited) yield break;   //抽到重复干员就毛都没有
            yield return operatorDef.Recruit(cell, map);
        }
    }
}
