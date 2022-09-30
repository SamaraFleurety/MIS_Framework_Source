using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class AbilityEffect_AddHediff : AbilityEffectBase
    {
        public override void DoEffect_Pawn(Pawn user, Thing target)
        {
            if (target == null || target.GetType() != typeof(Pawn))
            {
                return;
            }
            Pawn target_Pawn = target as Pawn;
            if (target_Pawn != null && !target_Pawn.Dead)
            {
                if (this.bodyPart == null)
                {
                    HealthUtility.AdjustSeverity(target_Pawn, this.hediff, this.severity);
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
                    Hediff hediff = target_Pawn.health.AddHediff(this.hediff,part,null,null);
                    hediff.Severity = this.severity;
                }
            }
        }
        public HediffDef hediff;
        public float severity;
        public string bodyPart;
    }
}