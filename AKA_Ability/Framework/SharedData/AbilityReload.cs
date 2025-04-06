using RimWorld.Utility;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKA_Ability.SharedData
{
    public class AbilityReloadProperty
    {
        public Type reloadClass;

        public int maxCharges;

        public ThingDef ammoDef;

        public int ammoCountToRefill;

        public int ammoCountPerCharge;

        public int baseReloadTicks = 60;

        public SoundDef soundReload;

        public AbilityReloadProperty()
        {
            this.reloadClass = typeof(AbilityReload);
        }

        [MustTranslate]
        public string cooldownGerund = "on cooldown";
        [MustTranslate]
        public string chargeNoun = "charge";
        public NamedArgument CooldownVerbArgument => cooldownGerund.CapitalizeFirst().Named("COOLDOWNGERUND");
        public NamedArgument ChargeNounArgument => chargeNoun.Named("CHARGENOUN");
    }

    //以后可能会有多个技能同时要装填？先做共享tracker的再说
    public class AbilityReload : IExposable, ICompWithCharges, IReloadableComp
    {
        public AbilityTracker tracker;
        public AbilityReloadProperty Props;

        public AbilityReload(AbilityReloadProperty property, AbilityTracker abilityTracker)
        {
            tracker = abilityTracker;
            Props = property;
        }
        SD_SharedCharge SharedChargeData => tracker.TryGetSharedData<SD_SharedCharge>();

        public int RemainingCharges 
        {
            get => SharedChargeData.cooldown.Charge; 
            set => SharedChargeData.cooldown.Charge = value; 
        }

        public int MaxCharges => SharedChargeData.cooldown.MaxCharge;

        public Thing ReloadableThing => tracker.holder;

        public ThingDef AmmoDef => Props.ammoDef;

        public int BaseReloadTicks => Props.baseReloadTicks;

        public string LabelRemaining => throw new NotImplementedException();

        public virtual void ExposeData()
        {
            BackCompatibility.PostExposeData(this);
        }

        public string DisabledReason(int minNeeded, int maxNeeded)
        {
            return TranslatorFormattedStringExtensions.Translate(arg3: ((Props.ammoCountToRefill == 0) ? ((minNeeded == maxNeeded) ? minNeeded.ToString() : $"{minNeeded}-{maxNeeded}") : Props.ammoCountToRefill.ToString()).Named("COUNT"), key: "CommandReload_NoAmmo", arg1: Props.ChargeNounArgument, arg2: NamedArgumentUtility.Named(AmmoDef, "AMMO"));
        }

        public bool NeedsReload(bool allowForceReload)
        {
            if (AmmoDef == null) return false;
            if (Props.ammoCountToRefill != 0)
            {
                if (!allowForceReload)
                {
                    return RemainingCharges == 0;
                }
                return RemainingCharges != MaxCharges;
            }
            return RemainingCharges != MaxCharges;
        }

        public void ReloadFrom(Thing ammo)
        {
            if (!NeedsReload(allowForceReload: true)) return;
            if (Props.ammoCountToRefill != 0)
            {
                if (ammo.stackCount < Props.ammoCountToRefill) return;
                ammo.SplitOff(Props.ammoCountToRefill).Destroy();
                RemainingCharges = MaxCharges;
            }
            else
            {
                if (ammo.stackCount < Props.ammoCountPerCharge) return;
                int num = Mathf.Clamp(ammo.stackCount / Props.ammoCountPerCharge, 0, MaxCharges - RemainingCharges);
                ammo.SplitOff(num * Props.ammoCountPerCharge).Destroy();
                RemainingCharges += num;
            }
            Props.soundReload?.PlayOneShot(new TargetInfo(tracker.owner.Position, tracker.owner.Map));
        }
        public int MinAmmoNeeded(bool allowForcedReload)
        {
            if (!NeedsReload(allowForcedReload))
            {
                return 0;
            }
            if (Props.ammoCountToRefill != 0)
            {
                return Props.ammoCountToRefill;
            }
            return Props.ammoCountPerCharge;
        }

        public int MaxAmmoNeeded(bool allowForcedReload)
        {
            if (!NeedsReload(allowForcedReload))
            {
                return 0;
            }
            if (Props.ammoCountToRefill != 0)
            {
                return Props.ammoCountToRefill;
            }
            return Props.ammoCountPerCharge * (MaxCharges - RemainingCharges);
        }

        public int MaxAmmoAmount()
        {
            if (AmmoDef == null)
            {
                return 0;
            }
            if (Props.ammoCountToRefill == 0)
            {
                return Props.ammoCountPerCharge * MaxCharges;
            }
            return Props.ammoCountToRefill;
        }

        public bool CanBeUsed(out string reason)
        {
            reason = "";
            if (RemainingCharges <= 0)
            {
                reason = DisabledReason(MinAmmoNeeded(allowForcedReload: false), MaxAmmoNeeded(allowForcedReload: false));
                return false;
            }
            return true;
        }
    }
}
