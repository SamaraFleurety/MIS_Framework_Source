using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class AKAbility_SelfTarget : AKAbility
    {
        private Command_Action cachedGizmo = null;

        public AKAbility_SelfTarget(OperatorAbilityDef def) : base(def)
        {
        }

        public override Gizmo GetGizmo()
        {
            if (cachedGizmo == null) InitializeGizmo();
            return cachedGizmo;
        }

        private void InitializeGizmo()
        {
            cachedGizmo = new Command_Action
            {
                action = delegate ()
                {
                    foreach (AbilityEffectBase compEffect in this.def.compEffectList)
                    {
                        compEffect.DoEffect_Pawn(CasterPawn, CasterPawn);
                        compEffect.DoEffect_IntVec(CasterPawn.Position, CasterPawn.Map, CasterPawn);
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
