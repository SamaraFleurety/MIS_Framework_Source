using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AK_DLL;
using UnityEngine;
using AKE_TraitExtension;

namespace AKE_OperatorExtension
{
    public class HCP_RandStats : HediffCompProperties
    {
        public int interval = 1;
        public TimeToTick intervalUnit = TimeToTick.day;
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
        //保存随机修正模板的特性List
        public List<TraitAndDegree> TraitSets => this.Props.TraitSets;
        //储存TraitSet的索引
        private HashSet<int> UsedIndex = new HashSet<int>();

        private static int index = 0;
        private int tick = 0;
        //检测条件为多少天
        private int TimerInterval_days(int value) => value * this.Props.interval * (int)this.Props.intervalUnit;

        public static bool hascurrentTrait = false;
        //读取干员身份证，非空
        private string OperatorID => this.Pawn.GetDoc()?.operatorID;
        private string GetOperatorTraitDef => $"AK_Trait_{OperatorID}";
        private string ThingdefName(int i) => AK_Tool.GetThingdefNameFrom((OperatorID + "Chaos" + i), "AKE", "Trait");
        #endregion
        //检查小人身上还有没有特定TraitDef
        private bool HasOperatorTraitDef(string XMLdefName)
        {
            for (int i = 0; i < this.Pawn.story.traits.allTraits.Count; i++)
            {
                if (this.Pawn.story.traits.allTraits[i].def.defName == DefDatabase<TraitDef>.GetNamed(XMLdefName).defName)
                    return true;
            }
            return false;
        }
        private Trait HC_GetCurrentTrait()
        {
            if (this.Pawn == null) return null;
            if (TraitSets == null) return null;
            for (int i = 0; i < this.Pawn.story.traits.allTraits.Count; i++)
            {
                for (int j = 0; i < TraitSets.Count; j++)
                {
                    if (this.Pawn.story.traits.HasTrait(TraitSets[j].def))
                    {
                        index = j;
                        return this.Pawn.story.traits.allTraits[i];
                    }
                }
            }
            return null;
        }
        private void HC_AddTrait()
        {
            if (this.Pawn == null) return;
            if (TraitSets == null) return;
            /*这部分今天懒得写先鸽了...记得使用index和UsedIndex计数检索
            foreach (TraitAndDegree TraitAndDegree in this.TraitSets)
            {
                TraitDegreeData data = TraitAndDegree.traitDef.degreeDatas[TraitAndDegree.degree];
                if (data.skillGains != null)
                {
                    index++;
                    this.Pawn.story.traits.GainTrait(new Trait(TraitAndDegree.traitDef, TraitAndDegree.degree));
                }
            }*/
            hascurrentTrait = true;
        }
        private void HC_RemoveTrait()
        {
            if (this.Pawn == null) return;
            if (TraitSets == null) return;
            if (hascurrentTrait)
            {
                this.Pawn.story.traits.RemoveTrait(HC_GetCurrentTrait());
                hascurrentTrait = false;
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            ++tick;
            if (tick >= TimerInterval_days(10))
            {
                if (!HasOperatorTraitDef(GetOperatorTraitDef)) { parent.comps.Remove(this); return; }
                HC_RemoveTrait();
                tick = 0;
                HC_AddTrait();
                index = 0;
                string translatedMessage = TranslatorFormattedStringExtensions.Translate("Phase1_SuccessMessage");
                MoteMaker.ThrowText(this.Pawn.PositionHeld.ToVector3(), this.Pawn.MapHeld, translatedMessage, 2f);
            }
        }
    }
}