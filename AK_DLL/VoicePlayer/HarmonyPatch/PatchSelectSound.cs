using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Selector), "Select")]
    public class PatchSelectSound
    {
        [HarmonyPostfix]
        public static void postfix(Object obj)
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
