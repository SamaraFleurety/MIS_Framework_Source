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
    public static class PawnDiedOrDownedThoughtsUtility_AppendThoughts_ForHumanlike_Patch
    {
        private static void Postfix(Pawn victim, DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            if (victim.story.traits.HasTrait(TraitDef.Named("AK_Trait_SpecterUnchainedSecond")))
            {
                float inspirationChance = 0.5f;
                if(Rand.Chance(inspirationChance))
                {
                    InspirationDef randomInspiration = DefDatabase<InspirationDef>.AllDefsListForReading.RandomElement();
                    victim.mindState.inspirationHandler.TryStartInspiration(randomInspiration);
                }
            }
        }
    }

    [HarmonyPatch(typeof(InspirationHandler))]
    [HarmonyPatch("GetRandomAvailableInspirationDef")]
    public static class InspirationHandler_GetRandomAvailableInspirationDef_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref InspirationDef __result, Pawn pawn)
        {
            if (pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_SpecterUnchainedThird")))
            {
                __result = DefDatabase<InspirationDef>.AllDefsListForReading.Find(def => def == InspirationDefOf.Inspired_Creativity);
            }
        }
    }

    [HarmonyPatch(typeof(JobGiver_BingeDrug))]
    [HarmonyPatch("GetChemical")]
    public static class JobGiver_BingeDrug_GetChemical_Patch
    {
        [HarmonyPostfix]
        private static ChemicalDef Postfix(Pawn pawn)
        {
            if(((MentalState_BingingDrug)pawn.MentalState).chemical == ChemicalDefOf.Alcohol && pawn.story.traits.HasTrait(TraitDef.Named("AK_Trait_Blaze")))
            {
                InspirationDef randomInspiration = DefDatabase<InspirationDef>.AllDefsListForReading.RandomElement();
                pawn.mindState.inspirationHandler.TryStartInspiration(randomInspiration);
            }
            return ((MentalState_BingingDrug)pawn.MentalState).chemical;
        }
    }
}
