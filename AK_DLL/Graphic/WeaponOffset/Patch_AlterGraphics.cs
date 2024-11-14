using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    //在渲染武器之前改变贴图
    [HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming")]
    public class Patch_AlterGraphics
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> tanspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            int index = -1;
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].opcode == OpCodes.Ldloc_3)
                {
                    index = i;
                    break;
                }
            }

            MethodInfo overrideMethod = typeof(Patch_AlterGraphics).GetMethod("AlterWeaponGraphics", BindingFlags.Static | BindingFlags.Public);
            if (index != -1)
            {
                list.InsertRange(index - 2, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg, 0),
                    new CodeInstruction(OpCodes.Ldloca, 3),
                    new CodeInstruction(OpCodes.Ldloca, 6),
                    new CodeInstruction(OpCodes.Ldarg, 1),
                    new CodeInstruction(OpCodes.Ldloc, 1),
                    new CodeInstruction(OpCodes.Call, overrideMethod)
                });
            }


            return list;
        }

        public static void AlterWeaponGraphics(Thing eq, ref Material material, ref Matrix4x4 matrix, Vector3 drawLoc, float rotate)
        {
            //float rotate = 1;
            Ext_WeaponWieldGraphics ext = eq.def.GetModExtension<Ext_WeaponWieldGraphics>();
            if (ext == null) return;

            Graphic alterGraphic = ext.DefaultGraphic(eq);
            material = ((!(alterGraphic is Graphic_StackCount graphic_StackCount)) ? alterGraphic.MatSingleFor(eq) : graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingleFor(eq));
            //matrix.SetTRS(pos: ext.alterGraphics.DrawOffsetForRot(Rot4.South), q: Quaternion.AngleAxis(1, Vector3.up), s: new Vector3(ext.alterGraphics.drawSize.x, 1, ext.alterGraphics.drawSize.y)); //pos位置偏移, q旋转四元数，s缩放
            matrix.SetTRS(pos: alterGraphic.DrawOffset(Rot4.South) + drawLoc, q: Quaternion.AngleAxis(rotate, Vector3.up), s: new Vector3(alterGraphic.drawSize.x, 0, alterGraphic.drawSize.y));
        }
    }
}
