using Verse;

namespace AKA_Ability.SharedData
{
    public abstract class AbilityTrackerSharedData_Base : IExposable
    {
        public AbilityTracker tracker;

        protected AbilityTrackerSharedData_Base(AbilityTracker tracker)
        {
            this.tracker = tracker;
        }

        public virtual void ExposeData()
        {
        }
    }
}
