using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AKA_Ability
{
    //此CD逻辑统一为：初始CD = MaxCD, 每tick - 1，到0时增加1Charge
    public class CDandCharge : IExposable
    {
        public CDandCharge()
        {
        }

        public CDandCharge(int CD, int maxCharge, int maxCD)
        {
            this.CD = CD;
            this.maxCharge = maxCharge;
            this.charge = maxCharge;
            this.maxCD = maxCD;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref CD, "CD");
            Scribe_Values.Look(ref maxCD, "maxCD");
            Scribe_Values.Look(ref charge, "charge");
            Scribe_Values.Look(ref maxCharge, "maxCharge");
            BackCompatibility.PostExposeData(this);
        }

        public override string ToString() 
        {
            return "CD" + this.CD + "，MaxCD" + this.maxCD +"，MaxCharge" + this.maxCharge + "，Charge" + this.charge;
        }

        public int CD;
        public int maxCD;
        public int charge;
        public int maxCharge = 1;
    }
}
