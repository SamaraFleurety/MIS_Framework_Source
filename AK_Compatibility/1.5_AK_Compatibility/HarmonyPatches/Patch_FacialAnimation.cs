using AK_DLL;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace PA_AKPatch
{
    public class Patch_FacialAnimation
    {
        public static Type DrawFaceGraphicsComp => AKPatches.FacialAnimation?.FirstOrDefault(t => t.Name == "DrawFaceGraphicsComp");
        public static Type FAHelper => AKPatches.FacialAnimation?.FirstOrDefault(t => t.Name == "FAHelper");

        public static readonly HashSet<Pawn> cachedPawn = new();

        internal static void PatchAll(Harmony harmony)
        {
            if (ModLister.GetActiveModWithIdentifier("Nals.FacialAnimation") == null) return;

            MethodBase method1 = DrawFaceGraphicsComp?.GetMethod("CompRenderNodes");
            MethodBase method2 = FAHelper?.GetMethod("ShouldDrawPawn");

            harmony.Patch(method1, postfix: new HarmonyMethod(typeof(Patch_FacialAnimation), nameof(Postfix_CompRenderNodes)));
            harmony.Patch(method2, postfix: new HarmonyMethod(typeof(Patch_FacialAnimation), nameof(Postfix_ShouldDrawPawn)));
        }

        public static void Postfix_CompRenderNodes(ref List<PawnRenderNode> __result, Pawn ___pawn)
        {
            if (AKC_ModSettings.disable_FacialAnimation) return;

            var ext = ___pawn.kindDef.GetModExtension<Ext_MarkNLIncompatible>();
            var doc = ___pawn.GetDoc();
            if (ext != null || doc?.operatorDef.GetModExtension<Ext_MarkNLIncompatible>() != null)
            {
                __result = null;
                return;
            }
        }

        public static bool Postfix_ShouldDrawPawn(bool value, Pawn pawn)
        {
            if (cachedPawn.Contains(pawn)) return false;

            var ext = pawn.kindDef.GetModExtension<Ext_MarkNLIncompatible>();
            if (ext != null)
            {
                cachedPawn.Add(pawn);
                return false;
            }
            return value;
        }
    }
}
