using System;
using Verse;
using RimWorld;

namespace AK_DLL
{
	public class GasDef : ThingDef
	{
		public HediffDef addHediff;

		public float hediffAddChance = 1f;

		public float hediffSeverity = 0.05f;

		public int ticksPerApplication = 150;

		public bool onlyAffectLungs = true;

		public bool isAcid;

		public bool ignoreAnimals;

		public bool ignoreNormalFlesh;

		public bool ignoreInsectFlesh;

		public bool ignoreMechanoidFlesh;

		public bool ignoreToxicSensitivity;

		public StatDef resistedBy = StatDefOf.ToxicResistance;
	}
}
