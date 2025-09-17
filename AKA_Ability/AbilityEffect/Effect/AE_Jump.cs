using AKA_Ability.AbilityEffect;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    public class AE_Jump : AbilityEffectBase, ITargetingValidator
    {
        public VerbProperties verbJump;

        public bool TargetingValidator(TargetInfo info)
        {
            List<Thing> list = info.Map.thingGrid.ThingsListAtFast(info.Cell);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].def.passability == Traversability.Impassable)
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            return JumpUtility.DoJump(caster.CasterPawn, target, null, verbJump);
        }
    }
}
