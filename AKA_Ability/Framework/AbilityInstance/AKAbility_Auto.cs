using AKA_Ability.Gizmos;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Auto : AKAbility
    {
        public bool AutoCast = false;

        //这是自动瞄准的目标
        protected virtual LocalTargetInfo Target => CasterPawn.TargetCurrentlyAimingAt;

        public AKAbility_Auto(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_Auto(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        public override void Tick()
        {
            base.Tick();
            string failReason = "";
            if (AutoCast && Find.TickManager.TicksGame % 180 == 0 && CastableNow(ref failReason))
            {
                if (Target == null) return;
                TryCastShot(Target);
            }
        }

        protected override void InitializeGizmo()
        {
            cachedGizmo = new Gizmo_AbilityCast_Toggle
            {
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                parent = this,
            };
        }

        protected override void UpdateGizmo()
        {
            cachedGizmo.Disabled = false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AutoCast, "auto");
        }
    }
}
