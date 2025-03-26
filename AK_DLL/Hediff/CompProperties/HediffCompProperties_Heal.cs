using Verse;

namespace AK_DLL
{
    public class HediffCompProperties_Heal : HediffCompProperties
    {
        public HediffCompProperties_Heal() 
        {
            this.compClass = typeof(HediffComp_Heal);
        }

        public int healOncePer;
        public int healPoint;
        public int amountOfHealOnce;
    }
}
