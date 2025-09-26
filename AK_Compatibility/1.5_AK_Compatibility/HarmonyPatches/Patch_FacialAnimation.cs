using AK_DLL;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace PA_AKPatch
{
    [HarmonyPatch]
    public class Patch_FacialAnimation
    {
        public static Type DrawFaceGraphicsComp => AKPatches.FacialAnimation?.FirstOrDefault(t => t.Name == "DrawFaceGraphicsComp");
        public static Type FAHelper => AKPatches.FacialAnimation?.FirstOrDefault(t => t.Name == "FAHelper");

        [HarmonyPrepare]
        public static bool Prepare()
        {
            return ModLister.GetActiveModWithIdentifier("Nals.FacialAnimation") != null;
        }

        [HarmonyPatch]
        public class ParallelPreDraw
        {
            public static HashSet<Pawn> cachedPawn = new();

            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                return FAHelper?.GetMethod("ShouldDrawPawn", BindingFlags.Static | BindingFlags.Public);
            }

            [HarmonyPostfix]
            public static bool Postfix(bool value, Pawn pawn)
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


        [HarmonyPatch]
        public class CompRenderNodes
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                return DrawFaceGraphicsComp?.GetMethod("CompRenderNodes");
            }

            [HarmonyPostfix]
            public static void Postfix(ref List<PawnRenderNode> __result, Pawn ___pawn)
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
        }

    }
}
