using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKE_VanyaShield
{
    //护盾允许有额外充能。如果剩余额外充能，在破碎时会马上reset
    public class TCP_ShieldExtraCharges : CompProperties
    {
        public int maxExtraCharges = 5;
        public int chargeInterval = 600;
        public TCP_ShieldExtraCharges()
        {
            compClass = typeof(TC_ExtraCharges);
        }
    }

    public class TC_ExtraCharges : ThingComp
    {
        int tick = 0;

        public int charges = 0;
        TCP_ShieldExtraCharges Prop => props as TCP_ShieldExtraCharges;
        int ChargeInterval => Prop.chargeInterval;
        int MaxCharge => Prop.maxExtraCharges;

        public override void CompTick()
        {
            Tick(1);
        }

        public virtual void Tick(int amt)
        {
            if (charges >= MaxCharge) return;
            tick += amt;

            while (tick >= ChargeInterval && charges < MaxCharge)
            {
                tick -= ChargeInterval;
                ++charges;
            }
        }
    }
}
