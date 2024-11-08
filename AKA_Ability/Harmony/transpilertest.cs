using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    /*[HarmonyPatch(typeof(DebugToolsGeneral), "Kill")]
    public class transpilertest
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Callvirt)
                {
                    Log.Message("patched 111");
                    list[i].opcode = OpCodes.Call;
                }
            }
            return list;
        }
    }*/
}
