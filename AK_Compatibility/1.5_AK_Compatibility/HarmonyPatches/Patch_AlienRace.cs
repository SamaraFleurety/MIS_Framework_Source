using AK_DLL;
using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace PA_AKPatch
{
    //种族修复
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_GeneratePawn
    {
        [HarmonyPrefix]
        public static void NewGeneratePawn_Prefix(ref PawnGenerationRequest request)
        {
            if (!ModsConfig.IsActive("erdelf.HumanoidAlienRaces")) return;

            if (!OperatorDef.currentlyGenerating || AKC_ModSettings.disable_PawnKindDef) return;
            if (request.KindDef.HasModExtension<Ext_KeepOriginal>()) return;

            request.KindDef = PawnKindDefOf.Colonist;
        }
    }
}
