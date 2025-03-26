using AKA_Ability.Gizmos;

namespace AKA_Ability
{
    public class AKAbility_SelfTarget : AKAbility_Base
    {

        public AKAbility_SelfTarget(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_SelfTarget(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        protected override void InitializeGizmoInnate()
        {
            cachedGizmo = new Gizmo_AbilityCast_Action
            {
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                parent = this,
                action = delegate ()
                {
                    this.TryCastShot(CasterPawn);
                }
            };
        }
    }
}
