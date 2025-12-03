using AK_DLL.Apparels;
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
            #region 大破逻辑 哪天找个时间删了
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
            #endregion
            if (__instance is Apparel_Shipgirl aps)
            {
                __result = aps.WornGraphicPathOverride;
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
