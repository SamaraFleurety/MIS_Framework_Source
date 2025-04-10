using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKA_Ability.AbilityEffect
{
    public class AE_ReloadFromThing : AbilityEffectBase, ITargetingValidator
    {
        public AKAbilityDef abilityToReload;//技能多了有可能做成下标
        public ThingDef ammoDef; //弹药def
        public int ammoCountToRefill = 0; //填了这个就是只接受一次性暴力装填,弹药数量满足就全部消耗,不论剩余
        public int ammoCountPerCharge; //装填一次充能需要的弹药数量
        public SoundDef soundReload;
        //public AbilityReloadProperty reloadProp = null;

        public bool TargetingValidator(TargetInfo info)
        {
            return true;
        }

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            //弹药def要自己检查
            Thing ammo = target.Cell.GetThingList(caster.CasterPawn.Map).Find(t => t.def == ammoDef);
            if (ammo == null) return false;

            AKAbility_Base ability = caster.container.TryGetAbility(abilityToReload);
            ReloadFrom(ammo);

            return base.DoEffect(caster, target);
            //最大努力交付弹药
            void ReloadFrom(Thing ammo)
            {
                if (!ability.cooldown.NeedsReload) return;
                if (ammoCountToRefill != 0)
                {
                    if (ammo.stackCount < ammoCountToRefill) return;
                    ammo.SplitOff(ammoCountToRefill).Destroy();
                    ability.cooldown.Charge = ability.cooldown.MaxCharge;
                }
                else
                {
                    if (ammo.stackCount < ammoCountPerCharge) return;
                    int num = Mathf.Clamp(ammo.stackCount / ammoCountPerCharge, 0, ability.cooldown.MaxCharge - ability.cooldown.Charge);
                    ammo.SplitOff(num * ammoCountPerCharge).Destroy();
                    ability.cooldown.Charge += num;
                }
                soundReload?.PlayOneShot(new TargetInfo(caster.CasterPawn.Position, caster.CasterPawn.Map));
            }
        }
    }
}
