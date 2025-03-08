using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using Verse;
using System.IO;
using System.Xml;
using UnityEngine.TextCore;

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
        /*foreach (FileInfo fileInfo in files)
            {
                string key = fileInfo.FullName.Substring(text.Length + 1);
                if (!dictionary.ContainsKey(key))    <--------------插入点在这，等于在括号里面的末尾加了个 &&不在跳过路径内
                {
                    dictionary.Add(key, fileInfo);
                }
}       */
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            MethodInfo overrideMethod = typeof(Patch_SkipXmlLoad).GetMethod("OverrideJudge", BindingFlags.Static | BindingFlags.Public);
            //FieldInfo fieldPawn = typeof(SkillRecord).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

            int index = -1;
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Ldloc_S && ((LocalBuilder)list[i].operand).LocalIndex == 9)
                {
                    if (list[i + 1].opcode == OpCodes.Call || list[i + 1].opcode == OpCodes.Callvirt)
                    {
                        index = i + 1;
                        break;
                    }
                }
            }

            if (index == -1)
            {
                Log.Warning("[MIS] 未patch xml loader");
                return instructions;
            }

            list.InsertRange(index + 1, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldloc_S, 8),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, overrideMethod)
                });

            return list;
        }

        public static bool OverrideJudge(bool containsKey, FileInfo fileInfo, ModContentPack mod)
        {
            if (!AK_Tool.ImplementLoadingOptimizer(mod)) return containsKey;

            AhoCorasick.Trie trie = new();
            foreach (string s in AK_ModSettings.forbiddenXmls)
            {
                trie.Add(s);
            }
            trie.Build();
            if (containsKey || trie.Find(fileInfo.FullName).Any() /*|| fileInfo.Name.Contains("_Defender")|| fileInfo.FullName.Contains("Main") /*|| fileInfo.Name.Contains("Guard") || fileInfo.Name.Contains("_Supporter") || fileInfo.Name.Contains("_Medic") || fileInfo.Name.Contains("_Sniper") || fileInfo.Name.Contains("_Specialist")*/)
            {
                Log.Message($"skipped xml: {fileInfo.FullName}");
                return true;
            }
            return false;
        }
    }
}
