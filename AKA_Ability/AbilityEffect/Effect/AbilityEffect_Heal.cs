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
        public override void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {
            if (target == null || !(target is Pawn t))
            {
                return;
            }
            //Pawn t = target as Pawn;
            if (!healNullFaction && t.Faction == null)
            {
                return;
            }

            if (t.Faction is Faction faction && user.Faction is Faction faction1)
            {
                if (faction != faction1)
                {
                    if (!healHostile && faction.RelationKindWith(faction1) == FactionRelationKind.Hostile)
                    {
                        return;
                    }
                    if (!healNeutral && faction.RelationKindWith(faction1) == FactionRelationKind.Neutral)
                    {
                        return;
                    }
                }
            }

            this.Heal(t);
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
            /*foreach (Hediff_Injury injury in target.health.hediffSet.GetInjuriesTendable().ToList())
            {
                if (heal > injury.Severity)
                {
                    injury.Heal(injury.Severity);
                    heal -= injury.Severity;
                }
                else
                {
                    injury.Heal(heal);
                    heal -= heal;
                    break;
                }
            }*/
        }
        public int healPoint;
        public bool healHostile;
        public bool healNeutral;
        public bool healNullFaction;
    }
}
