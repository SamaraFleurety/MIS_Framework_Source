using HarmonyLib;
using RimWorld;
using System;
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
                StatDef def = AKADefOf.AKA_Stat_RangedDamageFactor;
                if (def == null) return;
                __result *= attacker.GetStatValue(def);
            }
        }
    }
}
