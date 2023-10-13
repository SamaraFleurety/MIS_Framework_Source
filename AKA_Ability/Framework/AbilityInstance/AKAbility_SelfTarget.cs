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

        public AKAbility_SelfTarget(OpAbilityDef def) : base(def)
        {
        }

        protected override void UpdateGizmo()
        {
        }

        protected override void InitializeGizmo()
        {
            cachedGizmo = new Command_Action
            {
                action = delegate ()
                {
                    foreach (AbilityEffectBase compEffect in this.def.compEffectList)
                    {
                        compEffect.DoEffect_All(CasterPawn, CasterPawn, CasterPawn.Position, CasterPawn.Map);
                    }
                    UseOneCharge();
                },
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon
            };
        }
    }
}
