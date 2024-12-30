using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class ItemOnSpawn
    {
        public ThingDef item;
        public ThingDef stuff = null;
        public QualityCategory? quality = null;
        public int amount = 1;

        public Thing SpawnThing(IntVec3 cell, Map map)
        {
            if (map == null || !cell.InBounds(map))
            {
                Log.Error($"[AK] ({cell.x}, {cell.z}) 点位于地图外，无法刷物品");
                return null;
            }
            Thing reward = ThingMaker.MakeThing(item, stuff);
            reward.stackCount = amount;

            if (this.quality is QualityCategory q)
            {
                CompQuality compQuality = reward.TryGetComp<CompQuality>();
                compQuality?.SetQuality(q, null);
            }

            GenPlace.TryPlaceThing(reward, cell, map, ThingPlaceMode.Near);
            return reward;
        }
    }
}
