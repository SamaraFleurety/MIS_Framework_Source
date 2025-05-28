using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace AKUL_UnityExplorerLoader.HarmonyPatchs
{
    [HarmonyPatch]
    public static class Patch_KeyBindingDef
    {
        public static readonly string[] Targets =
        {
        "get_KeyDownEvent",
        "get_IsDownEvent",
        "get_JustPressed",
        "get_IsDown"
    };

        [HarmonyTargetMethods]
        public static IEnumerable<MethodBase> TargetMethods()
        {
            return AccessTools.GetDeclaredMethods(typeof(KeyBindingDef))
                .Where(method => Targets.Contains(method.Name));
        }

        [HarmonyPrefix]
        public static bool PassKeyInput_Prefix() => !UnityExplorer.UI.UIManager.ShowMenu || !KeyInputToggleWidget.Locked;
    }
}