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
                Hediff hediff;
                if (this.bodyPart == null)
                {
                    HealthUtility.AdjustSeverity(target_Pawn, this.hediffDef, this.severity);
                    hediff = target_Pawn.health.hediffSet.GetFirstHediffOfDef(this.hediffDef, false);
                }
                else
                {
                    BodyPartRecord part = null;
                    foreach (BodyPartRecord bodyPart in target_Pawn.RaceProps.body.AllParts) 
                    {
                        if (bodyPart.Label == this.bodyPart)
                        {
                            part = bodyPart;
                        }
                    }
                    hediff = target_Pawn.health.AddHediff(this.hediffDef,part,null,null);
                    hediff.Severity = this.severity;
                }
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

        public bool onSelf = false;
        public HediffDef hediffDef;
        public float severity = 1f;
        public string bodyPart;
    }
}