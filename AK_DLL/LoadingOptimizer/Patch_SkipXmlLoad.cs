using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    /*[HarmonyPatch(typeof(DirectXmlLoader), "DefFromNode")]
    public class Patch_test
    {
        [HarmonyPostfix]
        public static void fix(ref Def __result, XmlNode node, LoadableXmlAsset loadingAsset)
        {
            if (__result == null) return;

            if (__result.defName.Contains("AK_"))
            {
                //Log.Message($"found skipable def name {__result.defName}");
                __result = null;
            }
            //Log.Message($"loading asset name: {loadingAsset.name}");
        }
    }

    [HarmonyPatch(typeof(GenDefDatabase), "GetDefSilentFail")]
    public static class patch2
    {
        [HarmonyPrefix]
        public static bool fix(ref Def __result, Type type, string targetDefName)
        {
            if (targetDefName.Contains("AK_"))
            {
                __result = (Def)Activator.CreateInstance(type);
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }*/

    //按xml文件路径跳过读取
    [HarmonyPatch(typeof(DirectXmlLoader), "XmlAssetsInModFolder")]
    public static class Patch_SkipXmlLoad
    {
        /*FileInfo[] files = directoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);
         *     -----------插入点差不多在这，在getfiles后，先删掉想要跳过的file路径
                foreach (FileInfo fileInfo in files)
                {
                    string key = fileInfo.FullName.Substring(text.Length + 1);
                    if (!dictionary.ContainsKey(key))
                    .....
                }
        }*/
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            MethodInfo overrideMethod = typeof(Patch_SkipXmlLoad).GetMethod("OverrideMethod", BindingFlags.Static | BindingFlags.Public);

            int index = -1;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Stloc_S && ((LocalBuilder)list[i].operand).LocalIndex == 6)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Log.Warning("[MIS] 未patch xml loader");
                return instructions;
            }

            list.InsertRange(index, new CodeInstruction[]
                {
                    //new CodeInstruction(OpCodes.Ldloc_S, 8),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, overrideMethod)
                });

            return list;
        }

        public static FileInfo[] OverrideMethod(FileInfo[] files, ModContentPack mod)
        {
            if (!ImplementLoadingOptimizer(mod) /*|| AK_ModSettings.forbiddenXmls.NullOrEmpty()*/) return files;

            if (AK_ModSettings.forbiddenXmls.NullOrEmpty()) return files;
            List<FileInfo> list = new List<FileInfo>();

            AhoCorasick.Trie trie = new();
            foreach (string s in AK_ModSettings.forbiddenXmls)
            {
                Log.Message($"[AK] trie added {s}");
                trie.Add(s);
            }
            trie.Build();

            foreach (FileInfo file in files)
            {
                if (trie.Find(file.FullName).Any())
                {
                    Log.Message($"[AK]skipped file {file.FullName}");
                    continue;
                }
                list.Add(file);
            }

            return list.ToArray();
        }

        //此mod是否可选职业 当前仅舟主包可以
        public static bool ImplementLoadingOptimizer(ModContentPack mod)
        {
            return !mod.IsOfficialMod && mod.PackageId != null && mod.PackageId.Equals("MIS.Arknights".ToLower());
        }

        /*public static bool OverrideJudge(bool containsKey, FileInfo fileInfo, ModContentPack mod)
        {
            Log.Message("judge a");
            if (!AK_Tool.ImplementLoadingOptimizer(mod)) return containsKey;

            Log.Message("judge b");
            AhoCorasick.Trie trie = new();
            foreach (string s in AK_ModSettings.forbiddenXmls)
            {
                trie.Add(s);
            }
            Log.Message("judge c");
            trie.Build();
            Log.Message("judge d");
            if (containsKey || trie.Find(fileInfo.FullName).Any() /*|| fileInfo.Name.Contains("_Defender")|| fileInfo.FullName.Contains("Main") /*|| fileInfo.Name.Contains("Guard") || fileInfo.Name.Contains("_Supporter") || fileInfo.Name.Contains("_Medic") || fileInfo.Name.Contains("_Sniper") || fileInfo.Name.Contains("_Specialist")*//*)
            {
                Log.Message($"skipped xml: {fileInfo.FullName}");
                return true;
            }
            return false;
        }*/
    }
}
