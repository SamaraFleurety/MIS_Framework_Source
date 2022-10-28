using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HCP_SelfExplosion : HediffCompProperties
    {
        public int afterTicks = 1;
        public float radius = 2.2f;
        public int damage = 10;

        public HCP_SelfExplosion()
        {
            this.compClass = typeof(HCP_SelfExplosion);
        }
    }
}
