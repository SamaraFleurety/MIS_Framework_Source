﻿using System;
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
            OperatorDocument doc = __instance.pawn.GetDoc();
            if (doc != null)
            {
                if (__instance.Drafted)
                    doc.voicePack.draftSounds.RandomElement().PlaySound();
                else
                    doc.voicePack.undraftSound.PlaySound();
            }
        }
    }
}
