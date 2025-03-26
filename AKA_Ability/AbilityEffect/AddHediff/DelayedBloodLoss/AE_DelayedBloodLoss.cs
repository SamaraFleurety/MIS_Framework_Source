using AK_DLL;
using Verse;

namespace AKA_Ability
{
    /// <summary>
    /// 往目标身上加一个Hediff
    /// 效果为：delay * 单位时间后，每单位时间减少lossAmount的血，直到duration结束
    /// </summary>
    public class AE_DelayedBloodLoss : AbilityEffect_AddHediff
    {
        protected override void PreProcess()
        {
            this.hediffDef = DefDatabase<HediffDef>.GetNamed("AK_Hediff_BloodLoss");
        }

        protected override void postAddHediff(Hediff h)
        {
            HC_BloodLoss comp = (h as HediffWithComps).TryGetComp<HC_BloodLoss>();
            comp.Intervel = (int)this.unit;
            comp.Amount = this.lossAmount;
            comp.Duration = this.duration;
            comp.Delay = this.delay;
        }

        public int delay = 0;
        public TimeToTick unit = TimeToTick.hour; //unit
        public int duration = 10;
        public float lossAmount = 0.05f;
    }
}
