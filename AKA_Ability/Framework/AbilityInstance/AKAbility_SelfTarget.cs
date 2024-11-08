using AKA_Ability.Gizmos;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_SelfTarget : AKAbility
    {

        public AKAbility_SelfTarget(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_SelfTarget(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        protected override void InitializeGizmo()
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
