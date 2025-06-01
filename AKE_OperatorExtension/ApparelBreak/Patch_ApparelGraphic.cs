using AKE_OperatorExtension.ApparelBreak;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKE_OperatorExtension.HarmonyPatchs
{
    [HarmonyPatch(typeof(Apparel), "WornGraphicPath", MethodType.Getter)]
    public class Patch_ApparelGraphic
    {
        [HarmonyPrefix]
        public static bool prefix(Apparel __instance, ref string __result)
        {
            if (__instance is not Apparel_Breakdown ap) return HarmonyPrefixRet.keepOriginal;

            __result = ap.WornGraphicPathByHP;

            return HarmonyPrefixRet.skipOriginal;
        }
    }
}
