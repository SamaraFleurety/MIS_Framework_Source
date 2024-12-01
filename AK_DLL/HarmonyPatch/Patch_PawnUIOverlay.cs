using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Verse;
using UnityEngine;
using System;

namespace AK_DLL
{
    [HarmonyPatch(typeof(GenMapUI), "DrawPawnLabel", new Type[] { typeof(Pawn), typeof(Rect), typeof(float), typeof(float), typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool) })]
    internal class Patch_PawnLabel
    {
        public static bool DisablePawnLabel => !AK_ModSettings.disable_displayPawnLabelHealth;
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_PawnLabel(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            if (DisablePawnLabel)
            {
                return codes;
            }
            //IL_0038: stloc.1
            int index = codes.FindIndex(code => code.opcode == OpCodes.Stloc_1);
            codes.RemoveRange(index + 1, 5);
            codes.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldc_R4, 1f),
                new CodeInstruction(OpCodes.Stloc, 2)
            });
            return codes;
        }
    }
    [HarmonyPatch(typeof(PawnUIOverlay), "DrawPawnGUIOverlay")]
    internal static class Patch_PawnUIOverlay
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_DrawGUIOverlay(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            int index = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldarg_0 && codes[i + 1].opcode == OpCodes.Ldfld && codes[i + 2].opcode == OpCodes.Ldloc_0)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                MethodInfo methodInfo = typeof(DrawGUIOverlayExtras).GetMethod(nameof(DrawGUIOverlayExtras.AppendPawnGUIOverlayExtras), new[] { typeof(Pawn) });
                codes.InsertRange(index + 9, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PawnUIOverlay), "pawn")),
                    new CodeInstruction(OpCodes.Call, methodInfo)

                });
            }
            return codes;
        }
    }
    [HarmonyPatch(typeof(RealTime), "Update")]
    internal static class Patch_Factor_Accumulator
    {
        [HarmonyPostfix]
        public static void Postfix_UpdateTime()
        {
            GlobalFactor_Accumulator.Update();
        }
    }
    internal class Patch_PreRenderResults
    {
        public static bool Prefix_ParallelGetPreRenderResults(Pawn ___pawn, ref bool disableCache)
        {
            if (___pawn.RaceProps.Humanlike && AK_ModSettings.drawOutOfCameraZoom)
            {
                disableCache = true;
            }
            return true;
        }
    }
}
