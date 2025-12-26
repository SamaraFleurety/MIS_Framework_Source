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
    [HarmonyPatch(typeof(NegativeInteractionUtility), nameof(NegativeInteractionUtility.NegativeInteractionChanceFactor))]
    public class Patch_InsultFactor
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn initiator, Pawn recipient, ref float __result)
        {
            foreach (Trait trait in initiator.story.traits.allTraits)
            {
                if (trait.def.GetModExtension<Ext_TraitKind>() is Ext_TraitKind extTraitKind)
                {
                    __result = extTraitKind.insultFactor;
                    return HarmonyPrefixRet.skipOriginal;
                }
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
