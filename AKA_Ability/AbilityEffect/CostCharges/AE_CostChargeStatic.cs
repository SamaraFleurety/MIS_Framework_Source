using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_CostChargeStatic : AbilityEffectBase
    {
        public int cost = 1;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            caster.cooldown.CostCharge(cost);
            return base.DoEffect(caster, target);
        }
    }
}
