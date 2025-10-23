using AK_DLL;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    //舟血条 技力条相关
    [HarmonyPatch]
    internal class Patch_PawnUIOverlay
    {
        //不应该显示原版血条
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(GenMapUI), "DrawPawnLabel", typeof(Pawn), typeof(Rect), typeof(float), typeof(float), typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool))]
        public static IEnumerable<CodeInstruction> Transpiler_DrawPawnLabel(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher codeMatcher = new(instructions);
            codeMatcher.Start();
            codeMatcher.MatchStartForward(
                new CodeMatch(OpCodes.Ldarg_S),
                new CodeMatch(OpCodes.Brtrue_S)
            );
            codeMatcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_PawnUIOverlay), nameof(ShouldDrawPawnLabel))),
                new CodeInstruction(OpCodes.Stloc_2)
            );
            return codeMatcher.Instructions();
        }

        //显示额外的流血速率
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PawnUIOverlay), nameof(PawnUIOverlay.DrawPawnGUIOverlay))]
        public static IEnumerable<CodeInstruction> Transpiler_DrawGUIOverlay(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher codeMatcher = new(instructions);
            codeMatcher.Start();
            codeMatcher.MatchStartForward(
                new CodeMatch(OpCodes.Call, AccessTools.FirstMethod(typeof(GenMapUI), method => method.Name == "DrawPawnLabel"))
            );
            codeMatcher.Advance(1);
            codeMatcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PawnUIOverlay), "pawn")),
                new CodeInstruction(OpCodes.Call, typeof(DrawGUIOverlayExtras).GetMethod(nameof(DrawGUIOverlayExtras.DrawBleedLabel)))
            );
            return codeMatcher.Instructions();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(RealTime), nameof(RealTime.Update))]
        public static void Postfix_UpdateTime()
        {
            GlobalFactor_Accumulator.Update();
        }

        public static float ShouldDrawPawnLabel(float value)
        {
            return AK_ModSettings.disable_displayPawnLabelHealth ? 1f : value;
        }
    }

    [HarmonyPatch]
    internal class Patch_PawnRenderer
    {
        [HarmonyPrepare]
        public static bool Prepare()
        {
            return ModLister.GetActiveModWithIdentifier("Nals.FacialAnimation") == null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PawnRenderer), "ParallelGetPreRenderResults")]
        public static void Prefix_ParallelGetPreRenderResults(Pawn ___pawn, ref bool disableCache)
        {
            if (___pawn.RaceProps.Humanlike && AK_ModSettings.drawOutOfCameraZoom)
            {
                disableCache = true;

            }
            //return HarmonyPrefixRet.keepOriginal;;
        }
    }
}
