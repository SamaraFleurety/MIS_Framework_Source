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
            if (obj is Pawn pawn && pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) is Hediff_Operator hediff) 
            {
                hediff.voicePackDef.selectSounds.RandomElement().PlaySound();
            }
        }
    }
}
