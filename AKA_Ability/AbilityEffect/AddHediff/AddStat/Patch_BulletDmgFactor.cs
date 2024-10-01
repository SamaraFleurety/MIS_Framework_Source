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
    [HarmonyPatch(typeof(VerbProperties), "GetDamageFactorFor", new Type[] { typeof(Tool), typeof(Pawn), typeof(HediffComp_VerbGiver) })]
    public class Patch_BulletDmgFactor
    {
        [HarmonyPostfix]
        public static void postfix(VerbProperties __instance, ref float __result, Tool tool, Pawn attacker, HediffComp_VerbGiver hediffCompSource)
        {
            if (attacker != null && !__instance.IsMeleeAttack)
            {
                __result *= attacker.GetStatValue(AKADefOf.AKA_Stat_RangedDamageFactor);
            }
        }
    }
}
