using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HCP_RandAddHediff : HediffCompProperties
    {
        public int interval = 1;
        public TimeToTick intervalUnit = TimeToTick.hour;
        public List<HediffStat> hediffStatsSet = new List<HediffStat>();

        public HCP_RandAddHediff ()
        {
            this.compClass = typeof(HC_RandAddHediff);
        }
    }
}
