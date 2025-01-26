using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_MentalBreak : AbilityEffectBase
    {
        public string reason = "";
        public MentalBreakDef def;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Pawn t = target.Pawn;
            if (t ==  null || t.mindState == null || !t.mindState.mentalBreaker.CanDoRandomMentalBreaks) return false;
            t.mindState.mentalBreaker.TryDoMentalBreak(reason, def);
            return base.DoEffect(caster, target);
        }
    }
}
