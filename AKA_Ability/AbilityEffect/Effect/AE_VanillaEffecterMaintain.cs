using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_VanillaEffecterMaintain : AbilityEffectBase
    {
        public EffecterDef effecter;
        public int maintainTicks = 60;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            var effecter = this.effecter.Spawn();
            effecter.ticksLeft = maintainTicks;
            var map = caster.CasterPawn.Map;
            effecter.Trigger(target.ToTargetInfo(map), target.ToTargetInfo(map));
            effecter.Cleanup();
            return base.DoEffect(caster, target);
        }
    }
}
