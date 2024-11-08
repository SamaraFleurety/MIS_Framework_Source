using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //无论如何，一个召唤技能def该有且仅有一个summonbase
    public abstract class AE_SummonBase : AbilityEffectBase
    {
        //同时存在上限
        public int existLimits = 1;

        protected abstract Thing GenerateSummoned(AKAbility source);

        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Thing summoned = GenerateSummoned(caster);
            GenSpawn.Spawn(summoned, target.Cell, caster.CasterPawn.Map);
            return base.DoEffect(caster, target);
        }
    }
}
