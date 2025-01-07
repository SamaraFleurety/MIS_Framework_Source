using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    //显示头时，身体倍率始终视为1（成年人的倍率）
    //Vector2 vector = pawn.story.bodyType.headOffset * Mathf.Sqrt(pawn.ageTracker.CurLifeStage.bodySizeFactor <- 在最后这个字段后面插入判断);
    [HarmonyPatch(typeof(PawnRenderer), "BaseHeadOffsetAt")]
    public class Patch_ForbidAgedHeadOffset
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> tanspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            int index = -1;
            FieldInfo fieldBodySizeFactor = typeof(LifeStageDef).GetField("bodySizeFactor", BindingFlags.Instance | BindingFlags.Public);
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].operand is FieldInfo fi && fi == fieldBodySizeFactor)
                {
                    index = i;
                    break;
                }
            }

            MethodInfo overrideMethod = typeof(Patch_ForbidAgedHeadOffset).GetMethod("BodySizeFactor", BindingFlags.Static | BindingFlags.Public);
            FieldInfo fieldPawnInsideRender = typeof(PawnRenderer).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic);
            if (index != -1)
            {
                list.InsertRange(index + 1, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, fieldPawnInsideRender),
                    new CodeInstruction(OpCodes.Call, overrideMethod)
                });
            }


            return list;
        }

        public static float BodySizeFactor(float factorOriginal, Pawn p)
        {
            if (p.GetDoc() == null) return factorOriginal;
            return 1;
        }

        /*[HarmonyPrefix]
        public static bool fix(PawnRenderer __instance, ref Vector3 __result, Pawn ___pawn)
        {
            OperatorDocument doc = ___pawn.GetDoc();
            if (doc == null) return HarmonyPrefixRet.keepOriginal;
            
            __result = Vector3.zero;
            return HarmonyPrefixRet.skipOriginal;
        }*/
    }
}
