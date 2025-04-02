using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SummonPawn : AE_SummonBase
    {
        public PawnKindDef summonedDef;  //召唤物

        protected override Thing GenerateSummoned(AKAbility_Summon source)
        {
            return PawnGenerator.GeneratePawn(summonedDef, source.CasterPawn.Faction);
        }
    }
}
