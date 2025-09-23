using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AKS_Shield.Extension
{
    public class TCP_AddHediffWhileActive : CompProperties
    {
        public HediffDef hediff;
        public float severity = 10;
        public TCP_AddHediffWhileActive()
        {
            compClass = typeof(TC_AddHediffWhileActive);
        }
    }

    public class TC_AddHediffWhileActive : TC_ShieldExtension_Base
    {
        const int INTERVAL = 1250;

        public TCP_AddHediffWhileActive Props => props as TCP_AddHediffWhileActive;
        public override void Tick(int amt)
        {
            if (TickNow % INTERVAL == 0 && Wearer != null && Wearer.Drafted && HasEnergyLeft)
            {
                AddHediff(Wearer, Props.hediff, severity: Props.severity);
            }
        }

        //有一定可能该写个泛用工具dll了
        public static Hediff AddHediff(Pawn target, HediffDef hediffDef, BodyPartDef part = null, BodyPartRecord partRecord = null, float severity = 1f)
        {
            if (target == null)
            {
                return null;
            }

            Hediff hediff;
            if (part == null && partRecord == null)
            {
                HealthUtility.AdjustSeverity(target, hediffDef, severity);
                hediff = target.health.hediffSet.GetFirstHediffOfDef(hediffDef);
            }
            else
            {
                partRecord ??= GetBodyPartRecord(target, part);

                if (partRecord == null)
                {
                    return null;
                }

                hediff = GetMatchedHediff(target, hediffDef, partRecord);
                if (hediff != null)
                {
                    hediff.Severity += severity;
                    return hediff;
                }

                if (severity <= 0f)
                {
                    return null;
                }

                hediff = target.health.AddHediff(hediffDef, partRecord);
                hediff.Severity = severity;
            }

            return hediff;
        }

        public static BodyPartRecord GetBodyPartRecord(Pawn p, BodyPartDef partDef)
        {
            if (p == null || p.Dead || partDef == null)
            {
                return null;
            }

            IEnumerable<BodyPartRecord> notMissingParts = p.health.hediffSet.GetNotMissingParts();
            notMissingParts = notMissingParts.Where((BodyPartRecord record) => record.def == partDef);
            if (notMissingParts == null || notMissingParts.Count() == 0)
            {
                return null;
            }

            return notMissingParts.RandomElement();
        }

        private static Hediff GetMatchedHediff(Pawn p, HediffDef hDef, BodyPartRecord partRecord)
        {
            if (p == null || p.Dead || hDef == null)
            {
                return null;
            }

            IEnumerable<Hediff> hediffs = p.health.hediffSet.hediffs;
            hediffs = hediffs.Where((Hediff h) => h.Part == partRecord && h.def == hDef);
            if (hediffs == null || hediffs.Count() == 0)
            {
                return null;
            }

            return hediffs.RandomElement();
        }
    }
}
