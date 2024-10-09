using HarmonyLib;
using LMA_Lib.Traits;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace LMA_Lib.HarmonyPatchs
{
    [HarmonyPatch(typeof(Pawn), "TicksPerMove")]
    public class Patch_MovSpdOnWater
    {
        public static Dictionary<Pawn, float> cachedPawn = new();
        [HarmonyPostfix]
        public static void postfix(ref float __result, Pawn __instance, bool diagonal)
        {
            if (LMAConfigDef.ConfigDef.waterList.Contains(__instance.Map.terrainGrid.TerrainAt(__instance.Position)))
            {
                __result /= GetMoveSpeedOnWaterFor(__instance);
                __result = Mathf.Clamp(__result, 1f, 450f);
            }
        }

        private static float GetMoveSpeedOnWaterFor(Pawn pawn)
        {
            if (cachedPawn.ContainsKey(pawn)) return cachedPawn[pawn];

            if (!pawn.IsColonist) return 1;

            List<Trait> traits = pawn.story?.traits?.allTraits;
            if (traits == null) return 1;

            float factor = 1;

            foreach (Trait t in pawn.story.traits.allTraits)
            {
                Ext_MoveSpdOnWater ext = t.def.GetModExtension<Ext_MoveSpdOnWater>();
                if (ext != null) factor *= ext.speedFactor;
            }

            cachedPawn.Add(pawn, factor);

            return factor;
        }
    }
}