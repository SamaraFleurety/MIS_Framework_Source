using HarmonyLib;
using RimWorld;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Apparel), "WornGraphicPath", MethodType.Getter)]
    public class Patch_WornGraphic
    {
        private static bool forbidRecursion;

        [HarmonyPrefix]
        public static bool Prefix(ref string __result, Apparel __instance)
        {
            if (forbidRecursion)
            {
                forbidRecursion = false;
                return HarmonyPrefixRet.keepOriginal;
            }
            if (__instance is Apparel_Operator apparelOperator)
            {
                forbidRecursion = true;
                __result = apparelOperator.WornGraphicPath_MultiFoam;
                forbidRecursion = false;
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
