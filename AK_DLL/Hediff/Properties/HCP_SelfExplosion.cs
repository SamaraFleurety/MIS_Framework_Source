using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    class HCP_SelfExplosion : HediffCompProperties
    {
        int afterTicks = 1;
        float raidiuradius = 2.2f;
        float damage = 10f;

        public HCP_SelfExplosion()
        {
            this.compClass = typeof(HCP_SelfExplosion);
        }
    }
}
