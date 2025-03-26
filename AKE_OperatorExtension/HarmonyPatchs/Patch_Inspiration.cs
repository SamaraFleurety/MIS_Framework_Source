using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AKE_OperatorExtension.HarmonyPatchs
{
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility))]
    [HarmonyPatch("AppendThoughts_ForHumanlike")]
    public static class PawnDiedOrDownedThoughtsUtility_AppendThoughts_ForHumanlike_Patch  //在战斗中击杀敌人后，50%概率会产生一个灵感
    {
        private static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            if (dinfo == null || !dinfo.HasValue || dinfo.Value.Instigator == null)
            {
                return;
            }
            Pawn pawn = (Pawn)dinfo.Value.Instigator;
            if (pawn == null) { return; }
            if (pawn.story?.traits?.HasTrait(TraitDef.Named("AK_Trait_SpecterUnchainedSecond")) == true)
            {
                float inspirationChance = 0.5f;
                if (Rand.Chance(inspirationChance))
                {
                    pawn.mindState?.inspirationHandler?.TryStartInspiration(InspirationDefOf.Inspired_Creativity);
                }
            }
        }
    }

    [HarmonyPatch(typeof(InspirationHandler))]
    [HarmonyPatch("GetRandomAvailableInspirationDef")]
    public static class InspirationHandler_GetRandomAvailableInspirationDef_Patch  //只会产生创造灵感
    {
        [HarmonyPrefix]
        public static bool Prefix(ref InspirationDef __result, InspirationHandler __instance)
        {
            Pawn pawn = __instance.pawn;
            if (pawn.story == null || pawn.story.traits == null) return HarmonyPrefixRet.keepOriginal;
            if (pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_SpecterUnchainedThird")))
            {
                __result = DefDatabase<InspirationDef>.AllDefsListForReading.Find(def => def == InspirationDefOf.Inspired_Creativity);
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }

    [HarmonyPatch(typeof(IngestionOutcomeDoer_GiveHediff))]
    [HarmonyPatch("DoIngestionOutcomeSpecial")]
    public static class IngestionOutcomeDoer_GiveHediff_DoIngestionOutcomeSpecial_Patch  //饮用酒精后获得灵感
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, Thing ingested)
        {
            if (pawn == null || ingested == null || pawn.story == null || pawn.story.traits == null) return;
            CompProperties_Drug tcp = ingested.def.GetCompProperties<CompProperties_Drug>();
            if (tcp == null) return;
            if (tcp.chemical == ChemicalDefOf.Alcohol && pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_Blaze")))
            {
                InspirationDef randomInspiration = DefDatabase<InspirationDef>.AllDefsListForReading.RandomElement();
                pawn.mindState.inspirationHandler.TryStartInspiration(randomInspiration);
            }
        }
    }
}
