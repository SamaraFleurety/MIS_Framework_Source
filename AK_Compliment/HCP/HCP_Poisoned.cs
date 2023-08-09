using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_Compliment
{
    public class HCP_Poisoned : HediffCompProperties
    {
        public HCP_Poisoned()
        {
            this.compClass = typeof(HC_Poisoned);
        }
    }

    public class HC_Poisoned : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            
        }
    }

}
