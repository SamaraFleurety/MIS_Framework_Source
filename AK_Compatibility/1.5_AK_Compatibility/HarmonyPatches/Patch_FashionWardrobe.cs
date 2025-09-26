using AK_DLL;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace PA_AKPatch
{
    public class Patch_FashionWardrobe
    {
        public static Type SelApparelWindow => AKPatches.FashionWardrobe?.FirstOrDefault(t => t.FullName == "Fashion_Wardrobe.FW_Windows+SelApparelWindow");

        internal static void PatchAll(Harmony harmony)
        {
            if (ModLister.GetActiveModWithIdentifier("aedbia.fashionwardrobe") == null) return;

            MethodBase method1 = SelApparelWindow?.GetMethod("DoWindowContents");
            MethodBase method2 = SelApparelWindow?.GetMethod("Close");

            harmony.Patch(method1, prefix: new HarmonyMethod(typeof(Patch_FashionWardrobe), nameof(Prefix)));
            harmony.Patch(method2, prefix: new HarmonyMethod(typeof(Patch_FashionWardrobe), nameof(Postfix)));
        }

        public static void Prefix()
        {
            if (!OperatorDef.currentlyGenerating) OperatorDef.currentlyGenerating = true;
        }

        public static void Postfix()
        {
            if (OperatorDef.currentlyGenerating) OperatorDef.currentlyGenerating = false;
        }
    }
}
