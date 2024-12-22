using AK_DLL;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKR_Random.Rewards
{
    //奖励非pawn的物品
    public class Rewards_Things : RewardSet_Base
    {
        public List<ItemOnSpawn> itemRewards = new();
        public override IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null)
        {
            if (map == null || !cell.InBounds(map))
            {
                Log.Error($"[AKR] 无法给予奖品于 ({cell.x}, {cell.z})");
            }
            foreach (ItemOnSpawn item in itemRewards)
            {
                Thing reward = ThingMaker.MakeThing(item.item, item.stuff);
                reward.stackCount = item.amount;

                if (item.quality is QualityCategory quality)
                {
                    CompQuality compQuality = reward.TryGetComp<CompQuality>();
                    compQuality?.SetQuality(quality, null);
                }

                GenPlace.TryPlaceThing(reward, cell, map, ThingPlaceMode.Near);
                yield return reward;
            }
        }
    }
}
