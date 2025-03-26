using AK_DLL;
using HarmonyLib;
using RimWorld;

namespace LMA_Lib.HarmonyPatchs
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelChanged")]
    public class Patch_ApparealWearNotify 
    {
        [HarmonyPostfix]
        public static void fix(Pawn_ApparelTracker __instance)
        {
            if (__instance.pawn.GetDoc() is ShipDocument doc)
            {
                doc.FCS_Dirty = true;
            }
        }
    }
}
