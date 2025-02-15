using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SummonPawn : AE_SummonBase
    {
        public PawnKindDef summonedDef;  //召唤物

        public DutyDef dutyDef;

        protected override Thing GenerateSummoned(AKAbility_Summon source)
        {
            var pawn = PawnGenerator.GeneratePawn(summonedDef, source.CasterPawn.Faction);
            var lord = LordMaker.MakeNewLord(source.CasterPawn.Faction, new LordJob_Variable(dutyDef, null, null, null, 15f), source.CasterPawn.Map);
            lord.AddPawn(pawn);
            return pawn;
        }
    }
}
