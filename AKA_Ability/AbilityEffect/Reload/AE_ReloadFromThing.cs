using System;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_ReloadFromThing : AbilityEffectBase, ITargetingValidator
    {
        public AbilityReloadProperty reloadProp = null;

        public bool TargetingValidator(TargetInfo info)
        {
            throw new NotImplementedException();
        }

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            IntVec3 clickCell = target.Cell;
            //弹药def要自己检查
            Thing ammo = clickCell.GetThingList(caster.CasterPawn.Map).Find(t => t.def == reloadProp.ammoDef);
            if (ammo == null) return false;

            AbilityReload reloadComp = (AbilityReload)Activator.CreateInstance(reloadProp.reloadClass, reloadProp, caster.container);
            //最大努力交付弹药
            reloadComp.ReloadFrom(ammo);

            return base.DoEffect(caster, target);
        }
    }
}
