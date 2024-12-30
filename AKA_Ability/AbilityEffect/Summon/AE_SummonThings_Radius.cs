using AK_TypeDef;
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
    public class AE_SummonThings_Radius : AE_SummonThing
    {
        public float radius = 7f;
        public int count = 2;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster is not AKAbility_Summon casterReal)
            {
                Log.Error($"[AK]尝试使用非召唤技能{caster.def.label}召唤召唤物。");
                return false;
            }

            HashSet<IntVec3> cellInRange = GenericUtilities.TradeableCellsAround(target.Cell, caster.CasterPawn.Map, radius).ToHashSet();  
            if (cellInRange.Count < count) count = cellInRange.Count;

            for (int i = 0; i < count; ++i)
            {
                if (cellInRange.NullOrEmpty()) break;       //可能格子不够刷

                IntVec3 cell = cellInRange.RandomElement(); //这个算法不行，要是卡记得换

                Thing summoned = GenerateSummoned(casterReal);
                GenSpawn.Spawn(summoned, cell, caster.CasterPawn.Map);

                cellInRange.Remove(cell); 

                if (!recordCreature) continue;

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
            }

            return base.DoEffect(caster, target);
        }
    }
}
