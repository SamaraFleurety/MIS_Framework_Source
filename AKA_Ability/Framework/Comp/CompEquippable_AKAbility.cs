using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    //用来显示放在在武器上面的技能Gizmo
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

        public override IEnumerable<Gizmo> CompGetEquippedGizmosExtra()
        {
            if (!Holder.Drafted) yield break;
            foreach (Gizmo gizmo in Tracker.CompGetWeaponGizmosExtra())
            {
                yield return (Command)gizmo;
            }
        }
    }
}
