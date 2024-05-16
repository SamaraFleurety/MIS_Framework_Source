using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AK_DLL;
using UnityEngine;

namespace AKE_OperatorExtension
{
    public class HCP_RandStats : HediffCompProperties
    {
        public int interval = 10;
        public int positiveinterval = 3;
        public TimeToTick intervalUnit = TimeToTick.day;
        public List<TraitAndDegree> positiveTraitSets = new List<TraitAndDegree>();
        public List<TraitAndDegree> TraitSets = new List<TraitAndDegree>();
        public HCP_RandStats()
        {
            this.compClass = typeof(HC_RandStats);
        }
    }
    public class HC_RandStats : HediffComp
    {
        #region
        public HCP_RandStats Props => (HCP_RandStats)this.props;
        //保存随机修正模板 & 正面特性修正的List
        public List<TraitAndDegree> TraitSets => this.Props.TraitSets;
        public List<TraitAndDegree> PositiveTraitSets => this.Props.positiveTraitSets;
        //储存TraitSet的索引
        //private HashSet<int> UsedIndex = new HashSet<int>();
        private Dictionary<List<TraitAndDegree>, HashSet<int>> usedIndices = new Dictionary<List<TraitAndDegree>, HashSet<int>>();

        private static int index = 0;
        private int tick = 0;
        //检测条件为多少天
        private int TimerInterval_days => this.Props.interval * (int)this.Props.intervalUnit;
        private int positiveTimerInterval_days => this.Props.positiveinterval * (int)this.Props.intervalUnit;
        //读取干员身份证，非空
        private string OperatorID => this.Pawn.GetDoc()?.operatorID;
        private string GetOperatorTraitDef => $"AK_Trait_{OperatorID}";
        #endregion
        //检查小人身上还有没有特定TraitDef
        public static bool hascurrentTrait = false;
        public static bool haseatbingchilling = false;
        private bool HasOperatorTraitDef(string XMLdefName)
        {
            for (int i = 0; i < this.Pawn.story.traits.allTraits.Count; i++)
            {
                if (this.Pawn.story.traits.allTraits[i].def.defName == DefDatabase<TraitDef>.GetNamed(XMLdefName).defName)
                    return true;
            }
            return false;
        }
        private Trait HC_GetCurrentTrait(List<TraitAndDegree> traitset)
        {
            if (this.Pawn == null) return null;
            if (traitset == null) return null;
            for (int i = 0; i < traitset.Count; i++)
            {
                if (this.Pawn.story.traits.HasTrait(traitset[i].def))
                {
                    index = i;
                    return this.Pawn.story.traits.GetTrait(traitset[i].def);
                }
            }

            return null;
        }
        private void HC_AddTrait(List<TraitAndDegree> traitset)
        {
            if (this.Pawn == null || traitset == null || traitset.Count == 0) return;
            if (!usedIndices.TryGetValue(traitset, out var currentUsedIndex))
            {
                currentUsedIndex = new HashSet<int>();
                usedIndices[traitset] = currentUsedIndex;
            }
            if (currentUsedIndex.Count == traitset.Count)
            {
                currentUsedIndex.Clear();
                index = 0;
            }
            bool traitAdded = false;
            while (!traitAdded)
            {
                // 如果当前index已被使用，寻找下一个未使用的索引
                if (currentUsedIndex.Contains(index))
                {
                    index = (index + 1) % traitset.Count; // 循环至下一个索引
                    continue;
                }
                TraitAndDegree TraitAndDegree = traitset[index];
                TraitDegreeData data = TraitAndDegree.def.degreeDatas[TraitAndDegree.degree];
                if (data.skillGains != null)
                {
                    this.Pawn.story.traits.GainTrait(new Trait(TraitAndDegree.def, TraitAndDegree.degree));
                    currentUsedIndex.Add(index);
                    traitAdded = true;
                }
                index = (index + 1) % traitset.Count;
            }
            hascurrentTrait = true;
        }
        private void HC_RemoveTrait(List<TraitAndDegree> traitset)
        {
            if (this.Pawn == null || traitset == null || traitset.Count == 0) return;
            if (hascurrentTrait)
            {
                this.Pawn.story.traits.RemoveTrait(HC_GetCurrentTrait(traitset));
                hascurrentTrait = false;
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            ++tick;
            /*给冰淇淋用if (tick >= positiveTimerInterval_day && bingchilling)
            {
                if (!HasOperatorTraitDef(GetOperatorTraitDef)) { parent.comps.Remove(this); return; }
                if (HC_GetCurrentTrait(PositiveTraitSets) == null) { tick = 0; return; }
                tick = 0;
                HC_RemoveTrait(PositiveTraitSets);
                HC_AddTrait(TraitSets);
            }*/
            if (tick >= TimerInterval_days)
            {
                if (!HasOperatorTraitDef(GetOperatorTraitDef)) { parent.comps.Remove(this); return; }
                if (HC_GetCurrentTrait(TraitSets) == null) { tick = 0; return; }
                tick = 0;
                HC_RemoveTrait(TraitSets);
                HC_AddTrait(TraitSets);
                string translatedMessage = TranslatorFormattedStringExtensions.Translate("Phase1_SuccessMessage");
                MoteMaker.ThrowText(this.Pawn.PositionHeld.ToVector3(), this.Pawn.MapHeld, translatedMessage, 2f);
            }
        }
    }
}