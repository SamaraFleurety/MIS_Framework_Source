using Verse;

namespace AKA_Ability.InertiaConditioner
{
    //dll里手动控制开关，任何情况下不会自动判断
    public class IC_Manual : InertiaConditioner_Base
    {
        public bool inertia = false;
        public IC_Manual(AbilityTracker tracker) : base(tracker)
        {
        }

        public override bool InertiaNow()
        {
            return inertia;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref inertia, "inertia", false);
        }
    }
}
