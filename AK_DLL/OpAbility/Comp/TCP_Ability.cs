using Verse;

namespace AK_DLL
{
    //绑定在Thing上的入口
    public class TCP_Ability : CompProperties
    {
        public TCP_Ability()
        {
            this.compClass = typeof(TC_Ability);
        }
    }

    public class TC_Ability : ThingComp, IExposable
    {
        public bool autoCast = false;
        public override void CompTick()
        {
            base.CompTick();
            Log.Message("comp tick");
        }

        public void ExposeData()
        {
            Log.Message("expos");
            Scribe_Values.Look(ref this.autoCast, "auto", true);
        }
    }
}
