using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
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
        public static IEnumerable<CodeInstruction> tanspiler(IEnumerable<CodeInstruction> instructions)
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
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, overrideMethod)
                });
            }


            return list;
        }

        public static PawnRenderNodeProperties RenderTreeOverride(PawnRenderNodeProperties original, PawnRenderTree tree)
        {
            Pawn p = tree.pawn;
            OperatorDocument doc = p.GetDoc();
            if (doc == null) return original;

            return doc.operatorDef.renderTreeOverride == null ? original : doc.operatorDef.renderTreeOverride.root;
        }
    }
}
