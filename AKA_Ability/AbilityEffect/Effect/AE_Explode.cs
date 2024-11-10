using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_Explode : AbilityEffectBase
    {
        public float radius = 7.9f;
        public int damage = 20;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Pawn casterPawn = caster.CasterPawn;
            GenExplosion.DoExplosion(target.Cell, casterPawn.Map, radius, DamageDefOf.Flame, null, damage, -1f, null, null, null, null, null, 0f, 1, null, true, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f);
            return base.DoEffect(caster, target);
        }
    }
}
