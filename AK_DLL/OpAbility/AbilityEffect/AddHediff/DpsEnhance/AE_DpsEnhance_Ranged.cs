using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    /*public class RangedStat
    {
        public List<StatModifier> stats = new List<StatModifier>();
        /*public float accuTouch = 0;
        public float accuShort = 0;
        public float accuMed = 0;
        public float accuLong = 0;
        public float postCD = 0;*/  //包含在上面了
    /*
        public float preCD = 0;
        public float range = 0;
        public float armorPenetrate = 0;
        public float bulletSpeed = 0;
        public int damage = 0;
        public int burstCount = 0;
        public int burstInterval = 0;
        public String damageType = null;
        public String bullet = null;
        public String fireSound = null;
    }
    public class AE_DpsEnhance_Ranged : AbilityEffect_AddHediff
    {
        RangedStat statOffset;
        int duration = 1;

        public AE_DpsEnhance_Ranged()
        {
            this.severity = 1;
            this.onSelf = true;
            this.statOffset = new RangedStat();
        }

        protected override void preProcess()
        {
            this.hediffDef = DefDatabase<HediffDef>.GetNamed("AK_Hediff_EnhancerRanged");
        }

        protected override void postAddHediff(Hediff h)
        {
            HC_DpsEnhance_Ranged comp = (h as HediffWithComps).TryGetComp<HC_DpsEnhance_Ranged>(); 
            if (comp == null)
            {
                Log.Error("因为未知原因," + h.pawn.Name + "身上的远程增伤hediff组件错误");
            }
            else
            {
                comp.Duration = this.duration;
                comp.StatOffset = this.statOffset;
            }
        }
    }*/
}
