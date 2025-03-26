using Verse;

namespace AKA_Ability
{
    public class HCP_BloodLoss : HediffCompProperties
    {
        public int interval = 60;
        public float lossAmount = 0.05f;
        public int duration = 10;
        public int delay = 0;
        public HCP_BloodLoss()
        {
            this.compClass = typeof(HC_BloodLoss);
        }
    }
}
