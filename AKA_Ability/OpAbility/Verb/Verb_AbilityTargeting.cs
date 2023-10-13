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
        AKAbility parent;

        //Pawn Caster => parent.container.owner;
        protected override bool TryCastShot()
        {
            parent.def.CastEffects(CasterPawn, currentTarget.Cell, currentTarget.Thing, Caster.Map);
            return true;
        }
    }
}
