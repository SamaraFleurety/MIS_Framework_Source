using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
