using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL { 

    public class HCP_DpsEnhance_Melee : HediffCompProperties
    {
        public List<toolEnhance> enhances;

        public int interval = 1; //间隔是1秒，没必要改。
        public int procedureCount = 1;
        public int enhanceEndTime = 2;
        public HCP_DpsEnhance_Melee()
        {
            this.enhances = new List<toolEnhance>();
            this.compClass = typeof(HC_DpsEnhance_Melee);
        }
    }
}