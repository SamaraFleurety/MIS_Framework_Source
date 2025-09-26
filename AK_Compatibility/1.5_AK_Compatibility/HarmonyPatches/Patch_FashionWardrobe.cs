using AK_DLL;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PA_AKPatch
{
    [HarmonyPatch]
    public class Patch_FashionWardrobe
    {
        public static Type SelApparelWindow => AKPatches.FashionWardrobe?.FirstOrDefault(t => t.FullName == "Fashion_Wardrobe.FW_Windows+SelApparelWindow");

        [HarmonyPrepare]
        public static bool Prepare()
        {
            return ModLister.GetActiveModWithIdentifier("aedbia.fashionwardrobe") != null;
        }

        [HarmonyPatch]
        public class DoWindowContents
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                return SelApparelWindow?.GetMethod("DoWindowContents");
            }

            [HarmonyPrefix]
            public static void Prefix(Rect inRect)
            {
                if (!OperatorDef.currentlyGenerating) OperatorDef.currentlyGenerating = true;
            }
        }

        [HarmonyPatch]
        public class Close
        {
            [HarmonyTargetMethod]
            public static MethodBase TargetMethod()
            {
                return SelApparelWindow?.GetMethod("Close");
            }

            [HarmonyPrefix]
            public static void Postfix(bool doCloseSound = true)
            {
                if (OperatorDef.currentlyGenerating) OperatorDef.currentlyGenerating = false;
            }
        }
    }
}
