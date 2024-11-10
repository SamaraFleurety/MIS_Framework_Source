using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static Verse.DamageWorker;

namespace AK_DLL
{
    /*[HarmonyPatch(typeof(Pawn), "Drafted", MethodType.Getter)]
    public class PatchTest
    {
        [HarmonyPostfix]
        public static void prefix(Pawn __instance, bool __result)
        {
            Log.Message($"checking drafted {__instance.Name}, {__result}");
        }
    }

    /*[HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public class PatchTest2
    {
        [HarmonyPrefix]
        public static void prefix(Pawn __instance)
        {
            Log.Message($"show draft gizmo out for {__instance.Name} : {__instance.IsColonistPlayerControlled} - {__instance.IsColonyMech} - {__instance.IsColonyMutantPlayerControlled} and drafter{__instance.drafter != null}");
            Log.Message($"params: iscolonist{__instance.IsColonist} - mental{__instance.MentalStateDef == null}");
            Log.Message($"parms2.1 {__instance.Faction != null}");
            Log.Message($"params2.2 {__instance.Faction.IsPlayer}");
            Log.Message($"params2.31 - {__instance.RaceProps != null}");
            Log.Message($"params2.3 - {__instance.RaceProps.Humanlike}");
            Log.Message($"params2.4 - {!__instance.IsSlave} - {!__instance.IsMutant}");
        }
    }*/
}
