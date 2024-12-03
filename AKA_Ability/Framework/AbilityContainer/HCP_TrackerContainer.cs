using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class HCP_TrackerContainer : HediffCompProperties
    {
        public List<AKAbilityDef> abilities = new List<AKAbilityDef>(); //出生时带有的技能
        public HCP_TrackerContainer()
        {
            compClass = typeof(HC_TrackerContainer);
        }
    }

    //我觉得这玩意是必有pawn的
    public class HC_TrackerContainer : HediffComp
    {
        public AbilityTracker tracker;

        private HCP_TrackerContainer Props => props as HCP_TrackerContainer;
        private List<AKAbilityDef> Abilities => Props.abilities;

        //Apparel Parent => parent as Apparel;

        /*Pawn Wearer
        {
            get
            {
                if (Parent == null) return null;
                return Parent.Wearer;
            }
        }*/

        Pawn CasterPawn
        {
            get
            {
                return parent.pawn;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            //fixme:没做ce兼容
            if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null)
            {
                return;
            }
            Log.Message($"caster pawn {CasterPawn == null}");
            Log.Message($"caster pawn {CasterPawn.Name}");
            tracker = new AbilityTracker(CasterPawn);
            if (this.Abilities != null && this.Abilities.Count > 0)
            {
                foreach (AKAbilityDef i in this.Abilities)
                {
                    tracker.AddAbility(i);
                    //AKAbilityMaker.MakeAKAbility(i, AKATracker);
                }
            }
        }

        /*public override void PostPostMake()
        {
            base.PostPostMake();
            
        }*/
        /*public override void Notify_Equipped(Pawn pawn)
        {
            tracker.owner = pawn;
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            tracker.owner = null;
        }*/

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (CasterPawn == null) return;
            tracker.Tick();
            return;
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            return tracker.GetGizmos();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Log.Message("expose hcp tracker");
            Scribe_Deep.Look(ref tracker, "AKATracker", CasterPawn);
        }
    }
}
