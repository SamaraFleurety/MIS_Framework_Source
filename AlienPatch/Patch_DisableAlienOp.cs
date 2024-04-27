using HarmonyLib;
using AK_DLL;
using AlienRace;

namespace AKC_AlienRace
{
    [HarmonyPatch(typeof(HarmonyPatches), "GeneratePawnPrefix")]
    public class Patch_DisableAlienOp
    {
        [HarmonyPrefix]
        public static bool DisableKinddefAdjust()
        {
            if (OperatorDef.currentlyGenerating)
            {
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
