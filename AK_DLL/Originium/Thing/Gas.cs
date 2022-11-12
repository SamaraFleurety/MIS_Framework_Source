using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class Gas : RimWorld.Gas
	{
		public GasDef Def
		{
			get
			{
				return this.def as GasDef;
			}
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (this.Def.interval > 0)
			{
				this.interval = this.Def.interval;
			}
			this.tickCounter = this.interval;
		}

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
				this.tickCounter = this.interval;
			}
		}

        public override void TickLong()
        {
			this.Tick();
        }

        public override void TickRare()
        {
			this.Tick();
        }
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
					this.AddHediffToPawn(pawn, this.Def.addHediff, this.Def.hediffAddChance, this.Def.hediffSeverity);
				}
			}
		}

		public void AddHediffToPawn(Pawn pawn, HediffDef hediffToAdd, float chanceToAddHediff, float severityToAdd)
		{
			if (!this.PawnCanBeAffected(pawn))
			{
				return;
			}
			if (!Rand.Chance(chanceToAddHediff) || severityToAdd <= 0f)
			{
				return;
			}
			float statValue = 0;
			if (this.Def.resistedBy != null) statValue = StatExtension.GetStatValue(pawn, this.Def.resistedBy, true, -1);
			Hediff hediff = HediffMaker.MakeHediff(hediffToAdd, pawn, null);
			hediff.Severity = severityToAdd * (1 - statValue);
			
			if (pawn.health.hediffSet.HasHediff(hediffToAdd, false))
			{
				pawn.health.hediffSet.GetFirstHediffOfDef(hediffToAdd, false).Severity += hediff.Severity;
				return;
			}
			pawn.health.AddHediff(hediff, null, null, null);
		}

		private bool PawnCanBeAffected(Pawn pawn)
		{
			return (!this.Def.ignoreAnimals || !pawn.RaceProps.Animal) && (!this.Def.ignoreInsectFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Insectoid) && (!this.Def.ignoreMechanoidFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Mechanoid) && (!this.Def.ignoreNormalFlesh || pawn.RaceProps.FleshType != FleshTypeDefOf.Normal);
		}

		private int tickCounter = 30;

		private int interval = 30;
	}
}
