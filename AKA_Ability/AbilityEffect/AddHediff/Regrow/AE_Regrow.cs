﻿using System;
using Verse;

namespace AKA_Ability
{
    //此技能一般以1秒，而不是1小时计时
    public class AE_Regrow : AbilityEffect_AddHediff
    {
        float lastTime = 1f; //单位是小时。每小时是2500tick, 每10小时是1严重度。
        int healInterval = 60; //每多少tick治疗一次。60tick是现实中的一秒
        float healAmount = 10f; //每次治疗量
        RegrowType regrowType = RegrowType.compare;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            this.hediffDef = DefDatabase<HediffDef>.GetNamed("AK_Hediff_Regrow");
            this.severity = lastTime * 0.1f;
            /*if (target == null || target.GetType() != typeof(Pawn))
            {
                return false;
            }*/
            Pawn target_Pawn = target.Pawn;
            if (target_Pawn != null && !target_Pawn.Dead)
            {
                bool newlyAdded = false;
                HediffWithComps hediffRegrow = target_Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef) as HediffWithComps;
                if (hediffRegrow == null)
                {
                    HealthUtility.AdjustSeverity(target_Pawn, this.hediffDef, 0.01f);
                    newlyAdded = true;
                }
                hediffRegrow = target_Pawn.health.hediffSet.GetFirstHediffOfDef(hediffDef) as HediffWithComps;
                HC_Regrow hediffComp = hediffRegrow.TryGetComp<HC_Regrow>();
                //调整再生的数值
                if (hediffComp == null) Log.Error("治疗Hediff的Comp是空的");
                else
                {
                    if (this.regrowType == RegrowType.enhance)
                    {
                        hediffComp.HealInterval = Math.Min(hediffComp.HealInterval, this.healInterval);
                        hediffComp.HealAmount = Math.Max(hediffComp.HealAmount, this.healAmount);
                        hediffComp.parent.Severity += this.severity;
                    }
                    else if (newlyAdded || (this.regrowType == RegrowType.compare && NewRegrowIsBetter(hediffComp)) || this.regrowType == RegrowType.replace || this.regrowType == RegrowType.chronic || hediffComp.isFinal == true)
                    {
                        hediffComp.HealInterval = this.healInterval;
                        hediffComp.HealAmount = this.healAmount;
                        hediffComp.parent.Severity = this.severity;
                        hediffComp.isFinal = this.regrowType == RegrowType.chronic ? true : false;
                    }
                }
            }
            return base.DoEffect(caster, target);
        }

        private bool NewRegrowIsBetter(HC_Regrow oldRegrow)
        {
            if (((this.severity + 0.02) * this.healAmount / (float)this.healInterval) > (oldRegrow.parent.Severity * oldRegrow.HealAmount / (float)oldRegrow.HealInterval)) return true;
            return false;
        }
    }
}
