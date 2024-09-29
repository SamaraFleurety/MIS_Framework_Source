using Verse;

namespace AKS_Shield.Extension
{
    public class TCP_ExtraCharges : CompProperties
    {
        public int maxExtraCharges = 5;
        public int chargeInterval = 600;
        public TCP_ExtraCharges()
        {
            compClass = typeof(TC_ExtraCharges);
        }
    }

    //让护盾有额外充能。在护盾破碎时消耗充能然后马上把能量回复满。
    public class TC_ExtraCharges : TC_ShieldExtension_PostEffects_Base
    {
        int tick = 0;

        public int charges = 0;
        TCP_ExtraCharges Prop => props as TCP_ExtraCharges;
        int ChargeInterval => Prop.chargeInterval;
        int MaxCharge => Prop.maxExtraCharges;

        public override void CompTick()
        {
            Tick(1);
        }

        public override void Tick(int amt)
        {
            if (charges >= MaxCharge || CompShield.energy == 0) return;
            tick += amt;

            while (tick >= ChargeInterval && charges < MaxCharge)
            {
                tick -= ChargeInterval;
                ++charges;
            }
        }

        public override void Notify_Break(DamageInfo dinfo)
        {
            if (charges > 0)
            {
                --charges;
                CompShield.energy = CompShield.Props.energyMax;
            }
        }

        //重置后 没有额外充能
        public override void Notify_Reset()
        {
            charges = 0;
            tick = 0;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref tick, "t", 0);
            Scribe_Values.Look(ref charges, "charge");
        }
    }
}
