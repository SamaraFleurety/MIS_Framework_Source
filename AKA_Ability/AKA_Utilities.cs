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
        public static AKAbility MakeAKAbility(AKAbilityDef def, AKAbility_Tracker tracker)
        {

            AKAbility ability = (AKAbility)Activator.CreateInstance(AutoAbilityClass(def));

            //Log.Message($"akab: {ability == null}");
            //Log.Message($"order {ability.GetGizmo().Order}");

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

        private static Type AutoAbilityClass(AKAbilityDef def)
        {
            Type t = def.abilityClass;
            if (t != null) return t;
            switch (def.targetMode)
            {
                case TargetMode.AutoEnemy:
                    t = typeof(AKAbility_Toggle);
                    break;
                case TargetMode.Self:
                    t = typeof(AKAbility_SelfTarget);
                    break;
                case TargetMode.VerbSingle:
                    t = typeof(AKAbility_VerbTarget);
                    break;
                default:
                    Log.Message($"AKA invalid ability type for {def.defName}");
                    break;
            }
            return t;
        }
    }
}
