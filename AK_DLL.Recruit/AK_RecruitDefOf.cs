using RimWorld;
using Verse;

namespace AK_DLL.Recruit
{
    [DefOf]
    public static class AK_RecruitDefOf
    {
        public static JobDef AK_Job_UseRecruitConsole;
        public static JobDef AK_Job_OperatorChangeFashion;

        public static UIPrefabDef AK_Prefab_yccMainMenu;
        public static UIPrefabDef AK_Prefab_yccOpList;
        public static UIPrefabDef AK_Prefab_OpDetail;

        static AK_RecruitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AK_RecruitDefOf));
        }
    }
}
