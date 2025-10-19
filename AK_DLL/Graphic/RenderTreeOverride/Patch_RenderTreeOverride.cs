using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    //怎么没jb写注释
    //干员可以替换为别的渲染树
    /*
		if (Resolved)
		{
			return;
		}
		PawnRenderNodeProperties pawnRenderNodeProperties = pawn.RaceProps.renderTree?.root;  <--改变这玩意赋值结果
		if (pawnRenderNodeProperties?.nodeClass == null)
		{
			return;
		}
    */
    [HarmonyPatch(typeof(PawnRenderTree), "TrySetupGraphIfNeeded")]
    public class Patch_RenderTreeOverride
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            int index = -1;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Stloc_0)
                {
                    index = i;
                    break;
                }
            }


            MethodInfo overrideMethod = typeof(Patch_RenderTreeOverride).GetMethod("RenderTreeOverride", BindingFlags.Static | BindingFlags.Public);
            if (index != -1)
            {
                list.InsertRange(index, new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, overrideMethod)
                });
            }


            return list;
        }

        public static PawnRenderNodeProperties RenderTreeOverride(PawnRenderNodeProperties original, PawnRenderTree tree)
        {
            Pawn p = tree.pawn;
            OperatorDocument doc = p.GetDoc();
            if (doc == null || doc.operatorDef.renderTreeOverride == null) return original;

            //doc.operatorDef.renderTreeOverride.root.EnsureInitialized();

            return doc.operatorDef.renderTreeOverride.root;
        }
    }
}
