using AKR_Random.Rewards;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Rewards
{
    public class Rewards_Operator : RewardSet_Base
    {
        public OperatorDef operatorDef;

        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null)
        {
            yield return operatorDef.Recruit(cell, map);
        }
    }
}
