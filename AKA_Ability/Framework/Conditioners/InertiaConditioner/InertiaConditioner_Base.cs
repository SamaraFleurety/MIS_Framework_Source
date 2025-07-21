using Verse;

namespace AKA_Ability.InertiaConditioner
{
    //惰性判断器。惰性的技能不会tick，不会显示gizmo
    //用这个词是因为active之类的别的常用词容易产生歧义
    public abstract class InertiaConditioner_Base : IExposable
    {
        AbilityTracker tracker;
        public InertiaConditioner_Base(AbilityTracker tracker)
        {
            this.tracker = tracker;
        }

        public virtual void ExposeData()
        {
        }

        public abstract bool InertiaNow();
    }
}
