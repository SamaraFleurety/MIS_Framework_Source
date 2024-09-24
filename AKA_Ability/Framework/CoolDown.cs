using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Verse;

namespace AKA_Ability
{
    //此CD逻辑统一为：初始CD = CDPerCharge, 每tick - 1，到0时增加1Charge
    public class CoolDown : IExposable
    {
        public CoolDown()
        {
        }

        public CoolDown(int maxCharge, int CDPerCharge)
        {
            this.CDCurrent = 0;
            this.maxCharge = maxCharge;
            this.charge = maxCharge;
            this.CDPerCharge = CDPerCharge;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref CDCurrent, "CD");
            Scribe_Values.Look(ref CDPerCharge, "maxCD");
            Scribe_Values.Look(ref charge, "charge");
            Scribe_Values.Look(ref maxCharge, "maxCharge");
            BackCompatibility.PostExposeData(this);
        }

        public void Tick(int amt)
        {
            if (charge >= maxCharge) return;
            CDCurrent -= amt;
            if (CDCurrent <= 0)
            {
                charge += 1;
                if (charge < maxCharge) CDCurrent = CDPerCharge;
            }
        }

        public virtual void CostCharge(int cost)
        {
            if (cost == 0) return;
            if (charge == maxCharge) CDCurrent = CDPerCharge;
            charge -= cost;
            charge = Math.Max(charge, 0);
        }

        public override string ToString() 
        {
            return "CD" + this.CDCurrent + "，MaxCD" + this.CDPerCharge +"，MaxCharge" + this.maxCharge + "，Charge" + this.charge;
        }

        public int CDCurrent;
        public int CDPerCharge;
        public int charge;
        public int maxCharge = 1;
    }
}
