using AKA_Ability.DelayedEffects;
using RimWorld.Planet;
using System;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //delay no more
    public class AE_DelayedEffects : AbilityEffectBase
    {
        public AbilityEffectsDef delayedEffect;

        public int burstCount = 1;

        public int burstInterval = 1;

        public Type effectorType = typeof(DelayedEffectorBase);

        public bool delayFirstEffect = false; //第一次发效果是瞬发还是也有延迟

        public override bool DoEffect(AKAbility_Base caster, GlobalTargetInfo globalTargetInfo = default, LocalTargetInfo localTargetInfo = default)
        {
            if (caster.CasterPawn == null) return false;

            if (forceTargetSelf) localTargetInfo = new LocalTargetInfo(caster.CasterPawn);

            int delay = 1;
            if (delayFirstEffect) delay = burstInterval;
            int burstLeft = burstCount;

            //Log.Message("do effect a");
            DelayedEffectorBase effector = (DelayedEffectorBase)Activator.CreateInstance(effectorType, delayedEffect, caster, localTargetInfo, globalTargetInfo);

            while (burstLeft > 0)
            {
                --burstLeft;
                GC_DelayedAbilityManager.AddDelayedAbilities(delay, effector);
                delay += burstInterval;
            }

            return true;
        }


    }
}
