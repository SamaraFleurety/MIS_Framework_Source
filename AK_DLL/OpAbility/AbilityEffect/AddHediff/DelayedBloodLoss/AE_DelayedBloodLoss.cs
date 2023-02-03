using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AK_DLL
{
    public class AE_DelayedBloodLoss :AbilityEffect_AddHediff
    {
        protected override void preProcess()
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
