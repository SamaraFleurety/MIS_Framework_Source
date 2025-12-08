using AKA_Ability.CastConditioner;
using AKA_Ability.Gizmos;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Auto : AKAbility_Base
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
            if (AutoCast && Find.TickManager.TicksGame % 180 == 0 && CastableNow())
            {
                if (Target == null) return;
                TryCastShot(Target);
            }
        }

        public override bool CastableNow()
        {
            if (Find.TickManager.TicksGame % JudgeInterval == 0)
            {
                cachedCastableCondition = true;
                foreach (CastConditioner_Base i in def.castConditions)
                {
                    if (i.ignoredByAuto) continue;
                    if (!i.Castable(this))
                    {
                        cachedCastableCondition = false;
                        lastFailReason = i.failReason;
                        break;
                    }
                }
            }
            return cachedCastableCondition;
        }

        protected override void InitializeGizmoInnate()
        {
            cachedGizmo = new Gizmo_AbilityCast_Toggle(this)
            {
                icon = def.Icon,
                defaultLabel = def.label,
                defaultDesc = def.description,
            };
        }

        protected override void UpdateGizmoInnate()
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
