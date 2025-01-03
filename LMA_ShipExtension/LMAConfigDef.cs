﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib
{
    public class LMAConfigDef : Def
    {
        private static LMAConfigDef config = null;
        public static  LMAConfigDef ConfigDef
        {
            get
            {
                config ??= DefDatabase<LMAConfigDef>.GetRandom();
                return config;
            }
        }

        //在这个列表里面的地形才视为水 就俩，不需要转hash
        //我草不止俩
        public List<TerrainDef> waterList = new List<TerrainDef>();
        public HashSet<TerrainDef> cachedWaterList = null;

        public HashSet<TerrainDef> WaterList
        {
            get
            {
                cachedWaterList ??= waterList.ToHashSet();
                return cachedWaterList;
            }
        }
    }
}