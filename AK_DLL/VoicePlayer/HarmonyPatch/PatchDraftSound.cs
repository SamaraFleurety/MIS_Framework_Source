using HarmonyLib;
using RimWorld;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Pawn_DraftController), "Drafted", MethodType.Setter)]
    public class PatchDraftSound
    {
        [HarmonyPostfix]
        public static void Postfix(bool value, Pawn_DraftController __instance)
        {
            OperatorDocument doc = __instance.pawn.GetDoc();
            if (doc?.voicePack == null) return;

            if (__instance.Drafted)
                doc.voicePack.draftSounds.RandomElement().PlaySound();
            else
                doc.voicePack.undraftSound.PlaySound();
        }
    }
}
