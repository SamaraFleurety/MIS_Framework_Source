using System;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;
using Verse.Sound;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Pawn)/*, "Kill",new Type[] { typeof(DamageInfo),typeof(Hediff) }*/)]
    public class Patch_KillCorpse
    {
        [HarmonyPatch("Kill")]
        [HarmonyPostfix]
        public static void postfix(Pawn __instance) 
        {
            OperatorDocument doc = __instance.GetDoc();
            if (doc == null) return;
            __instance.Corpse?.Destroy();
            doc.voicePack.diedSound.PlayOneShot(null);
            doc.currentExist = false;
        }

        [HarmonyPatch("Destroy")]
        [HarmonyPostfix]
        public static void postfix_des(Pawn __instance)
        {
            if (__instance == null) return;
            OperatorDocument doc = __instance.GetDoc();
            if (doc == null) return;

            doc.currentExist = false;
            //if (__instance.Corpse == null) return;
            __instance.Corpse?.Destroy();
            doc.voicePack.diedSound.PlayOneShot(null);
        }
    }
}
