using AK_DLL;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace AKE_OperatorExtension.HarmonyPatchs
{
    [HarmonyPatch(typeof(SkillRecord), "Interval")]
    public static class Patch_SkillLoss
    {
        private static Dictionary<Pawn, float> colonistsMap = new();
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (ModLister.GetActiveModWithIdentifier("ratys.madskills") != null) return instructions;

            List<CodeInstruction> list = instructions.ToList();

            MethodInfo overrideMethod = typeof(Patch_SkillLoss).GetMethod("CorrectSkillLoss", BindingFlags.Static | BindingFlags.Public);
            FieldInfo fieldPawn = typeof(SkillRecord).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

            int index = -1;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Stloc_0)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Log.Warning("[MIS] 未patch技能遗忘特性");
                return instructions;
            }

            list.InsertRange(index + 1, new CodeInstruction[]
                {
                    new(OpCodes.Ldloca_S, 0),
                    new(OpCodes.Ldarg, 0),
                    new(OpCodes.Ldfld, fieldPawn),
                    new(OpCodes.Call, overrideMethod)
                });

            return list;
        }

        public static void CorrectSkillLoss(ref float rate, Pawn p)
        {
            if (!p.IsColonist) return;

            if (Find.TickManager.TicksGame % (int)TimeToTick.day == 0)
            {
                colonistsMap.Clear();
                colonistsMap = new Dictionary<Pawn, float>();
            }

            if (!colonistsMap.ContainsKey(p))
            {
                float factor = 1;
                foreach (Trait i in p.story.traits.allTraits)
                {
                    DefExt_SkillLossRate ext = i.def.GetModExtension<DefExt_SkillLossRate>();
                    if (ext != null)
                    {
                        factor *= ext.skillLossFactor;
                    }
                }
                colonistsMap.Add(p, factor);
            }

            rate *= colonistsMap[p];
            //Log.Message($"[mis] rate {rate}");
        }
    }
}
