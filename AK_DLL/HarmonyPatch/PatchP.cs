using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL {
    [HarmonyPatch(typeof(Dialog_InfoCard), "DoWindowContents")]
    public class PatchPortrait
    {
        [HarmonyPrefix]
        public static void postfix(Rect inRect)
        {
            Log.Message("SELECTED INFO");
        }
    }
}
