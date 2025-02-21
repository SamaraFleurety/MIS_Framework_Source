using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SummonThing : AE_SummonBase
    {
        public ThingDef summoned;

        public ThingDef stuff = null;

        public bool setFaction = false;

        protected override Thing GenerateSummoned(AKAbility_Summon source)
        {
            var thing = ThingMaker.MakeThing(summoned, stuff);
            if (setFaction) thing.SetFactionDirect(source.Caster.Faction);
            return thing;
        }
    }
}
