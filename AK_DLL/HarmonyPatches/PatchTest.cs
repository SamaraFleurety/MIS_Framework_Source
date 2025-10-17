using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace AK_DLL
{
    /*[HarmonyPatch(typeof(Messages), "Message", new Type[] { typeof(Message), typeof(bool) })]
    public class PatchTest
    {
        [HarmonyPrefix]
        public static void prefix(Message msg, bool historical, List<Message> ___liveMessages)
        {
            Log.Message($"a {msg == null}, {msg.text}, {msg.lookTargets == null}");

            Log.Message($"a1");
            if (historical && Find.Archive != null)
            {
                Log.Message($"a2");
                Find.Archive.Add(msg);
            }
            Log.Message($"a3");
            ___liveMessages.Add(msg);
            Log.Message($"a4");
            while (___liveMessages.Count > 12)
            {
                Log.Message($"a5");
                ___liveMessages.RemoveAt(0);
            }
            Log.Message($"a6");
            if (msg.def.sound != null)
            {
                Log.Message($"a7 {msg.def == null}");
                msg.def.sound.PlayOneShotOnCamera();
            }
            Log.Message($"a8");

        }
    }*/

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
