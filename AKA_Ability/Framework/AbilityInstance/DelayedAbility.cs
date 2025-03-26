namespace AKA_Ability
{
    /*public class DelayedAbility
    {
        public Pawn caster = null;
        public LocalTargetInfo targetInfo = null;
        public Thing target = null;
        public IntVec3? cell = null;
        public Map map = null;
        public AbilityEffectBase abilityEffect = null;

        public DelayedAbility(Pawn caster, LocalTargetInfo targetInfo, Thing target, IntVec3? cell, Map map, AbilityEffectBase abilityEffect)
        {
            this.caster = caster;
            this.targetInfo = targetInfo;
            this.target = target;
            this.cell = cell;
            this.map = map;
            this.abilityEffect = abilityEffect;
        }

        public void DoEffect_Delayed()
        {
            if (caster == null || caster.DestroyedOrNull() || caster.Dead) return;
            if (abilityEffect == null) return;
            if (targetInfo != null) abilityEffect.DoEffect_All(caster, targetInfo, true);
            else abilityEffect.DoEffect_All(caster, target, cell, map, true);
        }
    }*/
}
