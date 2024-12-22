using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random.Rewards
{
    //毛都不给
    public class Rewards_Nothing : RewardSet_Base
    {
        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null)
        {
            return Enumerable.Empty<object>();
        }
    }
}
