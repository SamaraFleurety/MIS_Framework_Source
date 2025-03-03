using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace AK_DLL.HarmonyPatchs
{
    //按路径，跳过部分资产文件夹的加载
    [HarmonyPatch(typeof(ModContentPack), "GetAllFilesForMod")]
    public static class Patch_ForbidLoadFiles
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions;
            List<CodeInstruction> list = instructions.ToList();

            MethodInfo overrideMethod = typeof(Patch_ForbidLoadFiles).GetMethod("OverrideMethod", BindingFlags.Static | BindingFlags.Public);
            //FieldInfo fieldPawn = typeof(SkillRecord).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

            int index = -1;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Stloc_S && ((LocalBuilder)list[i].operand).LocalIndex == 5)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Log.Warning("[MIS] 未patch file loader");
                return instructions;
            }

            list.InsertRange(index, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, overrideMethod)
                });

            return list;
        }

        public static FileInfo[] OverrideMethod(FileInfo[] files, ModContentPack mod)
        {
            if (!mod.PackageId.Contains("mis")) return files;

            List<FileInfo> filesFiltered = new();

            foreach (FileInfo file in files)
            {
                //Log.Message($"file: {file.Directory.FullName}");
                if (file.Directory.FullName.Contains("Main")) continue;
                filesFiltered.Add(file);
            }
            return filesFiltered.ToArray();
        }
    }
}
