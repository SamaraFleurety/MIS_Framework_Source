using RimWorld;
using Verse;


namespace AK_DLL
{
	[DefOf]
	internal class JobDefOf
	{
		static JobDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
		}
		public static JobDef AK_UseRecruitConsole;
	}
}
