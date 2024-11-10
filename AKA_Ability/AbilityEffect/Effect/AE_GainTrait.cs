using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AbilityEffect_GainTrait : AbilityEffectBase
    {
        public TraitDef trait;
        public int degree = 0;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null || targetPawn.story == null || targetPawn.story.traits == null) return false;
            targetPawn.story.traits.GainTrait(new Trait(this.trait, degree));
            return true;
        }

    }
}
