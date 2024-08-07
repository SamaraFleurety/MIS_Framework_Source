using AK_DLL;
using HarmonyLib;
using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AKE_OperatorExtension
{
    [HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility))]
    [HarmonyPatch("AppendThoughts_ForHumanlike")]
    public static class PawnDiedOrDownedThoughtsUtility_AppendThoughts_ForHumanlike_Patch  //在战斗中击杀敌人后，50%概率会产生一个灵感
    {
        private static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            Pawn pawn = (Pawn)dinfo.Value.Instigator;
            if (dinfo.HasValue && dinfo.Value.Instigator != null && pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_SpecterUnchainedSecond")))
            {
                float inspirationChance = 0.5f;
                if (Rand.Chance(inspirationChance))
                {
                    InspirationDef randomInspiration = DefDatabase<InspirationDef>.AllDefsListForReading.RandomElement();
                    pawn.mindState.inspirationHandler.TryStartInspiration(randomInspiration);
                }
            }
        }
    }

    [HarmonyPatch(typeof(InspirationHandler))]
    [HarmonyPatch("GetRandomAvailableInspirationDef")]
    public static class InspirationHandler_GetRandomAvailableInspirationDef_Patch  //只会产生创造灵感
    {
        [HarmonyPrefix]
        public static bool Prefix(ref InspirationDef __result, Pawn pawn)
        {
            if (pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_SpecterUnchainedThird")))
            {
                __result = DefDatabase<InspirationDef>.AllDefsListForReading.Find(def => def == InspirationDefOf.Inspired_Creativity);
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }

    [HarmonyPatch(typeof(JobGiver_BingeDrug))]
    [HarmonyPatch("IngestInterval")]
    public static class JobGiver_BingeDrug_IngestInterval_Patch  //饮用酒精后获得灵感
    {
        [HarmonyPrefix]
        private static void Prefix(JobGiver_BingeDrug __instance, Pawn pawn)
        {
            if (((MentalState_BingingDrug)pawn.MentalState).chemical == ChemicalDefOf.Alcohol && pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_Blaze")))
            {
                InspirationDef randomInspiration = DefDatabase<InspirationDef>.AllDefsListForReading.RandomElement();
                pawn.mindState.inspirationHandler.TryStartInspiration(randomInspiration);
            }
        }
    }
}
