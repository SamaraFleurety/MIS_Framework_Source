using System.Runtime.CompilerServices;
using Verse;

namespace AKA_Ability.Summon
{
    [Discardable]
    public class PawnClass_Summoned : Pawn
    {
        public override Graphic Graphic => base.Graphic;

        private TC_SummonedProperties summonedProperties = null;

        public TC_SummonedProperties SummonedProperties
        {
            get
            {
                summonedProperties ??= GetComp<TC_SummonedProperties>();
                return this.summonedProperties;
            }
        }
    }
}
