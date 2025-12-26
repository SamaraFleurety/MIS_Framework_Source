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
    [HarmonyPatch(typeof(InteractionWorker_KindWords), nameof(InteractionWorker_KindWords.RandomSelectionWeight))]
    public class Patch_KindWordWeight
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn initiator, Pawn recipient, ref float __result)
        {
            foreach (Trait trait in initiator.story.traits.allTraits)
            {
                if (trait.def.GetModExtension<Ext_TraitKind>() is Ext_TraitKind extTraitKind)
                {
                    __result = extTraitKind.kindWordsWeight;
                    return HarmonyPrefixRet.skipOriginal;

                }
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
