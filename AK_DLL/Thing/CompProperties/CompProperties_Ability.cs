using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Threading.Tasks;

namespace AK_DLL
{
    public class CompProperties_Ability : CompProperties_Reloadable
    {
        public CompProperties_Ability()
        {
            this.compClass = typeof(CompAbility);
            this.destroyOnEmpty = false;
            this.maxCharges = 1;
            this.displayGizmoWhileUndrafted = false;
        }
        public OperatorAbilityDef abilityDef;
        public int maxSummoned;
    }
}