using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AbilityEffect_Heal : AbilityEffectBase
    {
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Pawn user = caster.CasterPawn;
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null || !(targetPawn is Pawn t))
            {
                return false;
            }
            if (!healNullFaction && t.Faction == null)
            {
                return false;
            }

            if (t.Faction is Faction faction && user.Faction is Faction faction1)
            {
                if (faction != faction1)
                {
                    if (!healHostile && faction.RelationKindWith(faction1) == FactionRelationKind.Hostile)
                    {
                        return false;
                    }
                    if (!healNeutral && faction.RelationKindWith(faction1) == FactionRelationKind.Neutral)
                    {
                        return false;
                    }
                }
            }

            this.Heal(t);
            return base.DoEffect(caster, target);
        }

        private void Heal(Pawn target)
        {
            float heal = (float)this.healPoint;
            for (int i = 0; i < target.health.hediffSet.hediffs.Count; ++i)
            {
                Hediff injury = target.health.hediffSet.hediffs[i];
                if (injury is Hediff_Injury)
                {
                    if (heal > injury.Severity)
                    {
                        heal -= injury.Severity;
                        injury.Heal(injury.Severity);
                    }
                    else
                    {
                        injury.Heal(heal);
                        heal -= heal;
                        break;
                    }
                }
            }
        }
        public int healPoint;
        public bool healHostile;
        public bool healNeutral;
        public bool healNullFaction;
    }
}
