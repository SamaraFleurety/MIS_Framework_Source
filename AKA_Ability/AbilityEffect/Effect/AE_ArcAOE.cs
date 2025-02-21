using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using UnityEngine;

namespace AKA_Ability.AbilityEffect
{
    public abstract class AE_ArcAOE : AbilityEffectBase
    {
        public float range = 10f, arc = 120f;
        public EffecterDef effecter;

        List<Thing> thingList = new();

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            var pawn = caster.CasterPawn;
            thingList = GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, range, true)
                .Where(t => t is IAttackTarget att && pawn.HostileTo(t) && !att.ThreatDisabled(pawn)).ToList();
            foreach (var thing in thingList)
            {
                if (Vector3.Angle(target.CenterVector3 - pawn.DrawPos, thing.DrawPos - pawn.DrawPos) * 2 > arc) continue;
                if (!GenSight.LineOfSightToThing(pawn.PositionHeld, thing, pawn.Map)) continue;

                DoEffectThing(thing, pawn);
            }
            return true;
        }

        protected virtual void DoEffectThing(Thing t, Pawn caster)
        {
            effecter?.Spawn(caster, t);
        }
    }
    public class AE_ArcDamageAOE : AE_ArcAOE
    {
        public IntRange damageRange = new(1, 2);
        public DamageDef damageDef;
        public FloatRange penetrationRange = new(-1, -1);

        protected override void DoEffectThing(Thing t, Pawn caster)
        {
            base.DoEffectThing(t, caster);
            t.TakeDamage(new DamageInfo(damageDef, damageRange.RandomInRange, penetrationRange.RandomInRange, instigator: caster, intendedTarget: t));
        }
    }
    public class AE_ArcStunAOE : AE_ArcAOE
    {
        public IntRange stunTickRange = new(1, 2);

        protected override void DoEffectThing(Thing t, Pawn caster)
        {
            base.DoEffectThing(t, caster);
            if (t is ThingWithComps twc)
            {
                twc.TryGetComp<CompStunnable>()?.StunHandler.StunFor(stunTickRange.RandomInRange, caster);
            }
        }
    }
}
