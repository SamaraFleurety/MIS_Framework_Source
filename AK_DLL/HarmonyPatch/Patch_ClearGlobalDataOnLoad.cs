using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Profile;

namespace AK_DLL
{
    [HarmonyPatch(typeof(MemoryUtility), "ClearAllMapsAndWorld")]
    public class Patch_ClearGlobalDataOnLoad
    {
        [HarmonyPrefix]
        public static void prefix()
        {
            if (GC_AKManager.instance != null) GC_AKManager.instance.PreLoad();
        }
    }
}