using AK_DLL;
using AlienRace;
using HarmonyLib;
using System;
using Verse;

namespace Paluto22.AK.Patch.AlienRace
{
    [HarmonyPatch(typeof(AlienRenderTreePatches), "HeadGraphicForPrefix")]
    [HarmonyPatch(new Type[] { typeof(PawnRenderNode_Head), typeof(Pawn), typeof(Graphic) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
    public class Patch_HeadGraphicForPrefix
    {
        //对于非外星人种族小人,外星人的patch把GraphicFor原方法直接跳过了,傻逼 所以只能改它这个prefix
        //patch的是prefix返回的bool值 原prefix的目标方法HeadGraphicFor不能跳过,返回值__result=1 原prefix返回值决定它是否执行 可返回0
        [HarmonyPrefix]
        public static bool HeadGraphicForPrefix_Prefix(ref bool __result, PawnRenderNode_Head __instance, Pawn pawn)
        {
            __result = true;
            if (OperatorDef.currentlyGenerating == true || pawn.GetDoc() != null)
            {
                return false;
            }
            return true;
        }
    }
}
