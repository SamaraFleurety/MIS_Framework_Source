using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace AK_DLL.HarmonyPatchs
{
    /*[HarmonyPatch(typeof(ModContentPack), "AddDef")]
    public static class Patch_DefsInMod
    {
        [HarmonyPrefix]
        public static bool prefix(Def def)
        {
            if (ShouldSkipDefname(def.defName))
            {
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }

        public static bool ShouldSkipDefname(string defname)
        {
            if (defname.Contains("Dusk")) return true;
            return false;
        }
    }*/
}
