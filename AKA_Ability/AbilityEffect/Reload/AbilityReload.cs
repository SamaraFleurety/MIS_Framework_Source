namespace AKA_Ability
{
    //填个AKAbilityDef获取充能状态
    /*public class AbilityReloadProperty
    {
        public AKAbilityDef target;//技能多了有可能做成下标

        public ThingDef ammoDef; //弹药def

        public int ammoCountToRefill = 0; //填了这个就是只接受一次性暴力装填,弹药数量满足就全部消耗,不论剩余

        public int ammoCountPerCharge; //装填一次充能需要的弹药数量

        public int baseReloadTicks = 60;

        public SoundDef soundReload;

        [MustTranslate]
        public string cooldownGerund = "on cooldown";
        [MustTranslate]
        public string chargeNoun = "charge";
        public NamedArgument CooldownVerbArgument => cooldownGerund.CapitalizeFirst().Named("COOLDOWNGERUND");
        public NamedArgument ChargeNounArgument => chargeNoun.Named("CHARGENOUN");
    }

    //用来获取技能充能状态的组件
    public class AbilityReload : ICompWithCharges, IReloadableComp
    {
        public AbilityTracker tracker;
        public AbilityReloadProperty Props;

        public AbilityReload(AbilityReloadProperty property, AbilityTracker abilityTracker)
        {
            tracker = abilityTracker;
            Props = property;
        }

        AKAbility_Base cachedTarget = null;
        AKAbility_Base TargetToReload
        {
            get 
            {
                cachedTarget ??= tracker.TryGetAbility(Props.target);
                if (cachedTarget is null)
                {
                    Log.Error("cachedTarget不存在");
                }
                return cachedTarget;
            }
        }

        public int RemainingCharges 
        {
            get => TargetToReload.cooldown.Charge; 
            set => TargetToReload.cooldown.Charge = value; 
        }

        public int MaxCharges => TargetToReload.cooldown.MaxCharge;

        public Thing ReloadableThing => tracker.holder;

        public ThingDef AmmoDef => Props.ammoDef;

        public int BaseReloadTicks => Props.baseReloadTicks;

        public string LabelRemaining => $"{RemainingCharges} / {MaxCharges}";

        public bool NeedsReload(bool allowForceReload)
        {
            if (AmmoDef == null) return false;
            if (Props.ammoCountToRefill != 0)
            {
                if (!allowForceReload) return RemainingCharges == 0;
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
            if (!NeedsReload(allowForcedReload)) return 0;
            if (Props.ammoCountToRefill != 0) return Props.ammoCountToRefill;
            return Props.ammoCountPerCharge;
        }

        public int MaxAmmoNeeded(bool allowForcedReload)
        {
            if (!NeedsReload(allowForcedReload)) return 0;
            if (Props.ammoCountToRefill != 0) return Props.ammoCountToRefill;
            return Props.ammoCountPerCharge * (MaxCharges - RemainingCharges);
        }

        public int MaxAmmoAmount()
        {
            if (AmmoDef == null) return 0;
            if (Props.ammoCountToRefill == 0) return Props.ammoCountPerCharge * MaxCharges;
            return Props.ammoCountToRefill;
        }

        public string DisabledReason(int minNeeded, int maxNeeded)
        {
            return TranslatorFormattedStringExtensions.Translate(arg3: ((Props.ammoCountToRefill == 0) ? ((minNeeded == maxNeeded) ? minNeeded.ToString() : $"{minNeeded}-{maxNeeded}") : Props.ammoCountToRefill.ToString()).Named("COUNT"), key: "CommandReload_NoAmmo", arg1: Props.ChargeNounArgument, arg2: NamedArgumentUtility.Named(AmmoDef, "AMMO"));
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
    }*/
}
