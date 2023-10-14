using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    [StaticConstructorOnStartup]
    public static class AKA_Utilities
    {
    }

    public static class AKAbilityMaker
    {
        public static AKAbility MakeAKAbility(OpAbilityDef def, AKAbility_Tracker tracker)
        {
            AKAbility ability = (AKAbility)Activator.CreateInstance(def.abilityClass);

            ability.container = tracker;
            ability.cooldown = new CDandCharge(1, def.maxCharge, def.CD * (int)def.CDUnit);
            ability.def = def;

            if (def.grouped)
            {
                tracker.groupedAbilities.Add(ability);
                tracker.indexActiveGroupedAbility = 0;
            }
            else tracker.innateAbilities.Add(ability);

            return ability;
        }
    }
}
