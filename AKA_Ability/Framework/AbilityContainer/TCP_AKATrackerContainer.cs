using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class TCP_AKATracker : CompProperties
    {
        public List<AKAbilityDef> AKAbilities = new List<AKAbilityDef>();
        public TCP_AKATracker()
        {
            compClass = typeof(TC_AKATracker);
        }
    }

    public class TC_AKATracker : ThingComp
    {
        public AKA_AbilityTracker AKATracker;

        private TCP_AKATracker Props => props as TCP_AKATracker;
        private List<AKAbilityDef> AKAbilities => Props.AKAbilities;

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
            AKATracker = new AKA_AbilityTracker();
            if (this.AKAbilities != null && this.AKAbilities.Count > 0)
            {
                foreach (AKAbilityDef i in this.AKAbilities)
                {
                    AKAbilityMaker.MakeAKAbility(i, AKATracker);
                }
            }
        }
        public override void Notify_Equipped(Pawn pawn)
        {
            AKATracker.owner = pawn;
        }

        public override void CompTick()
        {
            AKATracker.Tick();
            return;
        }
        public override void CompTickLong()
        {
            AKATracker.Tick();
            return;
        }
        public override void CompTickRare()
        {
            AKATracker.Tick();
            return;
        }
        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            return AKATracker.GetGizmos();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref AKATracker, "AKATracker", Wearer);
        }
    }
}
