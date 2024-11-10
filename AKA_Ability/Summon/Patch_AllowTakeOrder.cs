using AKA_Ability.Summon;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
