using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using Verse.Sound;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Pawn_DraftController), "Drafted", MethodType.Setter)]
    public class PatchDraftSound
    {
        [HarmonyPostfix]
        public static void postfix(bool value, Pawn_DraftController __instance) 
        {
            if (__instance.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) is Hediff_Operator hediff)
            {
                if (__instance.Drafted)
                    hediff.document.voicePack.draftSounds.RandomElement().PlaySound();
                else
                    hediff.document.voicePack.undraftSound.PlaySound();
            }
        }
    }
}
