using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class HediffComp_RemoveTrait : HediffComp
    {
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (this.Pawn.story.traits.HasTrait(TraitDef.Named("AK_DisableAttack"))) 
            {
                this.Pawn.story.traits.RemoveTrait(this.Pawn.story.traits.GetTrait(TraitDef.Named("AK_DisableAttack")));
            }
            if (this.Pawn.story.traits.HasTrait(TraitDef.Named("AK_AbilityCD")))
            {
                this.Pawn.story.traits.RemoveTrait(this.Pawn.story.traits.GetTrait(TraitDef.Named("AK_AbilityCD")));
            }
        }
    }
}
