﻿using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    public class CompEquippable_AKAbility : CompEquippable
    {
        TC_AKATracker cachedTracker = null;
        private TC_AKATracker Tracker
        {
            get
            {
                cachedTracker ??= parent?.GetComp<TC_AKATracker>();
                return cachedTracker;
            }
        }
        public override void CompTick()
        {
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
