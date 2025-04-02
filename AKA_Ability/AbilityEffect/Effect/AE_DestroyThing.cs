using Verse;

namespace AKA_Ability.AbilityEffect
{
    //目标物品匹配并且数量足够就摧毁并且返回true，否则无事发生并且返回false
    public class AE_DestroyThing : AbilityEffectBase
    {
        public ThingDef thingdef;
        public int amount = 1;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.container == null) return false;

            Thing targetThing = target.Thing;
            if (targetThing == null || targetThing.def != thingdef || targetThing.stackCount < amount)
            {
                return false;
            }

            if (targetThing.stackCount > amount)
            {
                targetThing = targetThing.SplitOff(amount);
            }
            targetThing.Destroy();

            return true;
        }
    }
}
