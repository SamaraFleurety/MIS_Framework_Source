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
        public static IEnumerable<CodeInstruction> Tanspiler(IEnumerable<CodeInstruction> instructions)
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
                    new(OpCodes.Ldarg, 0),
                    new(OpCodes.Ldloca, 3),
                    new(OpCodes.Ldloca, 6),
                    new(OpCodes.Ldarg, 1),
                    new(OpCodes.Ldloc, 1),
                    new(OpCodes.Call, overrideMethod)
                });
            }


            return list;
        }

        public static void AlterWeaponGraphics(Thing eq, ref Material material, ref Matrix4x4 matrix, Vector3 drawLoc, float rotate)
        {
            Ext_WeaponWieldGraphics ext = null;
            Pawn weaponHolder = (eq.ParentHolder as Pawn_EquipmentTracker)?.pawn;
            //主要用于舰娘，可以让舰娘持有的武器总是有个幻化
            if (weaponHolder != null && weaponHolder.GetDoc() is OperatorDocument doc)
            {
                ext = doc.pendingFashionDef?.GetModExtension<Ext_WeaponWieldGraphics>();
                ext ??= doc.operatorDef.GetModExtension<Ext_WeaponWieldGraphics>();
            }

            //武器可以在自己的def上面标记，持有替换贴图
            ext ??= eq.def.GetModExtension<Ext_WeaponWieldGraphics>();
            if (ext == null) return;

            Graphic alterGraphic = ext.DefaultGraphic(eq);
            material = (alterGraphic is not Graphic_StackCount graphic_StackCount) ? alterGraphic.MatSingleFor(eq) : graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingleFor(eq);
            matrix.SetTRS(pos: alterGraphic.DrawOffset(Rot4.South) + drawLoc, q: Quaternion.AngleAxis(rotate, Vector3.up), s: new Vector3(alterGraphic.drawSize.x, 0, alterGraphic.drawSize.y));
        }
    }
}
