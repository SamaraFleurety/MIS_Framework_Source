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
        public override void DoEffect_Pawn(Pawn user, Thing target)
        {
            if (target == null|| target.GetType() != typeof(Pawn))
            {
                return;
            }
            Pawn pawn = target as Pawn;
            if (pawn.Faction == null&&this.healNullFaction == true) 
            {
                this.Heal(pawn);
            }

            if (pawn.Faction is Faction faction&&user.Faction is Faction faction1) 
            {
                if (faction != faction1)
                {
                    if (faction.RelationKindWith(faction1) == FactionRelationKind.Hostile && this.healHostile)
                    {
                        this.Heal(pawn);
                    }
                    if (faction.RelationKindWith(faction1) == FactionRelationKind.Neutral && this.healNeutral)
                    {
                        this.Heal(pawn);
                    }
                    if (faction.RelationKindWith(faction1) == FactionRelationKind.Ally)
                    {
                        this.Heal(pawn);
                    }
                }
                else 
                {
                    this.Heal(pawn);
                }
            }

        }

        private void Heal(Pawn target) 
        {
            float heal = (float)this.healPoint;
            foreach (Hediff_Injury injury in target.health.hediffSet.GetInjuriesTendable().ToList())
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
            }
        }
        public int healPoint;
        public bool healHostile;
        public bool healNeutral;
        public bool healNullFaction;
    }
}
