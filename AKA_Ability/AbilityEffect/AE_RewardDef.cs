using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKA_RewardDef
    {
        public List<ItemReward> items;
    }
    public class ItemReward
    {
        public ThingDef thing;

        public QualityCategory quality;

        public int count;
    }
}
