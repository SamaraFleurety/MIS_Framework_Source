using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace AK_DLL
{
    [HarmonyPatch(typeof(MainMenuDrawer), "Init")]
    public class PatchInit
    {
        [HarmonyPostfix]
        public static void postfix ()
        {
            AK_Tool.autoFillOperators();
        }
    }
}
