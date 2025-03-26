using AK_DLL;
using System.Collections.Generic;
using Verse;

namespace AKR_Random.Rewards
{
    //奖励非pawn的物品
    public class Rewards_Things : RewardSet_Base
    {
        public ItemOnSpawn itemRewardSingle = null; //有单个时不处理多个
        public List<ItemOnSpawn> itemRewards = new();
        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0)
        {
            if (map == null || !cell.InBounds(map))
            {
                Log.Error($"[AKR] 无法给予奖品于 ({cell.x}, {cell.z})");
            }

            if (itemRewardSingle != null)
            {
                yield return itemRewardSingle.SpawnThing(cell, map);
                yield break;
            }

            foreach (ItemOnSpawn item in itemRewards)
            {
                /*Thing reward = ThingMaker.MakeThing(item.item, item.stuff);
                reward.stackCount = item.amount;

                if (item.quality is QualityCategory quality)
                {
                    CompQuality compQuality = reward.TryGetComp<CompQuality>();
                    compQuality?.SetQuality(quality, null);
                }

                GenPlace.TryPlaceThing(reward, cell, map, ThingPlaceMode.Near);*/
                yield return item.SpawnThing(cell, map);
            }
        }
    }
}
