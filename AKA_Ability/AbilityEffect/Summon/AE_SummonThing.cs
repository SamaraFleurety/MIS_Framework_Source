using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SummonThing : AE_SummonBase
    {
        public ThingDef summoned;

        public ThingDef stuff = null;
        protected override Thing GenerateSummoned(AKAbility_Summon source)
        {
            return ThingMaker.MakeThing(summoned, stuff);
        }
    }
}
