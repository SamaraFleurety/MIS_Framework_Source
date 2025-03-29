using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class CompEquippable_AKAbility : CompEquippable
    {
        private TC_AKATracker Tracker => parent?.GetComp<TC_AKATracker>();

        public override void CompTick()
        {
            base.CompTick();
            Tracker?.CompTick();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!base.Holder.Drafted) return Enumerable.Empty<Gizmo>();
            return Tracker?.CompGetGizmosExtra();
        }
    }
}
