using AK_DLL;
using AlienRace;
using HarmonyLib;
using System;
using Verse;

namespace Paluto22.AK.Patch.AlienRace
{
    [HarmonyPatch(typeof(AlienRenderTreePatches), "BodyGraphicForPrefix")]
    [HarmonyPatch(new Type[] { typeof(PawnRenderNode_Body), typeof(Pawn), typeof(Graphic) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref })]
    public class Patch_BodyGraphicForPrefix
    {
        [HarmonyPrefix]
        public static bool BodyGraphicForPrefix_Prefix(ref bool __result, PawnRenderNode_Body __instance, Pawn pawn)
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
