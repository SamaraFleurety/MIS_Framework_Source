using AKA_Ability.Summon;
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

        protected abstract Thing GenerateSummoned(AKAbility_Summon source);

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (!(caster is AKAbility_Summon casterReal))
            {
                Log.Error($"[AK]尝试使用非召唤技能{caster.def.label}召唤召唤物。");
                return false;
            }

            Thing summoned = GenerateSummoned(casterReal);
            GenSpawn.Spawn(summoned, target.Cell, caster.CasterPawn.Map);

            TC_SummonedProperties compSummoned = summoned.TryGetComp<TC_SummonedProperties>();
            if (compSummoned == null)
            {
                Log.Error($"[AK]尝试召唤非召唤物{summoned.def.label}");
                return false;
            }

            compSummoned.Parent_Ability = casterReal;
            compSummoned.timeSpawned = 0;
            compSummoned.Parent_Summoner = casterReal.CasterPawn;


            casterReal.Notify_SummonedSpawned(summoned);
            return base.DoEffect(caster, target);
        }
    }
}
