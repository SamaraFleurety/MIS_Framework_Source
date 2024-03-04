using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AKA_Ability
{
    public class Verb_AbilityTargeting : Verb_CastBase
    {
        public AKAbility parent;

        protected override bool TryCastShot()
        {
            parent.def.CastEffects(CasterPawn, currentTarget.Cell, currentTarget.Thing, Caster.Map);
            parent.UseOneCharge();
            return true;
        }
    }
}
