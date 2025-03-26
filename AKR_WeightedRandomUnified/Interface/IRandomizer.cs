using System.Collections.Generic;
using Verse;

namespace AKR_Random
{
    public interface IRandomizer
    {
        public IEnumerable<IWeightedRandomable> Candidates { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell">刷奖品的格子</param>
        /// <param name="map">刷奖品的地图</param>
        /// <param name="gambler">谁抽的</param>
        /// <param name="point">默认不使用的预留神秘点数</param>
        /// <returns></returns>
        public abstract IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0);
    }
}
