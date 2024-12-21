using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AKS_Shield.Extension
{
    public class TCP_ExtraCharges : CompProperties
    {
        public int maxExtraCharges = 5;
        public int chargeInterval = 600;
        public int chargesOnReset = 0;   //护盾重置时回复的充能量

        public bool displayInfoGizmo = false;
        public string infoGizmoLabel = "AKS_ExtraChargeInfoGizmoLabel";
        public string infoGizmoDesc = "AKS_ExtraChargeInfoGizmoDesc";
        public string infoGizmoIcon;

        private Texture2D cachedGizmoIcon = null;
        public Texture2D InfoGizmoIcon
        {
            get
            {
                cachedGizmoIcon ??= ContentFinder<Texture2D>.Get(infoGizmoIcon, reportFailure: false);
                return cachedGizmoIcon;
            }
        }
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

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            if (!Prop.displayInfoGizmo || !CompShield.ShouldDisplay) yield break;
            yield return new Command_Toggle
            {
                defaultLabel = $"{Prop.infoGizmoLabel.Translate()} : {charges}",
                defaultDesc = Prop.infoGizmoDesc.Translate(),
                icon = Prop.InfoGizmoIcon,
                toggleAction = delegate ()
                {

                },
                isActive = () => charges > 0,
            };
        }

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
            Log.Message($"notify extra charge break");
            if (charges > 0)
            {
                Log.Message("reset");
                --charges;
                CompShield.energy = CompShield.EnergyMax;
            }
            else
            {
                tick = 0;
            }
        }

        //重置后
        public override void Notify_Reset()
        {
            charges = Prop.chargesOnReset;
            tick = 0;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref tick, "t", 0);
            Scribe_Values.Look(ref charges, "charge");
        }
    }
}
