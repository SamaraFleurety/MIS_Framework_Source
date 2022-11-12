using System;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Pawn), "Kill",new Type[] { typeof(DamageInfo),typeof(Hediff) })]
    public class Patch_KillCorpse
    {
        [HarmonyPostfix]
        public static void postfix(Pawn __instance) 
        {
            if (__instance.health.hediffSet.HasHediff(HediffDef.Named("AK_Operator")))
            {
                __instance.Corpse.Destroy();
            }
        }
    }
}
