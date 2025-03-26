using Verse;

namespace AK_DLL
{
    public class HCP_RemoveMe : HediffCompProperties
    {
        public HCP_RemoveMe()
        {
            compClass = typeof(HC_RemoveMe);
        }
    }

    public class HC_RemoveMe : HediffComp
    {
        int tick = 100;
        public override void CompPostTick(ref float severityAdjustment)
        {
            --tick;
            if (tick <= 0) parent.pawn.health.RemoveHediff(parent);
        }
    }
}
