using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;
using HarmonyLib;

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
                if (doc != null)
                { doc.voicePack.selectSounds.RandomElement().PlaySound(); }
            }
        }
    }
}
