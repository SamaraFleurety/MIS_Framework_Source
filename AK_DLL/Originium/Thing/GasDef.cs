using System;
using Verse;

namespace AK_DLL
{
	// Token: 0x02000016 RID: 22
	public class GasDef : ThingDef
	{
		// Token: 0x0400006C RID: 108
		public HediffDef addHediff;

		// Token: 0x0400006D RID: 109
		public float hediffAddChance = 1f;

		// Token: 0x0400006E RID: 110
		public float hediffSeverity = 0.05f;

		// Token: 0x0400006F RID: 111
		public int ticksPerApplication = 150;

		// Token: 0x04000070 RID: 112
		public bool onlyAffectLungs = true;

		// Token: 0x04000071 RID: 113
		public bool isAcid;

		// Token: 0x04000072 RID: 114
		public bool ignoreAnimals;

		// Token: 0x04000073 RID: 115
		public bool ignoreNormalFlesh;

		// Token: 0x04000074 RID: 116
		public bool ignoreInsectFlesh;

		// Token: 0x04000075 RID: 117
		public bool ignoreMechanoidFlesh;

		// Token: 0x04000076 RID: 118
		public bool ignoreToxicSensitivity;
	}
}
