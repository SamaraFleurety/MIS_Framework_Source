using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AE_Jump : AbilityEffectBase
    {
        public VerbProperties verbJump;

        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            return JumpUtility.DoJump(caster.CasterPawn, target, null, verbJump);
        }
    }
}
