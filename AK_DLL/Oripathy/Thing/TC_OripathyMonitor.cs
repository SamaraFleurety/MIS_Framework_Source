using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class TCP_OripathyMonitor : CompProperties
    {
        public TCP_OripathyMonitor()
        {
            this.compClass = typeof(TC_OripathyMonitor);
        }
    }

    public class TC_OripathyMonitor : ThingComp
    {
        public override void CompTick()
        {
        }
    }
}
