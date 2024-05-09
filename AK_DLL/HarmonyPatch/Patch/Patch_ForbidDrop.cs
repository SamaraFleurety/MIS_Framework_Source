using System;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;

namespace AK_DLL
{
    /*[HarmonyPatch(typeof(JobDriver), "SetupToils",null)]
    public class Patch_ForbidDrop
    {
        [HarmonyPrefix]
        static bool prefix(JobDriver __instance) 
        {
            if (__instance is JobDriver_DropEquipment&&__instance.job.targetA.Thing.TryGetComp<CompOperatorWeapon>() != null) 
            {
                return false;
            }
            return true;
        }
    }*/
}
