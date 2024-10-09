using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.Summon
{
    //召唤物会带此comp
    public class TCP_SummonedProperties : CompProperties
    {
        public int timeExpire = -1;
        public int summonCap = 1;        //最多同时存在多少个
        public TCP_SummonedProperties()
        {
            compClass = typeof(TC_SummonedProperties);
        }
    }

    public class TC_SummonedProperties : ThingComp
    {
        public TCP_SummonedProperties Props => props as TCP_SummonedProperties;
        public int timeSpawned;

        //上面时间大于这个就会消失。为-1不消失
        public int TimeExpire => Props.timeExpire;

        public Pawn Parent_Summoner;

        public AKAbility Parent_Ability;

        public override void CompTick()
        {
            base.CompTick();
        }

        public virtual void Tick(int amt)
        {
            if (TimeExpire != -1)
            {
                timeSpawned += amt;
                if (timeSpawned > TimeExpire)
                {
                    parent.Destroy(DestroyMode.QuestLogic);
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref timeSpawned, "spawn");
            //Scribe_Values.Look(ref timeExpire, "expire", -1);
            Scribe_References.Look(ref Parent_Summoner, "master");
        }

    }
}
