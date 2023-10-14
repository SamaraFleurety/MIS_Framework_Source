using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    //Vanilla Ability
    public class VAbility_AKATrackerContainer : Ability
    {
        public AKAbility_Tracker AKATracker;

        public VAbility_AKATrackerContainer(Pawn pawn)
            : base(pawn)
        {
        }

        public VAbility_AKATrackerContainer(Pawn pawn, AbilityDef def)
            : base(pawn, def)
        {
        }
        public override void AbilityTick()
        {
            AKATracker.Tick();
            return;
        }

        public override IEnumerable<Command> GetGizmos()
        {
            return AKATracker.GetGizmos();
        }

        public override void ExposeData()
        {
            base.ExposeData(); //删了的话会导致这个va本身无法被实例化
            Scribe_Deep.Look(ref AKATracker, "AKATracker", pawn);
        }
    }
}
