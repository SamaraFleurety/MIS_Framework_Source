using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AKA_Ability
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
                Hediff hediff = AddHediff(target_Pawn, this.hediffDef, this.bodyPart, severity : this.severity);
                
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

        /// <summary>
        /// 分支:
        ///     如果part与part record都为空，那就直接加在全身
        ///         然后直接调整严重度，ret
        ///     如果part record不为空，直接使用；否则依据part随机检索一个。
        ///         -如果还是检索不到那就直接ret空 (指定的part已经不存在)
        ///         检索此部位是否已有Hediff，如果有就直接调整严重度。
        ///         如果没有
        ///             -如果严重度小于等于0(即减少严重度)，那就ret空 (减少不存在的hediff没有意义)
        ///             -否则就增加此Hediff并调整严重度。
        /// </summary>
        public static Hediff AddHediff (Pawn target, HediffDef hediffDef, BodyPartDef part = null, BodyPartRecord partRecord = null, float severity = 1)
        {
            if (target == null) return null;

            Hediff hediff;
            if (part == null && partRecord == null)
            {
                //会拿到第一个此def的hediff然后调整严重度。如果拿不到就会新建一个，part record为null
                HealthUtility.AdjustSeverity(target, hediffDef, severity);
                hediff = target.health.hediffSet.GetFirstHediffOfDef(hediffDef, false);
            }
            else
            {
                if (partRecord == null) partRecord = GetBodyPartRecord(target, part);
                if (partRecord == null) return null;

                //hediff = target.health.hediffSet.GetFirstHediffOfDef(hediffDef);
                hediff = GetMatchedHediff(target, hediffDef, partRecord);
                if (hediff != null)
                {
                    hediff.Severity += severity;
                    return hediff;
                }
                /*BodyPartRecord partRecord1 = null;
                foreach (BodyPartRecord bodyPart in target.RaceProps.body.AllParts)
                {
                    if (bodyPart.def == part)
                    {
                        partRecord1 = bodyPart;
                        break;
                    }
                }*/
                if (severity <= 0) return null;
                hediff = target.health.AddHediff(hediffDef, partRecord, null, null);
                hediff.Severity = severity;
            }
            return hediff;
        }

        public static BodyPartRecord GetBodyPartRecord(Pawn p, BodyPartDef partDef)
        {
            if (p == null || p.Dead || partDef == null) return null;
            IEnumerable<BodyPartRecord> candidate = p.health.hediffSet.GetNotMissingParts();
            candidate = candidate.Where((BodyPartRecord record) => record.def == partDef);
            if (candidate == null || candidate.Count() == 0) return null;
            BodyPartRecord r = candidate.RandomElement();
            return r;
        }

        private static Hediff GetMatchedHediff(Pawn p, HediffDef hDef, BodyPartRecord partRecord)
        {
            if (p == null || p.Dead || hDef == null) return null;
            IEnumerable<Hediff> candidate = p.health.hediffSet.hediffs;
            candidate = candidate.Where((Hediff h) => (h.Part == partRecord && h.def == hDef));
            if (candidate == null || candidate.Count() == 0) return null;
            return candidate.RandomElement();
        }

        public bool onSelf = false;
        public HediffDef hediffDef;
        public float severity = 1f;
        public BodyPartDef bodyPart;
    }
}