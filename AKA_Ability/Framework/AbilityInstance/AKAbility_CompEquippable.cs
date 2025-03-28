using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    //是个空容器 用来激活武器上面的AKATracker Comp
    public class AKAbility_CompEquippable : AKAbility_Base
    {
        private ThingWithComps Weapon => CasterPawn?.equipment?.Primary;
        private TC_AKATracker Tracker => Weapon?.GetComp<TC_AKATracker>();

        public AKAbility_CompEquippable(AbilityTracker tracker) : base(tracker)
        {
        }

        public AKAbility_CompEquippable(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        public override void Tick()
        {
            Log.Message("Tick::AKAbility_CompEquippable");
            Tracker?.CompTick();
        }

        public override IEnumerable<Command> GetGizmos()
        {
            if (!base.CasterPawn.Drafted && !def.displayGizmoUndraft)
                yield break;
            if (Tracker == null)
                yield break;
            foreach (Gizmo gizmo in Tracker.CompGetGizmosExtra())
            {
                yield return (Command)gizmo;
            }
        }

        protected override void InitializeGizmoInnate()
        {
        }
    }
}
