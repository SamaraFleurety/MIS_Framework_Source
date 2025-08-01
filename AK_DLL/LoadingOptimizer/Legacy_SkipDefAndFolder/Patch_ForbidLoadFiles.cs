﻿namespace AK_DLL.HarmonyPatchs
{
    /*
    //按路径，跳过部分资产文件夹的加载
    //此原函数仅加载文件夹下路径，不会读取文件内容。泰南会读取sounds和Textures内所有文件，无论是否被使用，此patch原理是假装某些路径的文件不存在。
    [HarmonyPatch(typeof(ModContentPack), "GetAllFilesForMod")]
    public static class Patch_ForbidLoadFiles
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            MethodInfo overrideMethod = typeof(Patch_ForbidLoadFiles).GetMethod("OverrideMethod", BindingFlags.Static | BindingFlags.Public);

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
            if (!Patch_SkipXmlLoad.ImplementLoadingOptimizer(mod) || AK_ModSettings.forbiddenAssets.NullOrEmpty()) return files;

            Log.Message("inner");
            AhoCorasick.Trie trie = new();
            foreach (string s in AK_ModSettings.forbiddenAssets)
            {
                Log.Message($"[AK] trie added {s}");
                trie.Add(s);
            }
            trie.Build();

            List<FileInfo> filesFiltered = new();

            foreach (FileInfo file in files)
            {
                if (trie.Find(file.FullName).Any())
                {
                    Log.Message($"[AK]skipped file {file.FullName}");
                    continue;
                }
                filesFiltered.Add(file);
            }
            return filesFiltered.ToArray();
        }
    }*/
}
