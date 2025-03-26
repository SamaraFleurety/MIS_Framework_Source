using AKA_Ability.Summon;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AKA_Ability.HarmonyPatchs
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public class Patch_AllowTakeOrder
    {
        [HarmonyPrefix]
        public static bool prefix(Pawn pawn, ref bool __result)
        {
            if (pawn.TryGetComp<TC_SummonedProperties>() != null)
            {
                __result = true;
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
