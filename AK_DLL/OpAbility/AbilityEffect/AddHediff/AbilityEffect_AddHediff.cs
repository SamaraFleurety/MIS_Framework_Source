using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class AbilityEffect_AddHediff : AbilityEffectBase
    {
        protected virtual void preProcess() { 
        }
        public override void DoEffect_Pawn(Pawn user, Thing target)
        {
            preProcess();
            if (onSelf) target = user;
            if (target == null || target.GetType() != typeof(Pawn))
            {
                return;
            }
            Pawn target_Pawn = target as Pawn;
            if (target_Pawn != null && !target_Pawn.Dead)
            {
                Hediff hediff = AddHediff(target_Pawn, this.hediffDef, this.bodyPart, this.severity);
                
                if (hediff != null) postAddHediff(hediff);
            }
            else
            {
                Log.Warning("目标为空或已死亡.");
            }
        }

        protected virtual void postAddHediff (Hediff h)
        {
        }

        public static Hediff AddHediff (Pawn target, HediffDef hediffDef, BodyPartDef part, float severity)
        {
            if (target == null) return null;
            Hediff hediff;
            if (part == null)
            {
                HealthUtility.AdjustSeverity(target, hediffDef, severity);
                hediff = target.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);
            }
            else
            {
                hediff = target.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                if (hediff != null)
                {
                    hediff.Severity += severity;
                    return hediff;
                }
                BodyPartRecord partRecord = null;
                foreach (BodyPartRecord bodyPart in target.RaceProps.body.AllParts)
                {
                    if (bodyPart.def == part)
                    {
                        partRecord = bodyPart;
                        break;
                    }
                }
                hediff = target.health.AddHediff(hediffDef, partRecord, null, null);
                hediff.Severity = severity;
            }
            return hediff;
        }

        public bool onSelf = false;
        public HediffDef hediffDef;
        public float severity = 1f;
        public BodyPartDef bodyPart;
    }
}