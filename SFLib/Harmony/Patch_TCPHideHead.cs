using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace SFLib
{

    /*[HarmonyPatch(typeof(PawnRenderNode_Head), "GraphicFor")]
    public class Patch_TCPHideHead
    {
        public static bool forceHideHead = false;
        public static HashSet<Pawn> registeredPawns = new HashSet<Pawn>();
        [HarmonyPrefix]
        public static bool prefix(ref Graphic __result, Pawn pawn, PawnRenderNode_Head __instance)
        {
            if (forceHideHead) return HarmonyPrefixRet.skipOriginal;
            Shader shader = __instance.ShaderFor(pawn);
            if (shader == null)
            {
                return HarmonyPrefixRet.keepOriginal;
            }
            if (registeredPawns.Contains(pawn))
            {
                __result = GraphicDatabase.Get<Graphic_Single>("transp", shader);
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }*/

    [HarmonyPatch(typeof(PawnRenderTree))]
    public class Patch_TCPHideHead
    {
        public static bool forceHideHead = false;
        public static HashSet<Pawn> registeredPawns = new HashSet<Pawn>();

        [HarmonyPatch("ParallelPreDraw", new Type[] { typeof(PawnDrawParms) })]
        [HarmonyPostfix]
        public static void Postfix(ref List<PawnGraphicDrawRequest> ___drawRequests, PawnDrawParms parms)
        {
            if (forceHideHead || registeredPawns.Contains(parms.pawn))
            {
                ___drawRequests.RemoveAll(req => req.node.Props.skipFlag == RenderSkipFlagDefOf.Head);
            }
        }
    }
}
