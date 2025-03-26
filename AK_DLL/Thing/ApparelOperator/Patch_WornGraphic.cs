using HarmonyLib;
using RimWorld;

namespace AK_DLL.HarmonyPatchs
{
    [HarmonyPatch(typeof(Apparel), "WornGraphicPath", MethodType.Getter)]
    public class Patch_WornGraphic
    {
        static bool forbidRecursion = false;
        [HarmonyPrefix]
        public static bool prefix(ref string __result, Apparel __instance)
        {
            if (forbidRecursion)
            {
                forbidRecursion = false;
                return HarmonyPrefixRet.keepOriginal;
            }
            if (__instance is Apparel_Operator ap_op)
            {
                forbidRecursion = true;
                __result = ap_op.WornGraphicPath_MultiFoam;
                forbidRecursion = false;
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
