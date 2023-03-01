using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
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
