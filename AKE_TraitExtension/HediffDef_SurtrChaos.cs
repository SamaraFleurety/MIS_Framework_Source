using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AK_DLL;

namespace AKE_TraitExtension
{
    //没写完
    public class Hediff_SurtrChaos : OperatorDef
    {
        #region 属性
        public Def def;

        string targetTraitDefName;
        #endregion 属性
        public OperatorHediffDef_SurtrChaos Def
        {
            get
            {
                return this.def as OperatorHediffDef_SurtrChaos;
            }
        }

        public void Get_Operator_Traitname()
        {
            targetTraitDefName = $"AK_Trait_{name}";
        }

        protected new void Recruit_PersonalStat()
        {
            //在operator_Pawn.story.traits.GainTrait后面加上 检测到特定干员(如42)的专属特性就开始执行疯狂随机变动属性修正
            Get_Operator_Traitname();
            foreach (TraitAndDegree trait in traits.Where(t => t.def != null && t.def.defName == targetTraitDefName))
            {
                //迷迭香也一起疯狂失忆
                if (targetTraitDefName == "AK_Trait_Surtr" || targetTraitDefName == "AK_Trait_Rosmontis")
                {
                    //记得用AbilityEffect_AddHediff重写这部分。
                    //先加上一个空壳子Hediff
                    var HediffOnPawn = operator_Pawn?.health?.hediffSet?.GetFirstHediffOfDef(Def.HediffToAdd);
                    var randomSeverity = Rand.Range(0.15f, 0.30f);
                    if (HediffOnPawn != null)
                    {
                        //如果干员的hediffSet列表中已经有了定义的HediffToAdd,则增加Hediff的严重程度
                        HediffOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        Hediff hediff = HediffMaker.MakeHediff(Def.HediffToAdd, operator_Pawn, null);
                        hediff.Severity = randomSeverity;
                        operator_Pawn.health.AddHediff(hediff, null, null);
                        //随机属性变动算法，我是懒狗还没写

                        string translatedMessage = TranslatorFormattedStringExtensions.Translate("Paluto22_SuccessMessage", Def.AddHediffChance.ToString("P"));
                        MoteMaker.ThrowText(operator_Pawn.PositionHeld.ToVector3(), operator_Pawn.MapHeld, translatedMessage, 2f);
                    }


                }

            }
            var rand = Rand.Value;
            if (rand <= Def.AddHediffChance)
            {
                string translatedMessage = TranslatorFormattedStringExtensions.Translate("SuccessMessage", operator_Pawn.Label, Def.HediffToAdd.label);
                Messages.Message(translatedMessage, MessageTypeDefOf.NegativeHealthEvent);

            }
            foreach (Hediff hediff_Pawn in operator_Pawn.health.hediffSet.hediffs)
            {
                if (this.hediffInate != null && this.hediffInate.Count > 0)
                {
                    operator_Pawn.health.AddHediff(hediff_Pawn);
                }
            }
        }
    }
}
