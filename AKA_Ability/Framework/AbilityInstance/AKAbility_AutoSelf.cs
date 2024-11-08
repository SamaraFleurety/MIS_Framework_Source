using Verse;

namespace AKA_Ability
{
    public class AKAbility_AutoSelf : AKAbility_Auto
    {
        public AKAbility_AutoSelf(AbilityTracker tracker) : base(tracker)
        {
        }

        public AKAbility_AutoSelf(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        protected override LocalTargetInfo Target => CasterPawn;
    }
}
