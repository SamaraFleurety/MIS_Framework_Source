using HarmonyLib;
using UnityEngine;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker_Apparel_Body))]
    public class Patch_ApparelOffset
    {
        [HarmonyPatch("OffsetFor")]
        [HarmonyPostfix]
        public static void postfix_offset(ref Vector3 __result, PawnRenderNode n, PawnDrawParms parms)
        {
            Ext_ExtraGraphicData ext = n.apparel.def.GetModExtension<Ext_ExtraGraphicData>();
            if (ext != null)
            {
                Rot4 rot = parms.pawn.Rotation;
                __result.x += ext.graphicData.DrawOffsetForRot(rot).x;
                __result.y += ext.graphicData.DrawOffsetForRot(rot).y;
                __result.z += ext.graphicData.DrawOffsetForRot(rot).z;
            }
        }

        [HarmonyPatch("ScaleFor")]
        [HarmonyPostfix]
        public static void postfix_scale(ref Vector3 __result, PawnRenderNode n, PawnDrawParms parms)
        {
            Ext_ExtraGraphicData ext = n.apparel.def.GetModExtension<Ext_ExtraGraphicData>();
            if (ext != null)
            {
                Rot4 rot = parms.pawn.Rotation;
                __result.x *= ext.graphicData.drawSize.x;
                //__result.y += ext.graphicData.drawSize.y;
                __result.z *= ext.graphicData.drawSize.y;
            }
        }
    }
}
