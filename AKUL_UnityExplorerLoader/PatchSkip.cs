using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityExplorer;
using Verse;

namespace AKUL_UnityExplorerLoader.HarmonyPatchs
{
    //好像是在patch unity，会报错missing method，不知道为啥
    [HarmonyPatch]
    public class PatchSkip
    {
        [HarmonyTargetMethod]
        public static MethodBase targetmethod()
        {
            var type = typeof(ExplorerStandalone).Assembly.GetType("UnityExplorer.Runtime.UnityCrashPrevention");
            return type.GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Static);
        }

        //会报错 不清楚原因，看了眼也不重要所以不加载
        [HarmonyPrefix]
        public static bool prefix(Harmony ___harmony)
        {
            return false;
        }
    }

    //把隐藏的log打出来
    [HarmonyPatch(typeof(ExplorerCore), "Log", new Type[] { typeof(object), typeof(LogType) })]
    public class Patch_ShowLog
    {
        [HarmonyPrefix]
        public static void prefix(object message, LogType logType)
        {
            string text = message?.ToString() ?? "";
            Log.Message(text);
        }
    }

    //同上 patchunity会报错
    //而且很神秘的是如果直接patch TimeScaleWidget::InitPatch()，methodinfo存在但是还是报错missing method
    [HarmonyPatch]
    public class Patch_SkipPatchUnity
    {
        [HarmonyTargetMethod]
        public static MethodBase targetmethod()
        {
            var type = typeof(ExplorerStandalone).Assembly.GetType("UnityExplorer.UI.Widgets.TimeScaleWidget");
            return type.GetConstructors().First();
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> transp(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = instructions.ToList();

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (list[i].opcode == OpCodes.Call)
                {
                    list[i] = new CodeInstruction(OpCodes.Ret);
                    return list;
                }
            }

            return list;
        }
    }
}