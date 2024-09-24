using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    //放衣服上面的技能容器
    public class TCP_AKATracker : CompProperties
    {
        public List<AKAbilityDef> abilities = new List<AKAbilityDef>();
        public TCP_AKATracker()
        {
            compClass = typeof(TC_AKATracker);
        }
    }

    public class TC_AKATracker : ThingComp
    {
        public AbilityTracker tracker;

        private TCP_AKATracker Props => props as TCP_AKATracker;
        private List<AKAbilityDef> Abilities => Props.abilities;

        Apparel Parent => parent as Apparel;

        Pawn Wearer
        {
            get
            {
                if (Parent == null) return null;
                return Parent.Wearer;
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake(); 
            if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null)
            {
                return;
            }
            tracker = new AbilityTracker(Wearer);
            if (this.Abilities != null && this.Abilities.Count > 0)
            {
                foreach (AKAbilityDef i in this.Abilities)
                {
                    tracker.AddAbility(i);
                    //AKAbilityMaker.MakeAKAbility(i, AKATracker);
                }
            }
        }
        public override void Notify_Equipped(Pawn pawn)
        {
            tracker.owner = pawn;
        }

        public override void CompTick()
        {
            tracker.Tick();
            return;
        }
        public override void CompTickLong()
        {
            tracker.Tick();
            return;
        }
        public override void CompTickRare()
        {
            tracker.Tick();
            return;
        }
        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            return tracker.GetGizmos();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref tracker, "AKATracker", Wearer);
        }
    }
}
