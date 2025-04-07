using AKA_Ability.Cooldown;
using System;
using Verse;

namespace AKA_Ability.SharedData
{
    public class SDP_SharedCharge : AbilityTrackerSharedDataProperty
    {
        public CooldownProperty cooldownProperty;
    }
    public class SD_SharedCharge : AbilityTrackerSharedData_Base
    {
        public Cooldown_Regen cooldown;
        public SD_SharedCharge(AbilityTracker tracker) : base(tracker)
        {
        }

        public SD_SharedCharge(AbilityTracker tracker, AbilityTrackerSharedDataProperty props) : base(tracker, props)
        {
            SDP_SharedCharge prop = props as SDP_SharedCharge;
            cooldown = (Cooldown_Regen)Activator.CreateInstance(prop.cooldownProperty.cooldownClass, prop.cooldownProperty, null);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref cooldown, "cd");
        }
    }
}
