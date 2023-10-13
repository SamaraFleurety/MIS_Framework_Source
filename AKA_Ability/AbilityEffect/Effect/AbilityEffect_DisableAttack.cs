using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AbilityEffect_GainTraitSelf : AbilityEffectBase
    {
        public override void DoEffect_Pawn(Pawn user, Thing target)
        {
            int? degree_var = this.degree == null ?0:this.degree;
            user.story.traits.GainTrait(new Trait(this.trait, (int)degree_var));
        }
        public TraitDef trait;
        public int? degree;
    }
}
