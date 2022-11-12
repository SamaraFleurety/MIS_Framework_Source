using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HCP_DpsEnhance_Ranged : HediffCompProperties
    {
        public RangedStat statOffset;
        public int duration = 1;

        public HCP_DpsEnhance_Ranged()
        {
            this.statOffset = new RangedStat();
            this.compClass = typeof(HC_DpsEnhance_Ranged);
        }
    }
}
