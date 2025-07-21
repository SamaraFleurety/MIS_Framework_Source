using Verse;

namespace AKA_Ability.TickCondition
{
    //缩写是TiC
    public class TickCondion_Base : IExposable
    {
        public AbilityTracker tracker;

        public TickCondion_Base(AbilityTracker tracker)
        {
            this.tracker = tracker;
        }
        public virtual void ExposeData()
        {
            return;
        }

        public virtual bool TickableNow()
        {
            return true;
        }
    }
}
