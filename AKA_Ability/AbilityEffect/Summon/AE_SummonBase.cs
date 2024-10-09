using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SummonBase : AbilityEffectBase
    {
        public PawnKindDef summonedDef;  //召唤物

        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Pawn summoned = PawnGenerator.GeneratePawn(summonedDef, caster.CasterPawn.Faction);
            GenSpawn.Spawn(summoned, target.Cell, caster.CasterPawn.Map);
            return base.DoEffect(caster, target);
        }
    }
}
