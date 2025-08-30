using HarmonyLib;
using Verse;
using Verse.Sound;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Pawn)/*, "Kill",new Type[] { typeof(DamageInfo),typeof(Hediff) }*/)]
    public class Patch_KillCorpse
    {
        [HarmonyPatch("Kill")]
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance)
        {
            OperatorDocument doc = __instance.GetDoc();
            if (doc == null) return;
            __instance.Corpse?.Destroy();
            doc.voicePack.diedSound.PlayOneShot(null);
            doc.currentExist = false;
        }

        [HarmonyPatch("Destroy")]
        [HarmonyPostfix]
        public static void Postfix_des(Pawn __instance)
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
