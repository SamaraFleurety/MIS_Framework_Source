using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class GasEmitter : Building
	{// Token: 0x17000003 RID: 3
	 // (get) Token: 0x06000018 RID: 24 RVA: 0x000026DE File Offset: 0x000008DE
		private GasEmitterDef ExactProps
		{
			get
			{
				return this.def as GasEmitterDef;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000026EB File Offset: 0x000008EB
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000275A File Offset: 0x0000095A
		public override void Tick()
		{
			base.Tick();
			if (true)
			{
				this.ticksUntilPulse--;
				if (this.ticksUntilPulse <= 0)
				{
					this.PulseThisTick();
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000027E4 File Offset: 0x000009E4
		private void PulseThisTick()
		{
			this.ticksUntilPulse = this.ExactProps.ticksBetweenPulse;
			if (this.updateArea)
			{
				this.ScanArea(base.Position, base.Map, this.ExactProps.areaFillRadius);
			}
			this.updateArea = !this.updateArea;
			this.DoSpawnCycle(this.cellsInArea);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002844 File Offset: 0x00000A44
		private void DoSpawnCycle(List<IntVec3> eligibleCells)
		{
			if (eligibleCells.Count < 1)
			{
				return;
			}
			if (this.ExactProps.gasType == null)
			{
				Log.Warning("Something is wrong in a AreaThingSpawner: ");
				Log.Warning(this.ExactProps.defName + " in " + this.ExactProps.fileName + " thingToSpawnInArea is null");
				return;
			}
			for (int i = 0; i < eligibleCells.Count; i++)
			{
				Thing thing = ThingMaker.MakeThing(ThingDefOf.Mote_Stun, null);
				IntVec3 intVec = eligibleCells[i];
				if (!this.DoesCellContain(intVec, base.Map, this.ExactProps.gasType) && UnityEngine.Random.Range(0f, 1f) < this.ExactProps.chanceToFillEachCell)
				{
					thing = ThingMaker.MakeThing(this.ExactProps.gasType, null);
					thing.stackCount = this.ExactProps.countOfThingsToSpawnPerCell;
					if (!thing.Spawned)
					{
						GenSpawn.Spawn(thing, intVec, base.Map, 0);
					}
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002956 File Offset: 0x00000B56
		private bool DoesCellContain(IntVec3 cell, Map map, ThingDef thingToLookFor)
		{
			return GridsUtility.GetFirstThing(cell, map, thingToLookFor) != null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002964 File Offset: 0x00000B64
		private void ScanArea(IntVec3 posistion, Map map, float distance)
		{
			this.cellsInArea.Clear();
			if (!GenGrid.InBounds(posistion, map))
			{
				return;
			}
			Region region = GridsUtility.GetRegion(posistion, map, RegionType.Set_Passable);
			if (region == null)
			{
				return;
			}
			RegionTraverser.BreadthFirstTraverse(region, (Region from, Region traverseRegion) => traverseRegion.door == null, delegate 
				(Region targetRegion)
			{
				foreach (IntVec3 item in targetRegion.Cells)
				{
					if (item.InHorDistOf(posistion, this.ExactProps.areaFillRadius))
					{
						this.cellsInArea.Add(item);
					}
				}
				return false;
			}, 16, RegionType.Set_Passable);
		}

		// Token: 0x04000012 RID: 18
		private List<IntVec3> cellsInArea = new List<IntVec3>();

		// Token: 0x04000013 RID: 19
		private bool updateArea = true;

		// Token: 0x04000014 RID: 20
		private int ticksUntilPulse = 300;

	}
}
