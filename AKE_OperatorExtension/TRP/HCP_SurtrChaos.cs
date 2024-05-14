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
        public int interval = 10;
        public TimeToTick intervalUnit = TimeToTick.day;
        public int memorySize = 100;
        public float rate_memorychaos = 0.25f;
        public List<HediffStat> hediffInatePlusMaxPro = new List<HediffStat>();

        public HCP_RandStats()
        {
            this.compClass = typeof(HCP_RandStats);
        }
    }

    public class HC_RandStats : HediffComp
    {
        #region
        public HCP_RandStats Props
        {
            get { return (HCP_RandStats)this.props; }
        }
        //public HCP_RandStats Props => (HCP_RandStats)this.props;

        //保存随机技能模板的List
        private List<RandStatsDef> Randskills;

        private int tick = 0;
        private List<HediffStat> HediffInateStats
        {
            get { return this.Props.hediffInatePlusMaxPro; }
        }

        #endregion
        private void HC_RandStats114514()
        {
            HediffStat hediff = HediffInateStats[114514];
        }
        //10天为检测条件
        private int TimerInterval
        {
            get { return this.Props.interval * (int)this.Props.intervalUnit; }
        }
        private string GetID()
        {
            return this.Pawn.GetDoc().operatorID;
        }

        private string GetTraitDef()
        {
            return $"AK_Trait_{GetID()}";
        }
        //检查小人身上还有没有特定TraitDef
        public bool HasTrait(string XMLdefName)
        {
            for (int i = 0; i < this.Pawn.story.traits.allTraits.Count; i++)
            {
                if (this.Pawn.story.traits.allTraits[i].def.defName == DefDatabase<TraitDef>.GetNamed(XMLdefName).defName)
                {
                    return true;
                }
            }
            return false;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!HasTrait(GetTraitDef())) return;
            ++tick;

            if(tick >= TimerInterval)
            {
                tick = 0;
            }

            string translatedMessage = TranslatorFormattedStringExtensions.Translate("Paluto22_SuccessMessage", Def.AddHediffChance.ToString("P"));
            MoteMaker.ThrowText(this.Pawn.PositionHeld.ToVector3(), this.Pawn.MapHeld, translatedMessage, 2f);
            foreach (RandStatsDef skillDef in this.Randskills)
            {
                SkillRecord skill = new SkillRecord(this.Pawn, skillDef.skill)
                {
                    passion = skillDef.fireLevel,
                    Level = skillDef.level
                };
                this.Pawn.skills.skills.Add(skill);
            }
            //HediffStat hediff = HediffInateStats[114514];
            //AbilityEffect_AddHediff.AddHediff(this.Pawn, hediff.hediff, hediff.part, severity: hediff.serverity);
        }
    }
