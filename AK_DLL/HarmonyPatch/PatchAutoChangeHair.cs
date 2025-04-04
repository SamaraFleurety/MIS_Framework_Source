﻿using HarmonyLib;
using Verse;
using Verse.AI;

namespace AK_DLL
{
    //禁止随机变化发型
    [HarmonyPatch(typeof(JobGiver_UseStylingStationAutomatic), "TryGiveJob")]
    public static class PatchAutoChangeHair
    {
        [HarmonyPrefix]
        public static bool prefix(Pawn pawn, ref Job __result)
        {
            if (pawn.GetDoc() != null)
            {
                __result = null;
            }
            return false;
        }
    }
}
