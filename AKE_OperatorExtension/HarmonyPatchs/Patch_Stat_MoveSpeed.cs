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
    //碧蓝有写的衣服以倍率加移速，写偏移很奇怪
    //写得很随手，不要学
    [HarmonyPatch(typeof(StatWorker), "GetValueUnfinalized")]
    public class Patch_Stat_MoveSpeed
    {
        [HarmonyPostfix]
        public static void postfix(StatWorker __instance, StatRequest req, ref float __result, StatDef ___stat)
        {
            if (___stat == StatDefOf.MoveSpeed)
            {
                float factor = (req.Thing as Pawn).GetStatValue(AKA_Ability.AKADefOf.AK_Stat_MoveSpeedFactor);
                __result *= factor;
            }
        }
    }
}
