using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_KillTarget : AbilityEffectBase
    {
        public bool destroyCorpse = false;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Thing targetThing = target.Thing;
            targetThing?.Kill();

            if (destroyCorpse)
            {
                Pawn pawn = targetThing as Pawn;
                pawn?.Corpse.Destroy();
            }
            return base.DoEffect(caster, target);
        }
    }
}
