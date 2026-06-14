using HarmonyLib;
using RimWorld;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Selector), nameof(Selector.Select))]
    public class Patch_SelectSound
    {
        [HarmonyPostfix]
        public static void Postfix(object obj)
        {
            if (obj is Pawn pawn)
            {
                OperatorDocument doc = pawn.GetDoc();
                if (doc == null) return;
                if (doc.voicePack == null) return;
                doc.voicePack.selectSounds.RandomElement().PlaySound();
            }
        }
    }
}
