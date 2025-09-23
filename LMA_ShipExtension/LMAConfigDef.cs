using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LMA_Lib
{
    public class LMAConfigDef : Def
    {
        private static LMAConfigDef config = null;
        public static LMAConfigDef ConfigDef
        {
            get
            {
                config ??= DefDatabase<LMAConfigDef>.GetRandom();
                return config;
            }
        }

        //在这个列表里面的地形才视为水 就俩，不需要转hash
        //我草不止俩
        public List<TerrainDef> waterList = new();
        public HashSet<TerrainDef> cachedWaterList = null;

        public HashSet<TerrainDef> WaterList
        {
            get
            {
                cachedWaterList ??= waterList.ToHashSet();
                return cachedWaterList;
            }
        }

        //招募ui中，每个原版技能点都有个图标
        public Dictionary<SkillDef, string> skillIcons = new();
    }
}