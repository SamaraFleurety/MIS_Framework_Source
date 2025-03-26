using Verse;

namespace AKE_OperatorExtension
{
    public class HCP_GrowthSys : HediffCompProperties
    {
        public HCP_GrowthSys()
        {
            this.compClass = typeof(HC_GrowthSys);
        }
    }
    public class HC_GrowthSys : HediffComp
    {
        private int tick = 0;
        public HCP_GrowthSys Props => (HCP_GrowthSys)this.props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            ++tick;
            if (tick == 0)
            {

            }

        }
    }
}