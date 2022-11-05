using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class GasEmitterDef : ThingDef
	{
		public int ticksBetweenPulse = 120;

		public float chanceToFillEachCell = 0.8f;

		public float areaFillRadius = 3.5f;

		public ThingDef gasType;

		public int countOfThingsToSpawnPerCell = 1;

		public float fuelCostPerSpawn;

		public bool addToExsistingStacks;
	}
}
