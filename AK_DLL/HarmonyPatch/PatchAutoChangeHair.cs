using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace AK_DLL
{

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
