using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AK_DLL
{
	// Token: 0x02000015 RID: 21
	public class Gas : RimWorld.Gas
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000047AD File Offset: 0x000029AD
		public GasDef Def
		{
			get
			{
				return this.def as GasDef;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000047BA File Offset: 0x000029BA
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (this.Def.ticksPerApplication > 0)
			{
				this.ticksPerApplication = this.Def.ticksPerApplication;
			}
			this.tickCounter = this.ticksPerApplication;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000047EF File Offset: 0x000029EF
		public override void Tick()
		{
			if (base.Destroyed)
			{
				return;
			}
			base.Tick();
			this.tickCounter--;
			if (this.tickCounter <= 0)
			{
				this.ApplyHediff();
				this.tickCounter = this.ticksPerApplication;
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000482C File Offset: 0x00002A2C
		public void ApplyHediff()
		{
			if (this.Def.addHediff == null)
			{
				return;
			}
			if (base.Destroyed)
			{
				return;
			}
			List<Thing> thingList = GridsUtility.GetThingList(base.Position, base.Map);
			if (thingList.Count == 0 || thingList == null)
			{
				return;
			}
			for (int i = 0; i < thingList.Count; i++)
			{
				Pawn pawn = thingList[i] as Pawn;
				if (pawn != null && pawn.Spawned && !pawn.health.Dead)
				{
					this.AddHediffToPawn(pawn, this.Def.addHediff, this.Def.hediffAddChance, this.Def.hediffSeverity, this.Def.onlyAffectLungs);
				}
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000048D8 File Offset: 0x00002AD8
		public void AddHediffToPawn(Pawn pawn, HediffDef hediffToAdd, float chanceToAddHediff, float severityToAdd, bool onlylungs)
		{
			if (!this.PawnCanBeAffected(pawn))
			{
				return;
			}
			if (!Rand.Chance(chanceToAddHediff) || severityToAdd <= 0f)
			{
				return;
			}
			float statValue = StatExtension.GetStatValue(pawn, StatDefOf.ToxicResistance, true, -1);
			Hediff hediff = HediffMaker.MakeHediff(hediffToAdd, pawn, null);
			if (!this.Def.ignoreToxicSensitivity)
			{
				hediff.Severity = severityToAdd * (1f - statValue);
			}
			else
			{
				hediff.Severity = severityToAdd;
			}

			if (pawn.health.hediffSet.HasHediff(hediffToAdd, false))
			{
				pawn.health.hediffSet.GetFirstHediffOfDef(hediffToAdd, false).Severity += hediff.Severity;
				return;
			}
			pawn.health.AddHediff(hediff, null, null, null);
			
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004B0C File Offset: 0x00002D0C
		private bool PawnCanBeAffected(Pawn pawn)
		{
			return (!this.Def.ignoreAnimals || !pawn.RaceProps.Animal) && (!this.Def.ignoreInsectFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Insectoid) && (!this.Def.ignoreMechanoidFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Mechanoid) && (!this.Def.ignoreNormalFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Normal);
		}

		// Token: 0x0400006A RID: 106
		private int tickCounter = 30;

		// Token: 0x0400006B RID: 107
		private int ticksPerApplication = 30;
	}
}
