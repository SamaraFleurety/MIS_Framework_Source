using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SFLib
{
    //小人如果穿上带特定comp的衣服就不显示衣服 ret null会导致身体和衣服都不渲染
    [HarmonyPatch(typeof(PawnRenderNode_Body), "GraphicFor")]
    public class Patch_TCPHideBody
    {
        public static HashSet<Pawn> registeredPawns = new HashSet<Pawn>();
        [HarmonyPrefix]
        public static bool prefix(ref Graphic __result, Pawn pawn, PawnRenderNode_Body __instance)
        {
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
    }
}
