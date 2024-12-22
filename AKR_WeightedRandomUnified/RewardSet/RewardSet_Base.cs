using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random.Rewards
{
    //被选中时，直接给予奖励
    public abstract class RewardSet_Base : IWeightedRandomable
    {
        public int weight = 1;
        public virtual int Weight => weight;

        public abstract IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null);
    }
}
