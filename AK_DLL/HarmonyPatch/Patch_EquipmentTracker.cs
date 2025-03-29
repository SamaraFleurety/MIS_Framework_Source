using AKA_Ability;
using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick")]
    public class Patch_EquipmentTracker
    {
        public static void Postfix(Pawn_EquipmentTracker __instance)
        {
            List<ThingWithComps> allEquipmentListForReading = __instance.AllEquipmentListForReading;
            for (int i = 0; i < allEquipmentListForReading.Count; i++)
            {
                allEquipmentListForReading[i].GetComp<CompEquippable_AKAbility>()?.CompTick();
            }
        }
    }
}
