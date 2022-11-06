using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class TCP_GasEmitter : CompProperties
	{
		public int ticksBetweenPulse = 120;

		public float areaFillRadius = 3.5f;

		public ThingDef gasType;

		public int countOfThingsToSpawnPerCell = 1;

		public float fuelCostPerSpawn;

		public bool addToExsistingStacks;

        public TCP_GasEmitter()
        {
			this.compClass = typeof(TC_GasEmitter);
        }
    }
}
