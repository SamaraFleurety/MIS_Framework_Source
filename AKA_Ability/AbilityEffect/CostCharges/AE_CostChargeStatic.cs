using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_CostChargeStatic : AbilityEffectBase
    {
        public int cost = 1;
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            caster.cooldown.CostCharge(cost);
            return base.DoEffect(caster, target);
        }
    }
}
