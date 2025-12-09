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
            try
            {
                if (___stat == StatDefOf.MoveSpeed)
                {
                    if (req.Thing is not Pawn p) return;
                    float factor = p.GetStatValue(AKA_Ability.AKADefOf.AK_Stat_MoveSpeedFactor);
                    __result *= factor;
                    if (__result <= 0.1) __result = 0.1f;
                }
            }
            catch (Exception e)
            {
                Log.ErrorOnce($"[AKE]更改移速失败 {e.StackTrace}", 114515);
            }
        }
    }
}
