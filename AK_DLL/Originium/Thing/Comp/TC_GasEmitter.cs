using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class TC_GasEmitter : ThingComp
	{
		private TCP_GasEmitter ExactProps
		{
			get
			{
				return this.props as TCP_GasEmitter;
			}
		}

		public override void CompTick()
		{
			Log.Error("源石尘组件不应该使用normal tickertype");
		}

        public override void CompTickRare()
		{
			base.CompTickRare();
			this.updateRegionIntervel--;
			if (this.updateRegionIntervel <= 0)
            {
				this.ScanArea(base.parent.Position, base.parent.Map, this.ExactProps.areaFillRadius);
			}
			this.DoSpawnCycle(this.affectedCells);
		}
        public override void CompTickLong()
		{
			Log.Error("源石尘组件不应该使用long tickertype");
        }
        /*private void PulseThisTick()
		{
			this.updateRegionIntervel = this.ExactProps.ticksBetweenPulse;
			if (this.updateArea)
			{
				this.ScanArea(base.parent.Position, base.parent.Map, this.ExactProps.areaFillRadius);
			}
			this.updateArea = !this.updateArea;
			this.DoSpawnCycle(this.cellsInArea);
		}*/

		private void DoSpawnCycle(List<IntVec3> affectedCells)
		{
			if (affectedCells.Count < 1 || this.ExactProps.gasType == null)
			{
				return;
			}
			for (int i = 0; i < affectedCells.Count; i++)
			{
				Thing thing = ThingMaker.MakeThing(ThingDefOf.Mote_Stun, null);
				IntVec3 intVec = affectedCells[i];
				if (!this.DoesCellContain(intVec, base.parent.Map, this.ExactProps.gasType))
				{
					thing = ThingMaker.MakeThing(this.ExactProps.gasType, null);
					thing.stackCount = this.ExactProps.countOfThingsToSpawnPerCell;
					if (!thing.Spawned)
					{
						GenSpawn.Spawn(thing, intVec, base.parent.Map, 0);
					}
				}
			}
		}

		private bool DoesCellContain(IntVec3 cell, Map map, ThingDef thingToLookFor)
		{
			return GridsUtility.GetFirstThing(cell, map, thingToLookFor) != null;
		}

		private void ScanArea(IntVec3 posistion, Map map, float distance)
		{
			this.affectedCells.Clear();
			if (!GenGrid.InBounds(posistion, map))
			{
				return;
			}
			IntVec3[] nearLoc = new IntVec3[] {posistion, new IntVec3(posistion.x + 1, posistion.y, posistion.z), new IntVec3(posistion.x - 1, posistion.y, posistion.z), new IntVec3(posistion.x, posistion.y, posistion.z + 1), new IntVec3(posistion.x, posistion.y, posistion.z - 1) };
			Region region = null;
			for (int i = 0; i < 5; ++i)
            {
				if (nearLoc[i].InBounds(base.parent.Map))
                {
					region = GridsUtility.GetRegion(nearLoc[i], map, RegionType.Set_Passable);
					if (region != null) goto Find_Region;
				}
            }
			if (region == null)
			{
				return;
			}

			Find_Region:
			RegionTraverser.BreadthFirstTraverse(region, (Region from, Region traverseRegion) => traverseRegion.door == null, delegate 
				(Region targetRegion)
			{
				foreach (IntVec3 item in targetRegion.Cells)
				{
					if (item.InHorDistOf(posistion, this.ExactProps.areaFillRadius))
					{
						this.affectedCells.Add(item);
					}
				}
				return false;
			}, 16, RegionType.Set_Passable);
		}

		private List<IntVec3> affectedCells = new List<IntVec3>();

		private int updateRegionIntervel = 0;

	}
}
