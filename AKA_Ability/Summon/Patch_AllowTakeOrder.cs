using AKA_Ability.Summon;
using HarmonyLib;
using Verse;

namespace AKA_Ability.HarmonyPatchs
{
    [HarmonyPatch(typeof(Pawn), "CanTakeOrder", MethodType.Getter)]
    public class Patch_AllowTakeOrder
    {
        [HarmonyPrefix]
        public static bool prefix(Pawn __instance, ref bool __result)
        {
            if (__instance.TryGetComp<TC_SummonedProperties>() != null)
            {
                __result = true;
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
