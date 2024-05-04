using Verse;

namespace AK_DLL
{
    public class TCP_RemoveMe : CompProperties
    {
        public TCP_RemoveMe()
        {
            this.compClass = typeof(TC_RemoveMe);
        }
    }
    public class TC_RemoveMe : ThingComp
    {
        public override void PostPostMake()
        {
            if (AK_ModSettings.debugOverride == false && OperatorDef.currentlyGenerating == false)
            {
                this.parent.Destroy();
            }
        }

        public override void PostExposeData()
        {
        }
    }
}
