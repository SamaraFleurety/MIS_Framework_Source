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
        public List<HediffStat> hediffInatePlusMaxPro = new List<HediffStat>();

        public HCP_RandStats()
        {
            this.compClass = typeof(HC_RandStats);
        }
    }

    public class HC_RandStats : HediffComp
    {
        #region
        //public HCP_RandStats Props => (HCP_RandStats)this.props;
        public HCP_RandStats Props
        {
            get { return (HCP_RandStats)this.props; }
        }
        //保存随机技能模板的List
        private List<RandStatsDef> Randskills;
        private int index = 0;
        private int tick = 0;
        private int tick_phase2 = 0;
        private int tick_phase3 = 0;
        private int tick_bingchilling = 0;
        /*private List<HediffStat> HediffInateStats
        {
            get { return this.Props.hediffInatePlusMaxPro; }
        }*/
        #endregion
        //检测条件为冰淇淋增益3天，一阶段混乱10天,二阶段15天，三阶段20天
        private int TimerInterval_Bingchilling
        {
            get { return 3 * this.Props.interval * (int)this.Props.intervalUnit; }
        }

        private int TimerInterval_Phase1
        {
            get { return 10 * this.Props.interval * (int)this.Props.intervalUnit; }
        }
        private int TimerInterval_Phase2
        {
            get { return 15 * (this.Props.interval) * (int)this.Props.intervalUnit; }
        }
        private int TimerInterval_Phase3
        {
            get { return 20 * (this.Props.interval) * (int)this.Props.intervalUnit; }
        }

        private string GetID()
        {
            return this.Pawn.GetDoc()?.operatorID;
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
        //一阶段修正(-5,15)二阶段(-5,10)三阶段(-5,5)
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!HasTrait(GetTraitDef())) return;
            base.parent.Severity = 0.1f;
            if (base.parent.Severity <= 2.6f) ++tick;
            if (base.parent.Severity >= 2.6f) ++tick_phase2;
            if (base.parent.Severity >= 6.6f) ++tick_phase3;

            if (base.parent.Severity < 0.1f)
            {
                ++tick_bingchilling;
                if (tick_bingchilling >= TimerInterval_Bingchilling)
                {
                    foreach (RandStatsDef skillDef in this.Randskills)
                    { SetSkill(skillDef, 20); }
                    tick_bingchilling = 0;
                }
            }
            if (tick >= TimerInterval_Phase1)
            {
                tick = 0;
                foreach (RandStatsDef skillDef in this.Randskills)
                {
                    index++;
                    if (index == 7)
                    { SetSkill(skillDef, 16); continue; }
                    RandSkill(skillDef, -6, 16);
                }
                index = 0;
                base.parent.Severity += 0.5f;
                string translatedMessage = TranslatorFormattedStringExtensions.Translate("Phase1_SuccessMessage");
                MoteMaker.ThrowText(this.Pawn.PositionHeld.ToVector3(), this.Pawn.MapHeld, translatedMessage, 2f);
            }
            if (tick_phase2 >= TimerInterval_Phase2)
            {
                tick_phase2 = 0;
                foreach (RandStatsDef skillDef in this.Randskills)
                {
                    index++;
                    if (index == 7)
                    { SetSkill(skillDef, 16); continue; }
                    RandSkill(skillDef, -6, 11);
                }
                index = 0;
                base.parent.Severity += 1f;
                string translatedMessage = TranslatorFormattedStringExtensions.Translate("Phase2_SuccessMessage");
                MoteMaker.ThrowText(this.Pawn.PositionHeld.ToVector3(), this.Pawn.MapHeld, translatedMessage, 2f);
            }
            if (tick_phase3 >= TimerInterval_Phase3)
            {
                tick_phase3 = 0;
                foreach (RandStatsDef skillDef in this.Randskills)
                {
                    index++;
                    if (index == 7)
                    { SetSkill(skillDef, 16); continue; }
                    RandSkill(skillDef, -6, 6);
                }
                index = 0;
                string translatedMessage = TranslatorFormattedStringExtensions.Translate("Phase3_SuccessMessage");
                MoteMaker.ThrowText(this.Pawn.PositionHeld.ToVector3(), this.Pawn.MapHeld, translatedMessage, 2f);
            }
        }

        private void RandSkill(RandStatsDef skillDef, int min, int max)
        {
            int randomAdjustment = RandomLevelAdjustment(min, max);
            int adjustedLevel = skillDef.level + randomAdjustment;
            adjustedLevel = Math.Min(adjustedLevel, 20);
            SkillRecord skill = new SkillRecord(this.Pawn, skillDef.skill)
            {
                passion = skillDef.fireLevel,
                Level = adjustedLevel
            };
            this.Pawn.skills.skills.Add(skill);
        }

        private void SetSkill(RandStatsDef skillDef, int value)
        {
            int adjustedLevel = value;
            SkillRecord skill = new SkillRecord(this.Pawn, skillDef.skill)
            {
                passion = skillDef.fireLevel,
                Level = adjustedLevel
            };
            this.Pawn.skills.skills.Add(skill);
        }
        //调整小人的火有刷属性嫌疑 先弃用
        /*private byte RandomPassionAdjustment(int min,int max)
        {
            byte RandomPassion = (byte)new System.Random().Next(min, max);
            return RandomPassion; 
        }*/
        private int RandomLevelAdjustment(int min, int max)
        {
            int RandomLevel = new System.Random().Next(min, max);
            return RandomLevel;
        }
    }
}