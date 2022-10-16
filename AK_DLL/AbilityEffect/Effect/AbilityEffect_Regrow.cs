using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class AbilityEffect_Regrow : AbilityEffect_AddHediff
    {
        float lastTime = 1f; //单位是小时。每小时是2500tick, 每10小时是1严重度。
        int healInterval = 60; //每多少tick治疗一次。60tick是现实中的一秒
        float healAmount = 10f; //每次治疗量
        RegrowType regrowType = RegrowType.compare;

        public override void DoEffect_Pawn(Pawn user, Thing target)
        {
            this.hediff = DefDatabase<HediffDef>.GetNamed("AK_Hediff_Regrow");
            if (target == null || target.GetType() != typeof(Pawn))
            {
                return;
            }
            Pawn target_Pawn = target as Pawn;
            this.severity = lastTime * 0.1f;
            if (target_Pawn != null && !target_Pawn.Dead)
            {
                bool newlyAdded = false;
                HediffWithComps hediffRegrow = target_Pawn.health.hediffSet.GetFirstHediffOfDef(hediff) as HediffWithComps;
                if (hediffRegrow == null)
                {
                    HealthUtility.AdjustSeverity(target_Pawn, this.hediff, 0.01f);
                    newlyAdded = true;
                }
                hediffRegrow = target_Pawn.health.hediffSet.GetFirstHediffOfDef(hediff) as HediffWithComps;
                AK_HediffComp_Regrow hediffComp = null;
                foreach (HediffComp comp in hediffRegrow.comps)
                {
                    if (comp is AK_HediffComp_Regrow)
                    {
                        hediffComp = comp as AK_HediffComp_Regrow;
                        break;
                    }
                }
                if (hediffComp == null) Log.Error("治疗Hediff的Comp是空的");
                else
                {
                    if (this.regrowType == RegrowType.enhance)
                    {
                        hediffComp.HealInterval = Math.Min(hediffComp.HealInterval, this.healInterval);
                        hediffComp.HealAmount = Math.Max(hediffComp.HealAmount, this.healAmount);
                        hediffComp.parent.Severity += this.severity;
                    }
                    else if (newlyAdded || (this.regrowType == RegrowType.compare && NewRegrowIsBetter(hediffComp))|| this.regrowType == RegrowType.replace || this.regrowType == RegrowType.chronic || hediffComp.isFinal == true)
                    {
                        hediffComp.HealInterval = this.healInterval;
                        hediffComp.HealAmount = this.healAmount;
                        hediffComp.parent.Severity = this.severity;
                        hediffComp.isFinal = this.regrowType == RegrowType.chronic ? true : false;
                    }
                }
                //HealthUtility.AdjustSeverity(target_Pawn, this.hediff, this.severity);
            }
        }

        private bool NewRegrowIsBetter(AK_HediffComp_Regrow oldRegrow)
        {
            if (((this.severity + 0.02) * this.healAmount / (float)this.healInterval) > (oldRegrow.parent.Severity * oldRegrow.HealAmount / (float)oldRegrow.HealInterval)) return true;
            return false;
        }
    }

    public class AK_HediffComp_Regrow : HediffComp
    {
        public HediffCompProperties_Regrow Props
        {
            get
            {
                return (HediffCompProperties_Regrow)base.props;
            }
        }

        private HediffCompProperties_Regrow exactProps = new HediffCompProperties_Regrow();

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

        public bool isFinal;
        private int healTick;

        private bool CanHealNow
        {
            get
            {
                if (base.Pawn.health.hediffSet.hediffs.Find((Hediff x) => x is Hediff_Injury y && (y.CanHealNaturally() || y.CanHealFromTending())) != null)
                    return true;
                else return false;
            }
        }

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
            //FIXME:晚点修
            /*wn.health.hediffSet.GetHediffs<Hediff_Injury>();
            if ((from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                 where x.CanHealNaturally() || x.CanHealFromTending()
                 select x).TryRandomElement(out Hediff_Injury hediff_Injury))
            {
                hediff_Injury.Heal(points);
            }*/
        }

    }

    public class HediffCompProperties_Regrow : HediffCompProperties
    {
        public int healInterval = 60;

        public float healAmount = 1f;

        public HediffCompProperties_Regrow()
        {
            this.compClass = typeof(AK_HediffComp_Regrow);
        }
    }

}
