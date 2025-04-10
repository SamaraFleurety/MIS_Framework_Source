using AK_DLL.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AK_TypeDef
{
    [StaticConstructorOnStartup]
    public static class GenericUtilities
    {
        #region 文档系统
        public static T TryGetDoc<T>(this Thing t, string id = null) where T : DocumentBase
        {
            GC_Generic.instance.documents.TryGetValue(t, out DocumentTracker manager);
            if (manager == null) return default;

            id ??= typeof(T).FullName;

            manager.documents.TryGetValue(id, out DocumentBase res);
            return res as T;
        }

        public static void AddDoc<T>(this Thing t, T doc, string givenID = null) where T : DocumentBase
        {
            GC_Generic.instance.documents.TryGetValue(t, out DocumentTracker manager);
            if (manager == null)
            {
                manager = new DocumentTracker() { parent = t };
                GC_Generic.instance.documents.Add(t, manager);
            }
            givenID ??= typeof(T).FullName;
            if (manager.documents.ContainsKey(givenID))
            {
                Log.Error($"[AK] 尝试给{t} 重复的文档:{givenID}");
                return;
            }
            manager.documents.Add(givenID, doc);
        }
        #endregion

        #region 检索Hediff
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
        public static Hediff AddHediff(Pawn target, HediffDef hediffDef, BodyPartDef part = null, BodyPartRecord partRecord = null, float severity = 1, string customLabel = null)
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
                if (partRecord == null) partRecord = GetBodyPartRecord(target, part, customLabel);
                if (partRecord == null) return null;

                hediff = GetMatchedHediff(target, hediffDef, partRecord);
                if (hediff != null)
                {
                    hediff.Severity += severity;
                    return hediff;
                }
                if (severity <= 0) return null;
                hediff = target.health.AddHediff(hediffDef, partRecord, null, null);
                hediff.Severity = severity;
            }
            return hediff;
        }

        public static BodyPartRecord GetBodyPartRecord(Pawn p, BodyPartDef partDef, string customLabel = null)
        {
            if (p == null || p.Dead || partDef == null) return null;
            IEnumerable<BodyPartRecord> candidate = p.health.hediffSet.GetNotMissingParts();
            candidate = candidate.Where((BodyPartRecord record) => record.def == partDef);

            if (candidate == null || candidate.Count() == 0) return null;

            if (customLabel != null) candidate = candidate.Where((BodyPartRecord record) => record.untranslatedCustomLabel == customLabel);
            if (candidate == null || candidate.Count() == 0) return null;

            BodyPartRecord r = candidate.RandomElement();
            return r;
        }

        public static Hediff GetMatchedHediff(Pawn p, HediffDef hDef, BodyPartRecord partRecord)
        {
            if (p == null || p.Dead || hDef == null) return null;
            IEnumerable<Hediff> candidate = p.health.hediffSet.hediffs;
            candidate = candidate.Where((Hediff h) => (h.Part == partRecord && h.def == hDef));
            if (candidate == null || candidate.Count() == 0) return null;
            return candidate.RandomElement();
        }
        #endregion

        //检索圆形范围内可到达格子 轨道交易信标同款
        public static List<IntVec3> TradeableCellsAround(IntVec3 pos, Map map, float radius)
        {
            List<IntVec3> tradeableCells = new List<IntVec3>();
            //tradeableCells.Clear();
            if (!pos.InBounds(map))
            {
                return tradeableCells;
            }
            Region region = pos.GetRegion(map);
            if (region == null)
            {
                return tradeableCells;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 cell in r.Cells)
                {
                    if (cell.InHorDistOf(pos, radius))
                    {
                        tradeableCells.Add(cell);
                    }
                }
                return false;
            }, 16, RegionType.Set_Passable);
            return tradeableCells;
        }

        
    }
}
