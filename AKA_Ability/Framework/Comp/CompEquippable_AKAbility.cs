using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    public class CompEquippable_AKAbility : CompEquippable
    {
        private TC_AKATracker Tracker => parent?.GetComp<TC_AKATracker>();

        public override void CompTick()
        {
            Log.Message("CompTick");
            base.CompTick();
            Tracker?.CompTick();
        }

        public override IEnumerable<Gizmo> CompGetEquippedGizmosExtra()
        {
            if (!base.Holder.Drafted)
                yield break;
            foreach (Gizmo gizmo in Tracker.CompGetWeaponGizmosExtra())
            {
                yield return (Command)gizmo;
            }
        }
    }
}
