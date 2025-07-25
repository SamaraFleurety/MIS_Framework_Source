﻿using System;
using Verse;

namespace AKA_Ability
{
    public class HCP_Regrow : HediffCompProperties
    {
        public int healInterval = 60;

        public float healAmount = 1f;

        public HCP_Regrow()
        {
            this.compClass = typeof(HC_Regrow);
        }
    }
    public class HC_Regrow : HediffComp
    {
        #region 属性
        public HCP_Regrow Props
        {
            get
            {
                return (HCP_Regrow)base.props;
            }
        }

        private HCP_Regrow exactProps = new HCP_Regrow();

        public int HealInterval
        {
            get
            {
                return this.exactProps.healInterval;
            }
            set
            {
                this.exactProps.healInterval = value;
            }
        }

        public float HealAmount
        {
            get
            {
                return this.exactProps.healAmount;
            }
            set
            {
                this.exactProps.healAmount = value;
            }
        }
        #endregion

        #region 规范化组件
        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = this.Props;
        }

        //指上去 显示hediff属性
        public override string CompLabelInBracketsExtra
        {
            get
            {
                return ($"\n剩余:{(base.parent.Severity * 10).ToString("0.0")}环时\n间隔:{this.HealInterval / 60}秒\n强度{this.HealAmount}");
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.healTick, "healTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.exactProps = this.Props;
            }
        }
        #endregion

        public bool isFinal;
        private int healTick = 0;

        private bool CanHealNow
        {
            get
            {
                if (base.Pawn.health.hediffSet.hediffs.Find((Hediff x) => x is Hediff_Injury y && (y.CanHealNaturally() || y.CanHealFromTending())) != null)
                    return true;
                else return false;
            }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.healTick++;
            if (this.healTick >= this.HealInterval)
            {
                this.healTick = 0;
                if (this.CanHealNow)
                {
                    this.HealRandomInjury(base.Pawn, this.HealAmount);
                    base.parent.Severity -= this.HealAmount * 0.01f;
                }
            }
        }

        private void HealRandomInjury(Pawn pawn, float points)
        {
            for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; ++i)
            {
                Hediff h = pawn.health.hediffSet.hediffs[i];
                if (h is Hediff_Injury)
                {
                    float sever = h.Severity;
                    points = Math.Max(points, sever);
                    h.Heal(points);
                    points -= sever;
                }
            }
            /*if (pawn.health.hediffSet.GetInjuriesTendable() != null && pawn.health.hediffSet.GetInjuriesTendable().Count() > 0)
            {
                Hediff_Injury x = pawn.health.hediffSet.GetInjuriesTendable().RandomElement();
                x.Heal(points);
            }*/
            //这函数被太难扬咯，改用上面的丐版方法
            /*wn.health.hediffSet.GetHediffs<Hediff_Injury>();
            if ((from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                 where x.CanHealNaturally() || x.CanHealFromTending()
                 select x).TryRandomElement(out Hediff_Injury hediff_Injury))
            {
                hediff_Injury.Heal(points);
            }*/
        }
    }
}
