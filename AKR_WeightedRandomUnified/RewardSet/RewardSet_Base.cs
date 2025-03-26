using System.Collections.Generic;
using Verse;

namespace AKR_Random.Rewards
{
    //被选中时，直接给予本节点所有奖励。本节点是终点，不会再继续开连锁
    public abstract class RewardSet_Base : IWeightedRandomable
    {
        public int weight = 1;
        public virtual int Weight => weight;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell">刷奖品的中心节点</param>
        /// <param name="map">刷奖品的地图</param>
        /// <param name="gambler">谁抽的</param>
        /// <param name="point">抽奖点数。默认并不使用</param>
        /// <returns></returns>
        public abstract IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0);
    }
}
