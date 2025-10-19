using HarmonyLib;
using System;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_OverridePawnGeneration
    {
        [HarmonyPrefix]
        public static bool Prefix(PawnGenerationRequest request, ref Pawn __result)
        {
            if (OperatorDef.currentlyGenerating || request.KindDef.GetModExtension<Ext_LinkedReservedOperatorDef>() is not Ext_LinkedReservedOperatorDef ext)
            {
                return HarmonyPrefixRet.keepOriginal;
            }

            ext.linkedDef.cachedFaction = request.Faction;
            __result = ext.linkedDef.Recruit_NoMap();
            ext.linkedDef.cachedFaction = null;

            return HarmonyPrefixRet.skipOriginal;
        }
    }
}
