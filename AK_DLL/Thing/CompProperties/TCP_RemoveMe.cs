using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class TCP_RemoveMe : CompProperties
    {
        public TCP_RemoveMe()
        {
            this.compClass = typeof(TC_RemoveMe);
        }
    }
}
