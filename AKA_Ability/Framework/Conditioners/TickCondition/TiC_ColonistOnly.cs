namespace AKA_Ability.TickCondition
{
    public class TiC_ColonistOnly : TickCondion_Base
    {
        public TiC_ColonistOnly(AbilityTracker tracker) : base(tracker)
        {
        }

        public override bool TickableNow()
        {
            return tracker.owner != null && tracker.owner.IsColonist;
        }
    }
}
