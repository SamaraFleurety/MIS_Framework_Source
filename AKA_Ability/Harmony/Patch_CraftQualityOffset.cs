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
    [HarmonyPatch(typeof(QualityUtility), "GenerateQualityCreatedByPawn", new Type[] {typeof(Pawn), typeof(SkillDef), typeof(bool)})]
    public class Patch_CraftQualityOffset
    {
        [HarmonyPostfix]
        public static void Postfix(ref QualityCategory __result, Pawn pawn)
        {
            if (pawn != null)
            {
                int offset = (int)pawn.GetStatValue(AKADefOf.AK_Stat_CraftQualityOffset);
                if (offset != 0)
                {
                    int newQuality = (int)__result + offset;
                    if (newQuality < 0) newQuality = 0;
                    if (newQuality > 6) newQuality = 6;
                    __result = (QualityCategory)newQuality;
                }
            }
        }
    }
}
