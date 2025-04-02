using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AbilityEffect_Heal : AbilityEffectBase
    {
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.container == null) return false;

            Pawn user = caster.CasterPawn;
            Pawn targetPawn = target.Pawn;
            if (targetPawn == null || targetPawn == null)
            {
                return false;
            }
            //Log.Message($"[AK] {user.Name} try to heal {targetPawn.Name}");
            if (!healNullFaction && targetPawn.Faction == null)
            {
                return false;
            }

            if (targetPawn.Faction is Faction faction && user.Faction is Faction faction1)
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

            //Log.Message($"heal {targetPawn.Name} with {this.healPoint}");
            Heal(targetPawn, this.healPoint);
            return base.DoEffect(caster, target);
        }

        public static void Heal(Pawn target, float healPoint)
        {
            float heal = healPoint;
            for (int i = 0; i < target.health.hediffSet.hediffs.Count; ++i)
            {
                Hediff injury = target.health.hediffSet.hediffs[i];
                //Log.Message($"injury {injury.def.defName} at {injury.Severity} - heal {heal}");
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
                        //Log.Message($"ifin {injury.Severity}");
                        break;
                    }
                    //Log.Message($"ifin {injury.Severity}");
                }
            }
        }
        public int healPoint;
        public bool healHostile;
        public bool healNeutral;
        public bool healNullFaction;
    }
}
