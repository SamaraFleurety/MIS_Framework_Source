using AKA_Ability.DelayedEffects;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //delay no more
    public class AE_DelayedEffects : AbilityEffectBase
    {
        public DelayedEffectDef delayedEffect;

        public int burstCount = 1;

        public int burstInterval = 1;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            int delay = burstInterval;

            int burstLeft = burstCount;

            //fixme:估计要改成activator
            Effector_ShootProjectile eff = new Effector_ShootProjectile(delayedEffect, caster.CasterPawn, target);

            while (burstLeft > 0)
            {
                --burstLeft;
                delay += burstInterval;
                GC_DelayedAbilityManager.AddDelayedAbilities(delay, eff);
            }

            return base.DoEffect(caster, target);
        }
    }
}
