using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AE_Jump : AbilityEffectBase
    {
        public VerbProperties verbJump;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.container == null) return false;

            return JumpUtility.DoJump(caster.CasterPawn, target, null, verbJump);
        }
    }
}
