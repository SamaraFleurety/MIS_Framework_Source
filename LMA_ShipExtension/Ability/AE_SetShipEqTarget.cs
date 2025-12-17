using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.Ability
{
    //设置舰娘舰装攻击目标。应用优先级小于武器单独设置的目标
    //统一的目标，id强制重置为定值-1。武器自己的技能有id
    public class AE_SetShipEqTarget : AbilityEffectBase
    {
        public int? idOverride = null;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            int id = idOverride != null ? idOverride.Value : caster.ID;
            LMA_AbilityTracker tracker = (LMA_AbilityTracker)caster.container;
            if (tracker.shipEqTargets.ContainsKey(id))
            {
                tracker.shipEqTargets.Add(id, target);
            }
            else tracker.shipEqTargets[id] = target;
            return base.DoEffect(caster, target);
        }
    }
}
