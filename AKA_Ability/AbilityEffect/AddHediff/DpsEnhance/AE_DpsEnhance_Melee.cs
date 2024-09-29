using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class toolEnhance
    {
        public float originalPower;            //必须提供工具的原始攻击力来定位工具
        public float originalCD;               //也必须提供原始cd
        public float powerOffsetTotal = 0;     //最终希望增减多少攻击力
        public float CDOffsetTotal = 0;        //最终希望增减多少cd
        public Tool tool = null;               //缓存
        public bool shouldSkip = false;        //缓存
    }
    class AE_DpsEnhance_Melee : AbilityEffect_AddHediff
    {
        List<toolEnhance> enhances;
        public int interval = 1;                //多少 秒 渐进一次。默认是1，不建议改。
        public int procedureCount = 1;          //一共渐进多少次来增加攻击力。应该小于总渐进次数
        public int enhanceEndTime = 2;          //渐进多少次后这个buff消失(上个渐进次数内会缓慢强化伤害，而到了这个时间所有buff会消失。)

        public AE_DpsEnhance_Melee()
        {
            //this.onSelf = true;
            this.severity = 1f;
            this.enhances = new List<toolEnhance>();
        }

        protected override void PreProcess()
        {
            this.hediffDef = DefDatabase<HediffDef>.GetNamed("AK_Hediff_EnhancerMelee");
        }

        protected override void postAddHediff(Hediff h)
        {
            HC_DpsEnhance_Melee comp = (h as HediffWithComps).TryGetComp<HC_DpsEnhance_Melee>(); 
            if (comp == null)
            {
                Log.Error("因为未知原因," + h.pawn.Name + "身上的近战增伤hediff组件错误");
            }
            else
            {
                if (comp.Enhances == null) comp.Enhances = new List<toolEnhance>();
                comp.Enhances = this.enhances;
                comp.Interval = this.interval;
                comp.ProcedureCount = this.procedureCount;
                comp.EnhanceEndTime = this.enhanceEndTime;
            }
        }
    }
}
